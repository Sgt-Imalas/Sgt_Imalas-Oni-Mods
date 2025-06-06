using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Disease
{
	/// <summary>
	/// Several chemicals in the game can cause acid burns to dupes. This monitor checks for hazardous chemicals and applies damage accordingly.
	/// </summary>
	public class AcidBurnMonitor : KMonoBehaviour
	{
		[MyCmpReq] Health health;
		[MyCmpReq] KSelectable selectable;
		[MyCmpGet] SuitEquipper suitEquipper;

		public static StatusItem Status_item {
		get
			{
				if (status_item == null)
				{
					MakeStatusItem();
				}
				return status_item;
			}
			set => status_item = value;

		}
		private static StatusItem status_item = null;

		public static void MakeStatusItem()
		{
			status_item = new StatusItem("AcidBurns", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486, null);
			status_item.AddNotification();
		}

		public void CheckForAcidChemicals(float dt)
		{
			AcidChemical corrosiveChemical = this.AcidChemicalSearch();
			if (corrosiveChemical != null && health && (double)Time.time - (double)this.lastBurnTime > 5.0)
			{
				health.Damage(corrosiveChemical.damageDealt);
				this.lastBurnTime = Time.time;
				selectable.AddStatusItem(Status_item, this);
				return;
			}
			if (Time.time - this.lastBurnTime <= 5.0)
			{
				return;
			}
			selectable.RemoveStatusItem(Status_item, this);
		}

		public bool IsHazardousChemical(SimHashes chemical, float mass)
		{
			return this.chemicalList.ContainsKey(chemical) && mass > this.chemicalList[chemical].criticalMass;
		}

		public AcidChemical AcidChemicalSearch()
		{
			if (!KPrefabIDExtensions.HasTag(base.gameObject, GameTags.Dead) && !suitEquipper.IsWearingAirtightSuit())
			{
				int cell = Grid.PosToCell(this);
				Element element = Grid.Element[cell];
				float mass = Grid.Mass[cell];
				if (this.IsHazardousChemical(element.id, mass))
				{
					return this.chemicalList[element.id];
				}
				int upperCell = Grid.CellAbove(cell);
				Element element2 = Grid.Element[upperCell];
				float mass2 = Grid.Mass[upperCell];
				if (this.IsHazardousChemical(element2.id, mass2))
				{
					return this.chemicalList[element2.id];
				}
			}
			return null;
		}

		public AcidBurnMonitor()
		{
			Dictionary<SimHashes, AcidBurnMonitor.AcidChemical> dictionary = new Dictionary<SimHashes, AcidBurnMonitor.AcidChemical>
		{
			{
				SulphuricAcid_Liquid.SimHash,
				new AcidBurnMonitor.AcidChemical
				{
					criticalMass = 0.1f,
					damageDealt = 10f
				}
			},
			{
				SulphuricAcid_Gas.SimHash,
				new AcidBurnMonitor.AcidChemical
				{
					criticalMass = 0.1f,
					damageDealt = 10f
				}
			},
			{
				ToxicMix_Liquid.SimHash,
				new AcidBurnMonitor.AcidChemical
				{
					criticalMass = 0.1f,
					damageDealt = 10f
				}
			},
			{
				ToxicMix_Gas.SimHash,
				new AcidBurnMonitor.AcidChemical
				{
					criticalMass = 0.1f,
					damageDealt = 10f
				}
			},
			{
				NitricAcid_Liquid.SimHash,
				new AcidBurnMonitor.AcidChemical
				{
					criticalMass = 0.1f,
					damageDealt = 20f
				}
			}
		};
			this.chemicalList = dictionary;
		}

		public Dictionary<SimHashes, AcidBurnMonitor.AcidChemical> chemicalList;
		public float lastBurnTime;

		public class AcidChemical
		{
			public float criticalMass;
			public float damageDealt = 10f;
		}
	}
}

using Imalas_TwitchChaosEvents.Elements;
using System.Collections.Generic;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Attachments
{
	internal class Health_DamageHander : KMonoBehaviour, ISim1000ms
	{
		[MyCmpReq]
		Health _health;

		[MyCmpGet]
		SuitEquipper suitEquipper;

		[MyCmpGet]
		KSelectable selectable;
		[MyCmpGet]
		KPrefabID kPrefabID;
		public void Sim1000ms(float dt)
		{
			DamageChecker(dt);
		}

		public static void AddDamagingMaterial(SimHashes elementId, float minMassForDamage, float dps, float dpsSuited)
		{
			damagingMaterials[elementId] = new DamagingMaterial() { criticalMass = minMassForDamage, damagePerSecond = dps, damagePerSecondSuited = dpsSuited };
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (status_item == null)
				status_item = MakeStatusItem();
		}

		public static Dictionary<SimHashes, DamagingMaterial> damagingMaterials = new Dictionary<SimHashes, DamagingMaterial>()
		{
			{
				ModElements.Creeper.SimHash,
				new DamagingMaterial()
				{
					criticalMass = 0.1f,
					damagePerSecond = 100f/60f,
					//damagePerSecondSuited= 100f/600f,
				}
			}
		};
		public float lastBurnTime;
		public static StatusItem status_item;

		public static StatusItem MakeStatusItem()
		{
			StatusItem statusItem = new StatusItem("ITCE_HurtingElement", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 63486);
			statusItem.AddNotification();
			return statusItem;
		}

		float DealHealthDamage(DamagingMaterial damagingElement, float dt)
		{
			float damage = damagingElement.damagePerSecond * dt;
			if (suitEquipper != null && suitEquipper.IsWearingAirtightSuit())
			{
				damage = damagingElement.damagePerSecondSuited * dt;
			}
			_health.Damage(damage);
			return damage;
		}
		public bool IsMinion()
		{
			return kPrefabID.HasTag(GameTags.BaseMinion);
		}

		public void DamageChecker(float dt)
		{
			DamagingMaterial damagingElement = this.DamagingElementSearch();
			if (damagingElement != null)
			{
				DealHealthDamage(damagingElement, dt); 
				if (IsMinion())
				{
					this.lastBurnTime = Time.time;
					selectable.AddStatusItem(status_item, (object)this);
				}
			}
			else if(IsMinion())
			{
				if (Time.time - lastBurnTime > 5.0)
					selectable.RemoveStatusItem(status_item, (bool)this);
			}
		}

		public bool IsDamagingElement(SimHashes chemical, float mass) => damagingMaterials.ContainsKey(chemical) && mass > damagingMaterials[chemical].criticalMass;

		public DamagingMaterial DamagingElementSearch()
		{
			if (!this.gameObject.HasTag(GameTags.Dead) && !this.gameObject.HasTag(GameTags.Robot))
			{
				int cell = Grid.PosToCell(this.gameObject);
				if (!Grid.IsValidCell(cell))
					return (DamagingMaterial)null;

				Element element1 = Grid.Element[cell];
				float mass1 = Grid.Mass[cell];

				if (this.IsDamagingElement(element1.id, mass1))
					return damagingMaterials[element1.id];
				if (suitEquipper != null)
				{
					int i = Grid.CellAbove(Grid.PosToCell(this.gameObject));
					if (!Grid.IsValidCell(i))
						return (DamagingMaterial)null;

					Element element2 = Grid.Element[i];
					float mass2 = Grid.Mass[i];
					if (this.IsDamagingElement(element2.id, mass2))
						return damagingMaterials[element2.id];
				}

			}
			return (DamagingMaterial)null;
		}

		public class DamagingMaterial
		{
			public float criticalMass = 0.0f;
			public float damagePerSecond = 10f;
			public float damagePerSecondSuited = 0f;
		}
	}
}

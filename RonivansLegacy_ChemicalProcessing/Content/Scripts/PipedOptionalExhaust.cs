using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;
using UnityEngine;
using UtilLibs;
using KSerialization;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class PipedOptionalExhaust : KMonoBehaviour, ISim200ms
	{
		[SerializeField]
		public PipedConduitDispenser dispenser;

		[SerializeField]
		public List<Tag> elementTags = null;

		[SerializeField]
		public bool invertedFilter = false;

		[SerializeField]
		public float capacity = float.MaxValue;

		[SerializeField]
		public Storage storage;

		[SerializeField]
		public float emissionRate = 25f;

		[SerializeField]
		public float OverpressureThreshold = float.MaxValue;

		[MyCmpGet] ComplexFabricator complexfab;
		[MyCmpGet] KSelectable selectable;

		private Operational.Flag outputFlag;

		[MyCmpReq]
		readonly private Operational operational;

		private Guid pipeBlockedGuid;

		private Tag elementState = null;

		private static readonly CellElementEvent SpawnEvent = new(
	"Chemical_ExhaustSpawned",
	"Spawned by piped optional exhaust",
	true);

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (storage == null)
			{
				if (dispenser != null)
				{
					storage = dispenser.storage;
				}
				else if (complexfab != null)
				{
					storage = complexfab.outStorage;
				}
				else
				{
					storage = this.gameObject.AddOrGet<Storage>();
				}
			}
			if (dispenser != null)
			{
				dispenser.SkipSetOperational = true;
				elementState = dispenser.conduitType switch
				{
					ConduitType.Solid => GameTags.Solid,
					ConduitType.Liquid => GameTags.Liquid,
					ConduitType.Gas => GameTags.Gas,
					_ => null
				};
			}
			else
				SgtLogger.error("DISPENSER NULL ON: " + this.gameObject.name + " with tag: " + string.Join(',', elementTags));

			string operationalFlag = "output_blocked_";
			if (elementTags != null && elementTags.Any())
				operationalFlag += string.Join('_', elementTags);
			if (invertedFilter)
				operationalFlag += "_inverted";
			operationalFlag += "_" + capacity;
			operationalFlag += "_" + dispenser.conduitType.ToString();
			operationalFlag += "_" + dispenser.conduitOffset.ToString();


			outputFlag = new Operational.Flag(operationalFlag, Operational.Flag.Type.Functional);
		}

		GameObject GetFirstViableStoredItem()
		{
			bool elementLocked = elementState != null;

			if (!elementTags.Any() && elementState == null)
			{
				if (storage.items.Any())
					return storage.items.FirstOrDefault(e => elementLocked ? e.HasTag(elementState) : true);
				else
					return null;
			}

			for (int i = 0; i < storage.items.Count; i++)
			{
				GameObject item = storage.items[i];
				if (elementLocked && !item.HasTag(elementState))
					continue;

				bool hasAnyTags = item.HasAnyTags(elementTags);
				if (hasAnyTags != invertedFilter)
				{
					return item;
				}
			}
			return null;
		}

		public void Sim200ms(float dt)
		{
			GameObject storedObject = GetFirstViableStoredItem();
			PrimaryElement primaryElement = null;
			float stored = 0f;
			if (storedObject != null && storedObject.TryGetComponent<PrimaryElement>(out primaryElement))
			{
				stored = primaryElement.Mass;
			}

			int outputCell = dispenser.UtilityCell;
			if (outputCell < 0)
				return;
			bool allowedToSpill = (dispenser == null || !dispenser.IsConnected);
			bool outputCellBlocked = Grid.Mass[outputCell] > OverpressureThreshold || Grid.Solid[outputCell];
			bool maxAmountSpilled = false;

			if (stored > 0f && allowedToSpill && !outputCellBlocked)
			{
				float dispenseAmount = Mathf.Min(emissionRate * dt, stored);

				if (dispenseAmount < stored && storedObject.TryGetComponent<Pickupable>(out var pickupable))
				{
					storedObject = pickupable.Take(dispenseAmount).gameObject;
					primaryElement = storedObject.GetComponent<PrimaryElement>();
					maxAmountSpilled = true;
				}

				Element element = primaryElement.Element;
				float temperature = primaryElement.Temperature;
				int disease = primaryElement.DiseaseCount;
				byte idx = primaryElement.DiseaseIdx;

				if (element.IsGas)
				{
					SimMessages.ReplaceAndDisplaceElement(outputCell, element.id, SpawnEvent, dispenseAmount, temperature, idx, disease);
				}
				else if (element.IsLiquid)
				{
					FallingWater.instance.AddParticle(outputCell, element.idx, dispenseAmount, temperature, idx, disease, true);
				}
				else
				{
					element.substance.SpawnResource(Grid.CellToPosCCC(outputCell, Grid.SceneLayer.Ore), dispenseAmount, temperature, idx, disease, true, false, false);
				}

				if (storage.items.Contains(storedObject))
					storage.ConsumeIgnoringDisease(storedObject);
				else
					storedObject.DeleteObject();

				stored -= dispenseAmount;
			}

			bool overfilled = stored >= capacity && !maxAmountSpilled;
			bool spillingBlocked = (allowedToSpill && outputCellBlocked);
			pipeBlockedGuid = selectable?.ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked, this.pipeBlockedGuid, spillingBlocked) ?? Guid.Empty;

			bool blocked = spillingBlocked || overfilled;

			this.operational.SetFlag(outputFlag, !blocked);
		}
	}
}

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
		public Tag elementTag;

		[SerializeField]
		public float capacity;

		[SerializeField]
		public Storage storage;

		[SerializeField]
		public float emissionRate = 25f;

		[SerializeField]
		public float OverpressureThreshold = float.MaxValue;

		[MyCmpGet] ComplexFabricator complexfab;

		private Operational.Flag outputFlag;

		[MyCmpReq]
		readonly private Operational operational;

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
				dispenser.SkipSetOperational = true;
			else
				SgtLogger.error("DISPENSER NULL ON: " + this.gameObject.name + " with tag: " + elementTag);

			string operationalFlag = "output_blocked_";
			if (elementTag != null)
				operationalFlag += elementTag.ToString();
			operationalFlag += "_" + capacity;
			operationalFlag += "_" + dispenser.conduitType.ToString();
			operationalFlag += "_" + dispenser.conduitOffset.ToString();


			outputFlag = new Operational.Flag(operationalFlag, Operational.Flag.Type.Functional);
		}

		public void Sim200ms(float dt)
		{
			GameObject storedObject = this.storage.FindFirst(elementTag);
			PrimaryElement primaryElement = null;
			float stored = 0f;
			if(storedObject != null && storedObject.TryGetComponent<PrimaryElement>(out primaryElement))
			{
				stored = primaryElement.Mass;
			}

			int outputCell = dispenser.UtilityCell;
			bool allowedToSpill = (dispenser == null || !dispenser.IsConnected) && Grid.Mass[outputCell] < OverpressureThreshold;

			if (stored > 0f && allowedToSpill)
			{
				float dispenseAmount = Mathf.Min(emissionRate * dt, stored);

				if (dispenseAmount < stored && storedObject.TryGetComponent<Pickupable>(out var pickupable))
				{
					storedObject = pickupable.Take(dispenseAmount).gameObject;
					primaryElement = storedObject.GetComponent<PrimaryElement>();
				}

				Element element = primaryElement.Element;
				float temperature = primaryElement.Temperature;
				int disease = primaryElement.DiseaseCount;
				byte idx = primaryElement.DiseaseIdx;

				if (element.IsGas
					//|| element.IsLiquid
					)
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
				storage.ConsumeIgnoringDisease(storedObject);
				stored -= dispenseAmount;
			}
			if (capacity <= 0)
				return;

			bool overfilled = stored >= capacity;
			this.operational.SetFlag(outputFlag, !overfilled
				|| allowedToSpill
				);
		}
	}
}

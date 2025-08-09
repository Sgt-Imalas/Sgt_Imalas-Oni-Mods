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
		[Serialize]
		internal PipedConduitDispenser dispenser;

		[SerializeField]
		[Serialize]
		public Tag elementTag;

		[SerializeField]
		[Serialize]
		public float capacity;

		[SerializeField]
		[Serialize]
		public Storage storage;

		[MyCmpGet] ComplexFabricator complexfab;

		private static readonly Operational.Flag outputFlag = new Operational.Flag("output_blocked", Operational.Flag.Type.Functional);

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
		}

		public void Sim200ms(float dt)
		{
			GameObject storedObject = this.storage.FindFirst(elementTag);
			PrimaryElement component = null;
			float stored = 0f;
			if (storedObject != null)
			{
				component = storedObject.GetComponent<PrimaryElement>();
				stored = component.Mass;
			}

			bool allowedToSpill = (dispenser != null || !dispenser.IsConnected);
			if (stored > 0f && allowedToSpill)
			{
				Element element = component.Element;
				float temperature = component.Temperature;
				int disease = component.DiseaseCount;
				byte idx = component.DiseaseIdx;

				int outputCell = dispenser.UtilityCell;

				if (element.IsGas || element.IsLiquid)
				{
					SimMessages.ReplaceAndDisplaceElement(outputCell, element.id, SpawnEvent, stored, temperature, idx, disease);
				}
				else if (element.IsLiquid)
				{
					FallingWater.instance.AddParticle(outputCell, element.idx, stored, temperature, idx, disease, true);
				}
				else
				{
					element.substance.SpawnResource(Grid.CellToPosCCC(outputCell, Grid.SceneLayer.Ore), stored, temperature, idx, disease, true, false, false);
				}
				storage.ConsumeIgnoringDisease(storedObject);
				stored = 0f;
			}
			bool overfilled = stored >= capacity && capacity > 0;

			this.operational.SetFlag(outputFlag, !overfilled);
		}
	}
}

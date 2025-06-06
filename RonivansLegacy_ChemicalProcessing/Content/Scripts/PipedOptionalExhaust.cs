using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class PipedOptionalExhaust : KMonoBehaviour, ISim200ms
	{
		[SerializeField]
		internal PipedConduitDispenser dispenser;

		[SerializeField]
		public Tag elementTag;

		[SerializeField]
		public float capacity;

		[MyCmpAdd]
		private Storage storage;

		private static readonly Operational.Flag outputFlag = new Operational.Flag("output_blocked", Operational.Flag.Type.Functional);

		[MyCmpReq]
		readonly private Operational operational;

		private static readonly CellElementEvent SpawnEvent = new(
	"Chemical_ExhaustSpawned",
	"Spawned by piped optional exhaust",
	true);
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

			if (stored > 0f && dispenser != null)
			{
				if (!dispenser.IsConnected)
				{
					Element element = component.Element;
					float temperature = component.Temperature;
					int disease = component.DiseaseCount;
					byte idx = component.DiseaseIdx;

					int outputCell = dispenser.UtilityCell;

					if (element.IsGas)
					{
						SimMessages.ReplaceAndDisplaceElement(outputCell, element.id, SpawnEvent, stored, temperature, idx, disease);
					}
					else if (element.IsLiquid)
					{
						int elementIndex = ElementLoader.GetElementIndex(element.id);
						FallingWater.instance.AddParticle(outputCell, (byte)elementIndex, stored, temperature, idx, disease, true, false, false, false);
					}
					else
					{
						element.substance.SpawnResource(Grid.CellToPosCCC(outputCell, Grid.SceneLayer.Front), stored, temperature, idx, disease, true, false, false);
					}
					storage.ConsumeIgnoringDisease(storedObject);
					stored = 0f;
				}


			}
			bool overfilled = stored >= capacity;

			this.operational.SetFlag(outputFlag, !overfilled);
		}
	}
}

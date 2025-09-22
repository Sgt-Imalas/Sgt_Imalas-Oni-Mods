using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{


	internal class ElementThresholdOperational : KMonoBehaviour, ISim1000ms
	{
		[SerializeField]
		public Tag ThresholdTag;
		[SerializeField]
		public float Threshold = 500;

		[MyCmpReq]
		Storage storage;
		[MyCmpReq]
		Operational operational;
		[MyCmpReq]
		KSelectable selectable;

		Guid StatusItemHandle = Guid.Empty;
		private Operational.Flag StorageFullFlag;

		public override void OnSpawn()
		{
			StorageFullFlag = new Operational.Flag("ElementThresholdOperational_StorageFull_"+ThresholdTag, Operational.Flag.Type.Requirement);
			UpdateThreshold();
		}
		public void Sim1000ms(float dt)
		{
			UpdateThreshold();
		}
		void UpdateThreshold()
		{
			float current = storage.GetAmountAvailable(ThresholdTag);
			bool isAboveThreshold = current > Threshold;

			if (isAboveThreshold && StatusItemHandle == Guid.Empty)
			{
				StatusItemHandle = selectable.AddStatusItem(StatusItemsDatabase.Converter_StorageFull,this);
			}
			else if (!isAboveThreshold && StatusItemHandle != Guid.Empty)
			{
				selectable.RemoveStatusItem(StatusItemHandle);
				StatusItemHandle = Guid.Empty;
			}
			operational.SetFlag(StorageFullFlag, !isAboveThreshold);
		}
	}
}

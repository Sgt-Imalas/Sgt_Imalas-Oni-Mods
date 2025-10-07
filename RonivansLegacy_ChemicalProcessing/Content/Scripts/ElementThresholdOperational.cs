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
		[SerializeField]
		public bool CreateMeter = false;

		[MyCmpReq]
		Storage storage;
		[MyCmpReq]
		Operational operational;
		[MyCmpReq]
		KSelectable selectable;

		private MeterController meter;
		Guid StatusItemHandle = Guid.Empty;
		private Operational.Flag StorageFullFlag;

		public override void OnSpawn()
		{
			StorageFullFlag = new Operational.Flag("ElementThresholdOperational_StorageFull_" + ThresholdTag, Operational.Flag.Type.Requirement);
			if (CreateMeter)
			{
				this.meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, null);
			}
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
				StatusItemHandle = selectable.AddStatusItem(StatusItemsDatabase.Converter_StorageFull, this);
			}
			else if (!isAboveThreshold && StatusItemHandle != Guid.Empty)
			{
				selectable.RemoveStatusItem(StatusItemHandle);
				StatusItemHandle = Guid.Empty;
			}
			operational.SetFlag(StorageFullFlag, !isAboveThreshold);
			if(CreateMeter && meter != null)
			{
				meter.SetPositionPercent(Mathf.Clamp01(current / Threshold));
			}
		}
	}
}

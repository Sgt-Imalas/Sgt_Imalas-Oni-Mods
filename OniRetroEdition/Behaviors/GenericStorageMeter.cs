using KSerialization;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.Behaviors
{
	internal class GenericStorageMeter : KMonoBehaviour
	{
		[MyCmpGet]
		KBatchedAnimController kbac;

		[MyCmpReq]
		public Storage storage;
		public bool inFront = true;
		private MeterController storageMeter;

		[Serialize] public string meterTarget = "target_meter";

		[Serialize] public float maxValueOverride = -1;

		public override void OnSpawn()
		{
			base.OnSpawn();


			SgtLogger.l("initializing meter, meter target: " + meterTarget);
			storageMeter = new MeterController(kbac, meterTarget, "meter", inFront ? Meter.Offset.Infront : Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[]
			{
				meterTarget
			});

			storage.Subscribe((int)GameHashes.OnStorageChange, UpdateMeter);

			UpdateMeter(null);
		}
		public override void OnCleanUp()
		{

			storage.Unsubscribe((int)GameHashes.OnStorageChange, UpdateMeter);

			base.OnCleanUp();
		}

		void UpdateMeter(object data)
		{
			float maxValue = maxValueOverride > 0 ? maxValueOverride : storage.capacityKg, currentValue = storage.MassStored();

			float newMeterValue = Mathf.Clamp(currentValue / maxValue, 0, 1f);
			//SgtLogger.l("Meter on " + this.GetProperName() + " changed, new value: " + newMeterValue * 100 + "%");
			storageMeter.SetPositionPercent(newMeterValue);

		}
	}
}

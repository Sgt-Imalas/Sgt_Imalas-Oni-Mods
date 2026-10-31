using KSerialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.Behaviors
{
	internal class StoragesMeter : KMonoBehaviour
	{
		[MyCmpGet]
		KBatchedAnimController kbac;

		private MeterController storageMeter;
		private List<Storage> storages;
		[SerializeField]
		public bool inFront = false;
		[Serialize]
		public float MaxMassOverride = -1;

		public override void OnSpawn()
		{
			base.OnSpawn();
			storages = this.GetComponents<Storage>().ToList();

			string meterTarget;
			if (kbac.curBuild.GetSymbol("target_meter") != null)
				meterTarget = "target_meter";
			else
				meterTarget = "meter_target";

			SgtLogger.l("initializing meter on " + gameObject.name + ", meter target: " + meterTarget + ", in front? " + inFront);
			storageMeter = new MeterController(kbac, meterTarget, "meter", inFront ? Meter.Offset.Infront : Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[]
			{
				meterTarget
			});

			foreach (Storage s in storages)
			{
				_handles[s] = s.Subscribe((int)GameHashes.OnStorageChange, UpdateMeter);
			}
			UpdateMeter(null);
		}
		Dictionary<Storage, int> _handles = [];
		public override void OnCleanUp()
		{
			foreach (Storage s in storages)
			{
				if (s != null && _handles.TryGetValue(s, out int handle))
				{
					s.Unsubscribe(handle);
				}
			}

			base.OnCleanUp();
		}

		void UpdateMeter(object data)
		{
			float maxValue = 0, currentValue = 0;
			foreach (Storage s in storages)
			{
				maxValue += s.capacityKg;
				currentValue += s.MassStored();
			}
			if (MaxMassOverride > 0)
				maxValue = MaxMassOverride;

			float newMeterValue = Mathf.Clamp(currentValue / maxValue, 0, 1f);
			//SgtLogger.l("Meter on " + this.GetProperName() + " changed, new value: " + newMeterValue * 100 + "%");
			storageMeter.SetPositionPercent(newMeterValue);

		}
	}
}

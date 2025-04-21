using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
    class IceKettle_FuelMeterController : KMonoBehaviour, ISim200ms
	{
		[MyCmpReq]
		KBatchedAnimController kbac;

		MeterController FuelMeter;
		Storage fuelStorage;

		public override void OnSpawn()
		{
			base.OnSpawn();
			fuelStorage=gameObject.GetComponents<Storage>()?[0];
			if(fuelStorage==null)
			{
				Debug.LogError("IceKettle_FuelMeterController: No Storage component found");
				return;
			}

			FuelMeter = new MeterController(kbac, "meter_target_wood", "meter_wood", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[1]
			{
				"meter_target"
			});
		}

		public void Sim200ms(float dt)
		{
			FuelMeter?.SetPositionPercent(fuelStorage.MassStored() / fuelStorage.Capacity());
		}
	}
}


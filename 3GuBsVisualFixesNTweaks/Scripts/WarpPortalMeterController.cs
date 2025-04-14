using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
    class WarpPortalMeterController:KMonoBehaviour, ISim200ms
    {
        [MyCmpReq]
        WarpPortal warpPortal;
		[MyCmpReq]
		KBatchedAnimController kbac;

		MeterController TimerMeter;

		public override void OnSpawn()
		{
			base.OnSpawn();

			TimerMeter = new MeterController(kbac, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
			{
				"meter_target"
			});
		}

		public void Sim200ms(float dt)
		{
			TimerMeter.SetPositionPercent(warpPortal.rechargeProgress / WarpPortal.RECHARGE_TIME);
		}
	}
}

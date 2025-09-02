using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OniRetroEdition.Behaviors
{
	internal class MeterBehindBattery : Battery
	{
		public override void OnSpawn()
		{
			Components.Batteries.Add(this);
			Building component = GetComponent<Building>();
			PowerCell = component.GetPowerInputCell();
			Subscribe(-1582839653, OnTagsChangedDelegate);
			OnTagsChanged(null);
			bool flag = GetComponent<PowerTransformer>();
			meter = (flag ? null : new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL"));
			Game.Instance.circuitManager.Connect(this);
			Game.Instance.energySim.AddBattery(this);
		}
	}
}

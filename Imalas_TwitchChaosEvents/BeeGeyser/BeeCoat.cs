using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.BeeGeyser
{
	internal class BeeCoat:KMonoBehaviour
	{
		public override void OnSpawn()
		{
			base.OnSpawn();
			var tempMonitor = gameObject.AddOrGetDef<CritterTemperatureMonitor.Def>();
			tempMonitor.temperatureColdDeadly = 50f;
			tempMonitor.temperatureHotDeadly = 500f;
		}
	}
}

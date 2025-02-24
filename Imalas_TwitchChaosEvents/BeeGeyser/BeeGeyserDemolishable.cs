using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.BeeGeyser
{
	internal class BeeGeyserDemolishable:Demolishable
	{
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			SetWorkTime(666f);
		}
	}
}

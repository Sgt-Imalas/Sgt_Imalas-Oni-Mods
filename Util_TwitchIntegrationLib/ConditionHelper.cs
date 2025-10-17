using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util_TwitchIntegrationLib
{
	public static class ConditionHelper
	{
		public static bool MinCycle(int cycle) => GameClock.Instance.GetCycle() >= cycle;

		public static bool MinDupeCount(int count) => Components.LiveMinionIdentities.Count > count;
	}
}

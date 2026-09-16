using ONITwitchLib;
using System.Collections.Generic;
using System.Linq;
using TwitchColony.Api;

namespace Util_TwitchIntegrationLib
{
	public static class ConditionHelper
	{
		public static bool MinCycle(int cycle) => GameClock.Instance.GetCycle() >= cycle;

		public static bool MinDupeCount(int count) => Components.LiveMinionIdentities.Count > count;

		public static bool AsquaredTwitchIntegration_Active() => TwitchModInfo.TwitchIsPresent;
		public static bool TwitchColony_Active() => TwitchColony.Api.TwitchColonyApi.IsAvailable;

		public static bool FindAnyEligibleEvent(IEnumerable<string> eventIds)
		{
			if (AsquaredTwitchIntegration_Active())
			{
				List<EventInfo> weatherEvents = new List<EventInfo>();
				foreach (var e in eventIds)
				{
					if (EventRegistration.TryGetEvent_Asquared(e, out var eventInfo) && eventInfo.CheckCondition(null))
					{
						weatherEvents.Add(eventInfo);
					}
				}
				return weatherEvents.Any();
			}
			if (TwitchColony_Active())
			{
				List<EventDataInfo> weatherEvents = new List<EventDataInfo>();
				foreach (var e in eventIds)
				{
					if (EventRegistration.TryGetEvent_TwitchColony(e, out var eventInfo) && eventInfo.Eligible)
					{
						weatherEvents.Add(eventInfo);
					}
				}
				return weatherEvents.Any();
			}
			return false;
		}
	}
}

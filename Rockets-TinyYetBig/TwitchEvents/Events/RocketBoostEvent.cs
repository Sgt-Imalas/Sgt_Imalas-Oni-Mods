using ONITwitchLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using Util_TwitchIntegrationLib;

namespace Rockets_TinyYetBig.TwitchEvents.Events
{
	public class RocketBoostEvent : ITwitchEventBase
	{
		public string ID => "RTB_TwitchEvent_RocketBoost";
		public string EventName => "Rocket Boost";

		public Danger EventDanger => Danger.None;
		public string EventDescription => "{0} has recieved a boost!";
		public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;
		public Func<object, bool> Condition =>
				(data) =>
				{
					if (SpaceStationManager.GetRockets(out var rockets))
					{
						foreach (Clustercraft craft in rockets)
							if (craft.Status == Clustercraft.CraftStatus.InFlight && craft.controlStationBuffTimeRemaining <= 0)
								return true;
					}

					return false;
				};
		public Action<object> EventAction =>
			(data) =>
			{
				SpaceStationManager.GetRockets(out var rockets);
				rockets.ShuffleList();
				foreach (Clustercraft craft in rockets)
					if (craft.Status == Clustercraft.CraftStatus.InFlight && craft.controlStationBuffTimeRemaining <= 0)
					{
						craft.controlStationBuffTimeRemaining = (float)new System.Random().Next(600, 1200);

						ToastManager.InstantiateToast(EventName, string.Format(EventDescription, craft.Name));
						break;
					}
			};

		public string EventGroupID => "RTB_RocketBoostGroup";
	}
}

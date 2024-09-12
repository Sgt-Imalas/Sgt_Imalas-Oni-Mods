using ONITwitchLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using Util_TwitchIntegrationLib;

namespace Rockets_TinyYetBig.TwitchEvents.Events
{
	public class RocketBoostEventAll : ITwitchEventBase
	{
		public string ID => "RTB_TwitchEvent_RocketBoostAll";
		public string EventName => "Rocket Boost";

		public Danger EventDanger => Danger.None;
		public string EventDescription => "All flying rockets have recieved a boost!";
		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;
		public Func<object, bool> Condition
			=>
				data =>
				{
					if (SpaceStationManager.GetRockets(out var rockets))
					{
						foreach (Clustercraft craft in rockets)
							if (craft.Status == Clustercraft.CraftStatus.InFlight && craft.controlStationBuffTimeRemaining <= 0)
								return true;
					}

					return false;
				};
		public Action<object> EventAction => (
			data =>
			{
				SpaceStationManager.GetRockets(out var rockets);
				int counter = 0;
				foreach (Clustercraft craft in rockets)
					if (craft.Status == Clustercraft.CraftStatus.InFlight && craft.controlStationBuffTimeRemaining <= 0)
					{
						counter++;
						craft.controlStationBuffTimeRemaining = (float)new System.Random().Next(600, 1200);
					}
				if (counter > 0)
				{
					ToastManager.InstantiateToast(EventName, EventDescription);
				}
			});

		public string EventGroupID => "RTB_RocketBoostGroup";
	}
}

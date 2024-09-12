using ONITwitchLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using Util_TwitchIntegrationLib;

namespace Rockets_TinyYetBig.TwitchEvents.Events
{
	public class BlowUpRocketEvent : ITwitchEventBase
	{
		public string ID => "RTB_TwitchEvent_RocketSelfDestruct";
		public string EventName => "Unplanned Rocket Disassembly";

		public Danger EventDanger => Danger.Deadly;
		public string EventDescription => "wrong button Meep, WRONG BUTTON!!!\n\n {0} has self destructed.";
		public EventWeight EventWeight => EventWeight.WEIGHT_ALMOST_NEVER;
		public Func<object, bool> Condition =>
				(data) =>
				{
					if (SpaceStationManager.GetRockets(out var rockets))
					{
						foreach (Clustercraft craft in rockets)
							if (craft.Status == Clustercraft.CraftStatus.InFlight)
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
					if (craft.Status == Clustercraft.CraftStatus.InFlight)
					{
						craft.ModuleInterface.gameObject.Trigger(-1061799784);
						ToastManager.InstantiateToast(EventName, string.Format(EventDescription, craft.Name));
						break;
					}
			};

		public string EventGroupID => null;

	}
}

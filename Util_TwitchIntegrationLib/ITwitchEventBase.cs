using ONITwitchLib;
using System;

namespace Util_TwitchIntegrationLib
{
	public interface ITwitchEventBase
	{
		string ID { get; }
		string EventGroupID { get; }
		string EventName { get; }
		//string EventDescription { get; }
		EventWeight EventWeight { get; }
		Action<object> EventAction { get; }
		Func<object, bool> Condition { get; }
		Danger EventDanger { get; }

	}
	public enum EventWeight
	{
		WEIGHT_NEVER = 0,
		WEIGHT_ALMOST_NEVER = 1,
		WEIGHT_VERY_RARE = 3,
		WEIGHT_RARE = 7,
		WEIGHT_NORMAL = 14,
		WEIGHT_COMMON = 30
	}
}

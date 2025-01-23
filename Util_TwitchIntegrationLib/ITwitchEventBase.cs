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
		WEIGHT_NEVER = Consts.EventWeight.Never,
		WEIGHT_ALMOST_NEVER = Consts.EventWeight.AlmostNever,
		WEIGHT_VERY_RARE = Consts.EventWeight.VeryRare,
		WEIGHT_RARE = Consts.EventWeight.Rare,
		WEIGHT_UNCOMMON = Consts.EventWeight.Uncommon,
		WEIGHT_COMMON = Consts.EventWeight.Common,
		WEIGHT_FREQUENT = Consts.EventWeight.Frequent
	}
}

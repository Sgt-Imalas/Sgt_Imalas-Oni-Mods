//using ONITwitchLib;
//using System;
//using UnityEngine;
//using Util_TwitchIntegrationLib;

//namespace Imalas_TwitchChaosEvents.Events
//{
//	/// <summary>
//	/// Evil Tree event from aki twitchery, but it contains seeds that, when uncovered, are a time bomb to also spawn a very evil tree
//	/// </summary>
//	internal class VeryEvilTreeEvent : ITwitchEventBase
//	{
//		public string ID => "ITCE_VeryEvilTree";

//		public string EventGroupID => null;

//		public string EventName => STRINGS.CHAOSEVENTS.VERYEVILTREE.NAME;

//		public EventWeight EventWeight => EventWeight.WEIGHT_UNCOMMON;

//		public Action<object> EventAction => (_) =>
//		{
//			var go = new GameObject("tree spawner");
//			var tree = go.AddOrGet<TreeSpawner>();
//			tree.minDistance = 50;
//			go.SetActive(true);
//		};

//		public Func<object, bool> Condition => (_) => Config.Instance.SkipMinCycle || (GameClock.Instance.GetCycle() > 10);

//		public Danger EventDanger => Danger.Extreme;
//	}
//}

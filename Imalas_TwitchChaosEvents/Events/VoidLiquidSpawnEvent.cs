using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using System;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// void liquid from noita
	/// </summary>
	internal class VoidLiquidSpawnEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_StaredIntoTheVoid";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.NAME;

		//public EventWeight EventWeight => EventWeight.WEIGHT_VERY_RARE;
		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;

		public Action<object> EventAction => (object data) =>
		{
			var currentPlanet = ClusterManager.Instance.activeWorld;

			var randomMinion = Components.LiveMinionIdentities.GetWorldItems(currentPlanet.id).GetRandom();

			var voidSpawner = randomMinion.gameObject.AddComponent<VoidLiquidSpawner>();

			SpeedControlScreen.Instance.SetSpeed(0);
			string name = randomMinion.GetProperName();
			SoundUtils.PlaySound(ModAssets.SOUNDS.CAVE_NOISE, SoundUtils.GetSFXVolume() * 1.0f, true);

			ToastManager.InstantiateToastWithPosTarget(
				STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOAST,
				string.Format(STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOASTTEXT, name),
				randomMinion.transform.position);


			RandomTickManager.Instance.StartVoidEvent(randomMinion);

			GameScheduler.Instance.Schedule("VoidDefeat", 600f, _ =>
			{
				RandomTickManager.Instance.AdmitVoidDefeat();
			});
		};

		public Func<object, bool> Condition =>
			(data) =>
			{
				return Config.Instance.SkipMinCycle || GameClock.Instance.GetCycle() > 150;
			};

		public Danger EventDanger => Danger.Deadly;
	}
}

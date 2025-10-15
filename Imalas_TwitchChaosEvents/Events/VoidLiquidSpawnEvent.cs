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


			randomMinion.TryGetComponent<KSelectable>(out var selectable);
			selectable.AddStatusItem(ModAssets.StatusItems.VoidTarget);

			SpeedControlScreen.Instance.SetSpeed(0);
			string name = randomMinion.GetProperName();
			SoundUtils.PlaySound(ModAssets.SOUNDS.CAVE_NOISE, SoundUtils.GetSFXVolume() * 0.3f, true);

			ToastManager.InstantiateToastWithGoTarget(
				STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOAST,
				string.Format(STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOASTTEXT, name),
				randomMinion.gameObject);

			GameScheduler.Instance.Schedule("VoidAwoken", 100f, _ =>
			{
				var voidSpawner = randomMinion.gameObject.AddComponent<VoidLiquidSpawner>();

				RandomTickManager.Instance.StartVoidEvent(randomMinion);

				ToastManager.InstantiateToastWithGoTarget(
					STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOAST,
					string.Format(STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOASTTEXT2, name),
					randomMinion.gameObject);

				SoundUtils.PlaySound(ModAssets.SOUNDS.CAVE_NOISE, SoundUtils.GetSFXVolume() * 0.7f, true);
			});


			GameScheduler.Instance.Schedule("VoidDefeat", 700f, _ =>
			{
				selectable.RemoveStatusItem(ModAssets.StatusItems.VoidTarget);
				RandomTickManager.Instance.AdmitVoidDefeat();
			});
		};

		public Func<object, bool> Condition =>
			(data) =>
			{
				return (Config.Instance.SkipMinCycle || ConditionHelper.MinCycle(150)) && ConditionHelper.MinDupeCount(10);
			};

		public Danger EventDanger => Danger.Deadly;
	}
}

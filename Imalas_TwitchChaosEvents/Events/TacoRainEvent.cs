using ONITwitchLib;
using System;
using UnityEngine;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// Taco Meteor shower
	/// </summary>
	internal class TacoRainEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_TacoRain";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.TACORAIN.NAME;

		public string EventDescription => STRINGS.CHAOSEVENTS.TACORAIN.TOASTTEXT;

		public EventWeight EventWeight => EventWeight.WEIGHT_FREQUENT;

		public Action<object> EventAction => (object data) =>
		{
			string body = STRINGS.CHAOSEVENTS.TACORAIN.TOASTTEXT;

			if (ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe == false)
			{
				body += STRINGS.CHAOSEVENTS.TACORAIN.NEWRECIPE;
				ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe = true;
			}

			ChaosTwitch_SaveGameStorage.Instance.lastTacoRain = GameClock.Instance.GetTimeInCycles();


			int activeWorld = ClusterManager.Instance.activeWorldId;
			//rain.StartRaining();
			if (ClusterManager.Instance.activeWorld.IsModuleInterior)
			{
				if (ClusterManager.Instance.activeWorld.ParentWorldId == ClusterManager.Instance.activeWorld.id)
					activeWorld = 0;
				else
					activeWorld = ClusterManager.Instance.activeWorld.ParentWorldId;
			}

			var world = ClusterManager.Instance.GetWorld(activeWorld);
			if (!world.isSurfaceRevealed)
			{
				foreach (var planet in ClusterManager.Instance.WorldContainers)
				{
					if ((planet.IsDupeVisited || planet.IsStartWorld) && planet.IsSurfaceRevealed && !planet.IsModuleInterior)
					{
						world = planet;
						break;
					}
				}
			}



			SpeedControlScreen.Instance.SetSpeed(0);
			GameplayEventInstance eventInstance = GameplayEventManager.Instance.StartNewEvent(TacoMeteorPatches.ITC_TacoMeteors, world.id);
			// ClusterManager.Instance.activeWorld.GetSMI<GameplaySeasonManager.Instance>().Start(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
			if (Config.Instance.TacoEventMusic)
				SoundUtils.PlaySound(ModAssets.SOUNDS.TACORAIN, SoundUtils.GetSFXVolume() * 0.3f, true);

			//var pos = world.LookAtSurface();


			ToastManager.InstantiateToastWithPosTarget(
			STRINGS.CHAOSEVENTS.TACORAIN.TOAST,
			 body, GetSurfacePos(world), 80);
		};

		Vector3 GetSurfacePos(WorldContainer world)
		{
			Vector3 vector = new Vector3(world.WorldOffset.x + (world.Width / 2), world.WorldOffset.y + (world.Height - 15), 0f);
			return vector;
		}


		public Func<object, bool> Condition =>
			(data) =>
			{
				bool anyUnlockedPlanetRevealed = false;

				foreach (var planet in ClusterManager.Instance.WorldContainers)
				{
					if ((planet.IsDupeVisited || planet.IsStartWorld) && planet.IsSurfaceRevealed && !planet.IsModuleInterior)
					{
						anyUnlockedPlanetRevealed = true;
						break;
					}
				}

				if (!anyUnlockedPlanetRevealed)
					return false;


				return
				Config.Instance.SkipMinCycle
				||
				(GameClock.Instance.GetCycle() > 50 && !ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe)
				||
				(ChaosTwitch_SaveGameStorage.Instance.lastTacoRain + 75f > GameClock.Instance.GetTimeInCycles())
				;
			};

		public Danger EventDanger => Danger.None;
	}
}

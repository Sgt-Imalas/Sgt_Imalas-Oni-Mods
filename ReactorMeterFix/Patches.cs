using FMOD;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Core;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TemplateClasses;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UtilLibs;

namespace TinyFixes
{
	internal class Patches
	{      
		/// <summary>
		/// fix moonlet absense crashing saving
		/// </summary>
		[HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.GetSettingsForMixingMetrics))]
		public static class CustomGameSettings_GetSettingsForMixingMetrics_Patch
		{
			public static void Prefix(CustomGameSettings __instance)
			{
				var currentStorySetting = __instance.currentStoryLevelsBySetting.Keys.ToList();

				foreach(var story in currentStorySetting)
				{
					if (!__instance.StorySettings.ContainsKey(story))
					{
						__instance.StorySettings.Remove(story);
					}
				}
				var currentMixingSettings = __instance.CurrentMixingLevelsBySetting.Keys.ToList();

				foreach (var mixing in currentMixingSettings)
				{
					if (!__instance.MixingSettings.ContainsKey(mixing))
					{
						__instance.CurrentMixingLevelsBySetting.Remove(mixing);
					}
				}
			}
		}
		/// <summary>
		/// Fix the reactor meter by removing that obsolete frame scale hack thing from an earlier reactor implementation
		/// </summary>
		[HarmonyPatch(typeof(Reactor), nameof(Reactor.OnSpawn))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Prefix()
			{
				Reactor.meterFrameScaleHack = 1;
			}
		}

		/// <summary>
		/// fix that check to actually check for immunities instead of hardcoding for the WarmTouch effect (which breaks the effect for WarmTouchFood)
		/// </summary>
		[HarmonyPatch(typeof(ColdImmunityMonitor), nameof(ColdImmunityMonitor.HasImmunityEffect))]
		public static class ColdImmunityMonitor_HasImmunityEffect
		{
			static Effect ColdAir;
			public static void Postfix(ref bool __result, ColdImmunityMonitor.Instance smi)
			{
				if (__result)
					return;
				if (ColdAir == null)
					ColdAir = Db.Get().effects.Get("ColdAir");

				var effects = smi.GetComponent<Effects>();
				if (effects.HasImmunityTo(ColdAir))
					__result = true;
			}
		}
		/// <summary>
		/// add proper cold air effect immunity to WarmTouch and WarmTouchFood so the tooltips actually reflect that
		/// </summary>
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				Effect frostImmunityEffect = Db.Get().effects.Get("WarmTouch");
				frostImmunityEffect.immunityEffectsNames = frostImmunityEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();

				Effect frostImmunityFoodEffect = Db.Get().effects.Get("WarmTouchFood");
				frostImmunityFoodEffect.immunityEffectsNames = frostImmunityFoodEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();
			}
		}
		///BalloonstandSensor
		///
		[HarmonyPatch(typeof(BalloonStandCellSensor), nameof(BalloonStandCellSensor.Update))]
		public class BalloonStandCellSensor_Update_Patch
		{
			//skip this method re-checking every single tick
			public static bool Prefix(BalloonStandCellSensor __instance)
			{				
				return false;
			}
		}

    //    [HarmonyPatch(typeof(LoadingOverlay), nameof(LoadingOverlay.Load))]
    //    public class Overlay_Icon_Replace
    //    {
    //        //replace loading dupe face with pip
    //        public static void Postfix()
    //        {
				//var instance = LoadingOverlay.instance;
				//var image = instance.transform.Find("Image").GetComponent<Image>();
				//var pipSprite = Def.GetUISprite(Assets.GetPrefab(SquirrelConfig.ID));
    //            image.preserveAspect = true;
    //            image.sprite = pipSprite.first;

				//var rect = image.rectTransform();
				//rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
    //        }
    //    }

        [HarmonyPatch(typeof(BalloonStandCellSensor), nameof(BalloonStandCellSensor.GetCell))]
		public class BalloonStandCellSensor_GetCell_Patch
		{
			//only run the query when requested
			public static void Prefix(BalloonStandCellSensor __instance)
			{
				UpdateBalloonBuddyCells(__instance);
			}
		}
		[HarmonyPatch(typeof(BalloonStandCellSensor), nameof(BalloonStandCellSensor.GetStandCell))]
		public class BalloonStandCellSensor_GetStandCell_Patch
		{
			//only run the query when requested
			public static void Prefix(BalloonStandCellSensor __instance)
			{
				UpdateBalloonBuddyCells(__instance);
			}
		}
		[HarmonyPatch(typeof(OxygenBreather), nameof(OxygenBreather.OnSpawn))]
		public class OverjoyedTrigger
		{
			public static void Postfix(OxygenBreather __instance)
			{
				__instance.Subscribe((int)GameHashes.RefreshUserMenu, (_)=>OnRefreshUserMenu(__instance));
			}
			private static void OnRefreshUserMenu(OxygenBreather obj)
			{
				var smi = obj.GetSMI<JoyBehaviourMonitor.Instance>();

				if (smi != null) 
				{
					var button = new KIconButtonMenu.ButtonInfo("crew_state_happy", "trigger joy",  smi.GoToOverjoyed, is_interactable: true);
					Game.Instance.userMenu.AddButton(obj.gameObject, button);
				}
			}
		}

		public class LastFoundCell
		{
			public int cell;
			public int standCell;
            public int time;
			public LastFoundCell(int _cell, int _standCell, int _time)
			{
				this.cell = _cell;
				this.standCell = _standCell;
				this.time = _time;
			}
			public void UpdateCached(int _cell, int _standCell, int _time)
            {
                this.cell = _cell;
                this.standCell = _standCell;
                this.time = _time;
            }
        }
		public static LastFoundCell AddOrRefreshLastFoundCell(BalloonStandCellSensor instance)
		{
			int _time = GameClock.Instance.GetFrame();
			if (CachedValues.TryGetValue(instance, out var cache))
			{
				cache.UpdateCached(instance.cell, instance.standCell, _time);
				return cache;
            }
			CachedValues[instance] = new(instance.cell, instance.standCell, _time);
			return CachedValues[instance];
		}


        static Dictionary<BalloonStandCellSensor, LastFoundCell> CachedValues = new();
		public static void UpdateBalloonBuddyCells(BalloonStandCellSensor __instance)
		{
			//for efficiency, use caching of the found value and only refresh it at max. every N frames if the method is called in rapid succession; otherwise use the cached value
			if(CachedValues.TryGetValue(__instance, out var CachedForInstance))
			{
				int currentFrame = GameClock.Instance.GetFrame();
				//the following value determines the min. number of frames it reuses the cached value without reevaluating
				int minTimeUseCached = 20;

				if(CachedForInstance.time + minTimeUseCached > currentFrame)
				{
					__instance.standCell = CachedForInstance.standCell;
					__instance.cell = CachedForInstance.cell;
                    return;
				}
			}


			int worldId = __instance.gameObject.GetMyWorldId();

			__instance.cell = Grid.InvalidCell;
			__instance.standCell = Grid.InvalidCell;
			var roomProber = Game.Instance.roomProber;

			var brain = __instance.brain;
			var navigator = __instance.navigator;
			var navTable = navigator.NavGrid.NavTable;
			int currentMinimumNavCost = int.MaxValue;

			int maxNavCost = 4000; //game use 50, which is 5 tiles...

			int potentialStandCellLeft = Grid.InvalidCell;
			int potentialStandCellRight = Grid.InvalidCell;


			void PickRandomDirectionCell()
			{
				//at least one of these is supposed to be a valid cell
				if (potentialStandCellLeft == Grid.InvalidCell && potentialStandCellRight != Grid.InvalidCell)
				{
					__instance.standCell = potentialStandCellRight;
				}
				else if (potentialStandCellLeft != Grid.InvalidCell && potentialStandCellRight == Grid.InvalidCell)
				{
					__instance.standCell = potentialStandCellLeft;
				}
				else
				{
					if (new System.Random().Next(0, 2) != 0)
					{
						__instance.standCell = potentialStandCellLeft;
					}
					else
					{
						__instance.standCell = potentialStandCellRight;
					}
				}
                AddOrRefreshLastFoundCell(__instance);

            }
			void CheckCell(int mingleCell)
			{
				int navigationCost = navigator.GetNavigationCost(mingleCell);
				//finding the closest viable spot
				if (navigationCost == -1 || navigationCost > currentMinimumNavCost)
					return;

				//if cell withing nav cost bounds
				if (navigationCost < maxNavCost)
				{

					int betweenCellRight = Grid.CellRight(mingleCell);
					int ballonStandRight = Grid.CellRight(betweenCellRight);
					int betweenCellLeft = Grid.CellLeft(mingleCell);
					int balloonStandLeft = Grid.CellLeft(betweenCellLeft);

					CavityInfo ballonGiverLocationCavityInfo = roomProber.GetCavityForCell(mingleCell);
					CavityInfo ballonStandCavityInfoLeft = roomProber.GetCavityForCell(balloonStandLeft);
					CavityInfo balloonStandCavityInfoRight = roomProber.GetCavityForCell(ballonStandRight);
					if (ballonGiverLocationCavityInfo != null)
					{

						//if at least one of the two directions is valid, this is the new minimum travel cost. Check both to potentially flip the output.
						if (balloonStandCavityInfoRight != null && balloonStandCavityInfoRight.handle == ballonGiverLocationCavityInfo.handle
							&& navTable.IsValid(betweenCellRight) && navTable.IsValid(ballonStandRight))
						{
							//SgtLogger.l("cell to the right was valid!");

							__instance.cell = mingleCell; //new minimum found
							potentialStandCellRight = ballonStandRight;
							currentMinimumNavCost = navigationCost;
						}
						if (ballonStandCavityInfoLeft != null && ballonStandCavityInfoLeft.handle == ballonGiverLocationCavityInfo.handle
							&& navTable.IsValid(betweenCellLeft) && navTable.IsValid(balloonStandLeft))
						{
							//SgtLogger.l("cell to the left was valid!");

							__instance.cell = mingleCell; //new minimum found
							potentialStandCellLeft = balloonStandLeft;
							currentMinimumNavCost = navigationCost;
						}
					}
				}
			}

			//finding closest viable stand cell in mingle cells
			foreach (int mingleCell in Game.Instance.mingleCellTracker.mingleCells)
			{
				if (brain.IsCellClear(mingleCell))
				{
					CheckCell(mingleCell);
				}
			}
			//query for mingle cells was successful
			if(potentialStandCellLeft != Grid.InvalidCell ||potentialStandCellRight != Grid.InvalidCell)
			{
				PickRandomDirectionCell();
				return;
			}
			SgtLogger.l("reverting to gathering points...");
			//no viable stand locations were found, using printing pods and watercoolers as backup
			if (Components.SocialGatheringPoints.CountWorldItems(worldId) > 0)
			{
				maxNavCost = int.MaxValue; //fallback, dont care about max range anymore, also there are way less items here
				currentMinimumNavCost = int.MaxValue;

				var gatheringPoints = Components.SocialGatheringPoints.GetItems(worldId);

				foreach (var item in gatheringPoints)
				{
					int cell = Grid.PosToCell(item);
					CheckCell(cell);
				}
				//valid fallback location found
				if (potentialStandCellLeft != Grid.InvalidCell || potentialStandCellRight != Grid.InvalidCell)
				{
					PickRandomDirectionCell();
					return;
				}
			}

			//if still nothing found yet... check current position validitiy (prevent dupe freezing in place when one of the two directions is valid at least..)

			CheckCell(Grid.PosToCell(__instance.brain));
			PickRandomDirectionCell();
		}
	}
}

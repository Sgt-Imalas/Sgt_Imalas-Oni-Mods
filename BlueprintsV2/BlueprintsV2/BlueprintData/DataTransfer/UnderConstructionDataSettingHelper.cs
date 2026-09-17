using BlueprintsV2.BlueprintData;
using BlueprintsV2.ModAPI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
	public static class UnderConstructionDataSettingHelper
	{
		public static List<string> ComponentsToIgnore =
			[
			nameof(BuildingEnabledButton)
			//,nameof(Prioritizable)
			//,API_Consts.ConduitFlagID

			];

		static GameObject temporaryTargetBuildingGO;
		static UnderConstructionDataTransfer lastSelected;
		static SchedulerHandle? uiDelayHandle, fillUpHandle;
		static HashSet<int> _stolenCells = [];
		public static bool Pending => uiDelayHandle.HasValue;
		public static KSelectable TemporarySelectable { get; private set; }

		public static bool HasDataTransferComponents(BuildingUnderConstruction building)
		{
			if (building == null)
				return false;
			if (!API_Methods.AllowedByRules(building.Def))
				return false;
			var completeVersion = building.Def.BuildingComplete;
			if (completeVersion.HasTag(API_Consts.SkipPreconfiguration))
				return false;

			try
			{
				var data = API_Methods.GetAdditionalBuildingData(completeVersion);
				foreach (var entry in ComponentsToIgnore)
					data.Remove(entry);
				SgtLogger.l("Checking if " + completeVersion.name + " has data transfer components, found " + data.Count + " entries:");
				foreach (var entry in data)
				{
					SgtLogger.l(entry.Key);
				}

				return data.Any();
			}
			catch
			{
				return false;
			}
;
		}
		public static void StartEditingUnderConstructionData(UnderConstructionDataTransfer origin)
		{
			lastSelected = origin;
			var def = origin.building.Def;
			if (uiDelayHandle.HasValue)
			{
				uiDelayHandle.Value.ClearScheduler();
				uiDelayHandle = null;
			}
			if (temporaryTargetBuildingGO != null)
			{
				CleanUpTemporaryBuilding();
			}

			var world = origin.GetMyWorld();
			var worldOffset = world.WorldOffset;

			int cell = Grid.XYToCell(worldOffset.X, worldOffset.Y + 2);
			//spawn it close to the origin, but dont let it clip into negative cell indicies
			cell += Mathf.CeilToInt((def.WidthInCells / 2f));


			HashSet<CellOffset> willConsume = [];
			var solidDef = def.BuildingComplete.GetDef<MakeBaseSolid.Def>();
			if (solidDef != null)
			{
				foreach (var cellOffset in solidDef.solidOffsets)
					willConsume.Add(cellOffset);
			}
			if (def.BuildingComplete.TryGetComponent<Door>(out _) || def.BuildingComplete.TryGetComponent<SimCellOccupier>(out _))
			{
				foreach (var cellOffset in def.PlacementOffsets)
					willConsume.Add(cellOffset);
			}
			if (willConsume.Count > 0)
			{
				var neutroniumIdx = ElementLoader.GetElementIndex(SimHashes.Unobtanium);
				foreach (var offset in willConsume)
				{
					int buildingCell = Grid.OffsetCell(cell, offset);

					if (Grid.IsValidCell(buildingCell) && Grid.ElementIdx[buildingCell] == neutroniumIdx)
						_stolenCells.Add(buildingCell);
				}
			}

			temporaryTargetBuildingGO = def.Create(Grid.CellToPos(cell), null, [SimHashes.Unobtanium.CreateTag()], null, UtilMethods.GetKelvinFromC(20), def.BuildingComplete);
			temporaryTargetBuildingGO.GetComponent<DataTransferCleanup>().CleanupAfterUse();
			TemporarySelectable = temporaryTargetBuildingGO.GetComponent<KSelectable>();
			//prevent "build outside start biome" achievment from triggering
			temporaryTargetBuildingGO.GetComponent<KPrefabID>().AddTag(GameTags.TemplateBuilding);

			if (temporaryTargetBuildingGO.TryGetComponent<KBatchedAnimController>(out var kbac))
				kbac.animScale = 0;

			//hide deconstruction button
			if (temporaryTargetBuildingGO.TryGetComponent<Deconstructable>(out var decon))
				decon.allowDeconstruction = false;

			int paused = SpeedControlScreen.Instance.pauseCount;
			if (paused > 0)
			{
				SpeedControlScreen.Instance.pauseCount = 1;
				SpeedControlScreen.Instance.Unpause(false);
			}

			//1 frame delay to properly load the extra buttons on the menu screen
			uiDelayHandle = GameScheduler.Instance.ScheduleNextFrame("frame delay", (_) =>
			{
				if (paused > 0)
				{
					SpeedControlScreen.Instance.pauseCount = 0;
					SpeedControlScreen.Instance.Pause(false);
					SpeedControlScreen.Instance.pauseCount = paused;
				}
				if (temporaryTargetBuildingGO != null)
				{
					UnderConstructionDataTransfer.TransferDataTo(temporaryTargetBuildingGO, origin.GetStoredData());
					Game.Instance.Trigger((int)GameHashes.SelectObject, temporaryTargetBuildingGO);
				}
				uiDelayHandle = null;
			});
		}
		public static void CollectDataFrom(DataTransferCleanup dataContainer)
		{
			if (lastSelected != null && lastSelected.gameObject != null)
			{
				var buildingSettingData = API_Methods.GetAdditionalBuildingData(dataContainer.gameObject);
				foreach (var entries in buildingSettingData)
					lastSelected.SetDataToApply(entries.Key, entries.Value);
			}
			CleanUpTemporaryBuilding();
		}

		internal static void CleanUpTemporaryBuilding()
		{
			if (temporaryTargetBuildingGO != null)
				UnityEngine.Object.Destroy(temporaryTargetBuildingGO);
			TemporarySelectable = null;
			temporaryTargetBuildingGO = null;

			if (_stolenCells.Count > 0 && uiDelayHandle == null)
			{
				uiDelayHandle = GameScheduler.Instance.ScheduleNextFrame("fill up area", (_) =>
				{
					foreach (int cell in _stolenCells)
					{
						SimMessages.ReplaceElement(cell, SimHashes.Unobtanium, CellEventLogger.Instance.DebugTool, 20_000);
					}
					_stolenCells.Clear();
					uiDelayHandle = null;
				});
			}
		}


		/// <summary>
		/// turn off status items on the temp building
		/// </summary>
		[HarmonyPatch(typeof(KSelectable), nameof(KSelectable.AddStatusItem))]
		public class KSelectable_AddStatusItem_Patch
		{
			public static bool Prefix(KSelectable __instance, ref Guid __result)
			{
				if (TemporarySelectable != __instance)
					return true;

				__result = Guid.Empty;
				return false;
			}
		}
		/// <summary>
		/// override the prioritizable check to allow the temporary target building to be prioritized.
		/// </summary>
		[HarmonyPatch(typeof(Prioritizable), nameof(Prioritizable.IsPrioritizable))]
		public class Prioritizable_IsPrioritizable_Patch
		{
			public static bool Prefix(Prioritizable __instance, ref bool __result)
			{
				if (__instance.gameObject == temporaryTargetBuildingGO)
				{
					__result = true;
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Prevents the ComplexFabricatorSideScreen from showing the temporary target building as a valid target since recipes arent configurable and it crashes the soldering station.
		/// </summary>
		[HarmonyPatch(typeof(ComplexFabricatorSideScreen), nameof(ComplexFabricatorSideScreen.IsValidForTarget))]
		public class ComplexFabricatorSideScreen_IsValidForTarget_Patch
		{
			public static bool Prefix(GameObject target) => target != temporaryTargetBuildingGO;
		}
		/// <summary>
		/// dont show copy setting button on temp building
		/// </summary>
		[HarmonyPatch(typeof(CopyBuildingSettings), nameof(CopyBuildingSettings.OnRefreshUserMenu))]
		public class CopyBuildingSettings_OnRefreshUserMenu_Patch
		{
			public static bool Prefix(CopyBuildingSettings __instance) => __instance.gameObject != temporaryTargetBuildingGO;
		}

		/// <summary>
		/// dont show temp building in hover cards
		/// </summary>
		[HarmonyPatch(typeof(SelectToolHoverTextCard), nameof(SelectToolHoverTextCard.UpdateHoverElements))]
		public class SelectToolHoverTextCard_UpdateHoverElements_Patch
		{
			public static void Prefix(List<KSelectable> hoverObjects)
			{
				if (TemporarySelectable != null)
					hoverObjects.Remove(TemporarySelectable);
			}
		}

		/// <summary>
		/// Prevents the ModularConduitPortTiler from updating endcaps if the visualizers are not initialized as that would crash the game.
		/// this happens if 2 or more launchpads are placed via blueprint
		/// </summary>
		[HarmonyPatch(typeof(ModularConduitPortTiler), nameof(ModularConduitPortTiler.UpdateEndCaps))]
		public class ModularConduitPortTiler_UpdateEndCaps_Patch
		{
			public static bool Prefix(ModularConduitPortTiler __instance)
			{
				bool hasVisualizersInitialized = true;
				if (__instance.manageLeftCap)
				{
					if (__instance.leftCapDefault == null || __instance.leftCapConduit == null || __instance.leftCapLaunchpad == null)
						hasVisualizersInitialized = false;
				}
				if (__instance.manageRightCap)
				{
					if (__instance.rightCapDefault == null || __instance.rightCapConduit == null || __instance.rightCapLaunchpad == null)
						hasVisualizersInitialized = false;
				}
				return hasVisualizersInitialized;
			}
		}



		[HarmonyPatch(typeof(ReceptacleSideScreen), nameof(ReceptacleSideScreen.Initialize))]
		public class ReceptacleSideScreen_Initialize_Patch
		{
			/// <summary>
			/// prevents that stupid assert in ReceptacleSideScreen.Initialize that crashes the game for no reason on preconfiguring sometimes for some people due to race conditions
			/// </summary>
			/// <param name="_"></param>
			/// <param name="orig"></param>
			/// <returns></returns>
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var m_Assert = AccessTools.Method(typeof(Debug), nameof(Debug.Assert), [typeof(bool), typeof(object)]);

				foreach (var ci in orig)
				{
					if (ci.Calls(m_Assert))
					{
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ReceptacleSideScreen_Initialize_Patch), nameof(DoNotCrashThisScreenWithThatStoopidAssert)));
					}
					else
						yield return ci;

				}
			}

			private static void DoNotCrashThisScreenWithThatStoopidAssert(bool entityCountCorrect, object crashMsg)
			{
				if (entityCountCorrect)
					return;
				SgtLogger.warning((string)crashMsg);
			}
		}
	}
}

using BlueprintsV2.BlueprintData;
using BlueprintsV2.ModAPI;
using HarmonyLib;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Device;
using UtilLibs;
using static BlueprintsV2.BlueprintData.DataTransferHelpers;
using static Grid;

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

		static GameObject temporaryTargetBuilding;
		static UnderConstructionDataTransfer lastSelected;
		public static KSelectable TemporarySelectable { get; private set; }

		public static bool HasDataTransferComponents(BuildingUnderConstruction building)
		{
			if (building == null)
				return false;
			if (!API_Methods.AllowedByRules(building.Def))
				return false;
			var completeVersion = building.Def.BuildingComplete;
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
			if (temporaryTargetBuilding != null)
			{
				UnityEngine.Object.Destroy(temporaryTargetBuilding);
				temporaryTargetBuilding = null;
			}

			var world = origin.GetMyWorld();
			var worldOffset = world.WorldOffset;

			int cell = Grid.XYToCell(worldOffset.X, worldOffset.Y);

			cell += Mathf.CeilToInt((def.WidthInCells / 2f)); //spawn it close to the origin, but dont let it clip into negative cell indicies

			temporaryTargetBuilding = def.Create(Grid.CellToPos(cell), null, [SimHashes.Unobtanium.CreateTag()], null, 100, def.BuildingComplete);

			TemporarySelectable = temporaryTargetBuilding.GetComponent<KSelectable>();
			//prevent "build outside start biome" achievment from triggering
			temporaryTargetBuilding.GetComponent<KPrefabID>().AddTag(GameTags.TemplateBuilding);

			if (temporaryTargetBuilding.TryGetComponent<KBatchedAnimController>(out var kbac))
				kbac.animScale = 0;

			//hide deconstruction button
			if (temporaryTargetBuilding.TryGetComponent<Deconstructable>(out var decon))
				decon.allowDeconstruction = false;

			bool isPaused = SpeedControlScreen.Instance.IsPaused;
			if (isPaused)
				SpeedControlScreen.Instance.Unpause(false);

			//1 frame delay to properly load the extra buttons on the menu screen
			GameScheduler.Instance.ScheduleNextFrame("pause", (_) =>
			{
				UnderConstructionDataTransfer.TransferDataTo(temporaryTargetBuilding, origin.GetStoredData());
				Game.Instance.Trigger((int)GameHashes.SelectObject, temporaryTargetBuilding);
				if (isPaused)
					SpeedControlScreen.Instance.Pause(false);
				Game.Instance.Subscribe((int)GameHashes.SelectObject, HandleDeselection);
			});
		}
		public static void HandleDeselection(object data)
		{
			if (data != null && (data is GameObject target) && target == temporaryTargetBuilding)
				return;

			Game.Instance.Unsubscribe((int)GameHashes.SelectObject, HandleDeselection);
			var buildingSettingData = API_Methods.GetAdditionalBuildingData(temporaryTargetBuilding);
			UnityEngine.Object.Destroy(temporaryTargetBuilding);
			foreach (var entries in buildingSettingData)
				lastSelected.SetDataToApply(entries.Key, entries.Value);

			TemporarySelectable = null;
			UnderConstructionDataTransfer.SelectButtonUnlocked = true;

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
				if (__instance.gameObject == temporaryTargetBuilding)
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
			public static bool Prefix(GameObject target) => target != temporaryTargetBuilding;
		}
		/// <summary>
		/// dont show copy setting button on temp building
		/// </summary>
		[HarmonyPatch(typeof(CopyBuildingSettings), nameof(CopyBuildingSettings.OnRefreshUserMenu))]
		public class CopyBuildingSettings_OnRefreshUserMenu_Patch
		{
			public static bool Prefix(CopyBuildingSettings __instance) => __instance.gameObject != temporaryTargetBuilding;
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
	}
}

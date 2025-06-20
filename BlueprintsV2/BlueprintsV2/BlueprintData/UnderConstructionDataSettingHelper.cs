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
using UtilLibs;
using static Grid;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
	public static class UnderConstructionDataSettingHelper
	{
		public static List<string> ComponentsToIgnore =
			[
			"BuildingEnabledButton"
			,API_Consts.ConduitFlagID

			];

		static GameObject temporaryTargetBuilding;
		static UnderConstructionDataTransfer lastSelected;
		public static KSelectable TemporarySelectable { get; private set; }

		public static bool HasDataTransferComponents(BuildingUnderConstruction building)
		{
			if (building == null)
				return false;
			var completeVersion = building.Def.BuildingComplete;
			var data = API_Methods.GetAdditionalBuildingData(completeVersion);

			foreach (var entry in ComponentsToIgnore)
				data.Remove(entry);

			return data.Any();
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

			//var leftCell = Grid.CellLeft(Grid.PosToCell(origin));

			temporaryTargetBuilding = def.Create(Grid.CellToPos(0), null, [SimHashes.Unobtanium.CreateTag()], null, 100, def.BuildingComplete);
			bool isPaused = SpeedControlScreen.Instance.IsPaused;
			//if (isPaused)
			//	SpeedControlScreen.Instance.Unpause(false);

			UnderConstructionDataTransfer.TransferDataTo(temporaryTargetBuilding, origin.GetStoredData());
			TemporarySelectable = temporaryTargetBuilding.GetComponent<KSelectable>();
			DetailsScreen.Instance.Refresh(temporaryTargetBuilding);
			Game.Instance.Subscribe((int)GameHashes.SelectObject, HandleDeselection);
			//if (isPaused)
			//	SpeedControlScreen.Instance.Pause(false);
		}
		public static void HandleDeselection(object data)
		{
			Game.Instance.Unsubscribe((int)GameHashes.SelectObject, HandleDeselection);
			var buildingSettingData = API_Methods.GetAdditionalBuildingData(temporaryTargetBuilding);
			UnityEngine.Object.Destroy(temporaryTargetBuilding);
			foreach (var entries in buildingSettingData)
				lastSelected.SetDataToApply(entries.Key, entries.Value);

			TemporarySelectable = null;

		}
		[HarmonyPatch(typeof(KSelectable), nameof(KSelectable.GetStatusItemGroup))]
		public class KSelectable_GetStatusItemGroup_Patch
		{
			public static bool Prefix(KSelectable __instance) => TemporarySelectable != __instance;
		}

	}
}

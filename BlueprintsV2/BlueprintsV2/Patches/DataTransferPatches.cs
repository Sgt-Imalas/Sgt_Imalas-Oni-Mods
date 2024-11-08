using BlueprintsV2.BlueprintData;
using HarmonyLib;
using Klei.AI;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace BlueprintsV2.Patches
{
	internal class DataTransferPatches
	{
		[HarmonyPatch(typeof(BuildingLoader), nameof(BuildingLoader.CreateBuildingUnderConstruction))]
		public class BuildingLoader_CreateBuildingUnderConstruction_Patch
		{
			public static void Postfix(GameObject __result)
			{
				__result.AddOrGet<UnderConstructionDataTransfer>();
			}
		}
		public static class ApplySettingsToNewBuilding
		{
			[HarmonyPatch(typeof(GameplayEventManager), "OnSpawn")]
			public static class GameplayEventManager_OnSpawn
			{
				public static void Postfix(GameplayEventManager __instance)
				{
					__instance.Subscribe(-1661515756, OnBuildingConstructed);
				}
			}
			[HarmonyPatch(typeof(GameplayEventManager), "OnCleanUp")]
			public static class GameplayEventManager_OnCleanup
			{
				public static void Postfix(GameplayEventManager __instance)
				{
					__instance.Unsubscribe(-1661515756, OnBuildingConstructed);
				}
			}
			static void OnBuildingConstructed(object data)
			{
				if (data == null)
					return;

				//SgtLogger.l("onbuildingconstructed");
				if (data is BonusEvent.GameplayEventData bonusData)
				{
					if (bonusData.building == null)
						return;

					var pos = bonusData.building.NaturalBuildingCell();
					var layer = bonusData.building.Def.ObjectLayer;
					Tuple<int, ObjectLayer> targetPos = new(pos, layer);

					var targetBuilding = bonusData.building.gameObject;
					//SgtLogger.l($"first: {UnderConstructionDataTransfer.RegisteredTransferPlans.TryGetValue(targetPos, out var test)}, second: {test.building.Def.PrefabID == bonusData.building.Def.PrefabID} ({test.building.Def.PrefabID},{bonusData.building.Def.PrefabID}), third {targetBuilding != null}");

					if (UnderConstructionDataTransfer.RegisteredTransferPlans.TryGetValue(targetPos, out UnderConstructionDataTransfer dataTransferItem)
						&& dataTransferItem.building.Def.PrefabID == bonusData.building.Def.PrefabID
						&& targetBuilding != null)
					{
						var dataToApply = dataTransferItem.GetStoredData();
						GameScheduler.Instance.ScheduleNextFrame("delayed settings application", (_) =>
						{
							UnderConstructionDataTransfer.TransferDataTo(targetBuilding, dataToApply);
						});
					}
				}
			}
		}

	}
}

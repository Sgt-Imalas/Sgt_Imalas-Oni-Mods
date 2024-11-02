using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace RebuildPreserve
{
	internal class Patches
	{

		[HarmonyPatch(typeof(Reconstructable))]
		[HarmonyPatch(nameof(Reconstructable.TryCommenceReconstruct))]
		public static class Reconstructable_TryCommenceReconstruct_Patch
		{

			public static void Prefix(Reconstructable __instance)
			{
				if (!__instance.reconstructRequested)
					return;

				var go = __instance.gameObject;
				__instance.TryGetComponent<BuildingComplete>(out var building);
				GameObject cachedSource = null;


				foreach (var comp in go.GetComponents(typeof(KMonoBehaviour)))
				{
					if (comp != null)
					{
						if (cachedSource == null)
						{
							cachedSource = new GameObject("cachedSource_" + go.name);
							cachedSource.SetActive(false);
						}

						var method = new Traverse(comp).Method("OnCopySettings", new[] { typeof(object) });
						var method2 = new Traverse(comp).Method("OnCopySettingsDelegate", new[] { typeof(object) });
						if (method.MethodExists() || method2.MethodExists())
						{
							var cache = cachedSource.AddComponent(comp.GetType());
							//SgtLogger.l(message: "Caching " + comp.GetType().ToString());
							CopyProperties(comp, cache);
						}
					}
				}

				if (go.TryGetComponent<IHaveUtilityNetworkMgr>(out var networkItem))
				{
					if (cachedSource == null)
					{
						cachedSource = new GameObject("cachedSource_" + go.name);
						cachedSource.SetActive(false);
					}
					cachedSource.AddComponent<ConduitDirectionInfo>().StoreConduitConnections(building.NaturalBuildingCell(), networkItem.GetNetworkManager());
				}

				if (cachedSource != null)
				{
					//SgtLogger.l(message: "adding to dic ");
					var targetPos = new Tuple<int, ObjectLayer>(building.NaturalBuildingCell(), building.Def.ObjectLayer);

					BuildSettingsPreservationData.Instance.ReplaceEntry(targetPos, cachedSource, __instance.building.Def.PrefabID);

					//if (cachedSource.TryGetComponent<TreeFilterable>(out var filter))
					//{

					//    SgtLogger.l("getting filters, count: " + filter.GetTags().Count());
					//}
				}
			}

			public static void CopyProperties(object source, object destination)
			{
				if (source == null || destination == null)
					throw new Exception("Source or/and Destination Objects are null");
				// Getting the Types of the objects

				Type typeDest = destination.GetType();
				Type typeSrc = source.GetType();

				var currentParentType = typeDest.BaseType;

				FieldInfo[] srcProps = typeSrc.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				while (currentParentType != null && currentParentType != typeof(KMonoBehaviour) && currentParentType != typeof(StateMachineComponent))
				{
					//SgtLogger.l(currentParentType.Name,"climbin inheritance");
					srcProps = srcProps.Union(currentParentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)).ToArray();
					currentParentType = currentParentType.BaseType;
				}


				if (srcProps == null || srcProps.Length == 0)
				{
					SgtLogger.l("no props found: " + source.ToString());
					return;
				}

				foreach (var srcProp in srcProps)
				{
					//SgtLogger.l(source.ToString() + srcProp.Name);
					//if (srcProp.Name.ToLowerInvariant().Contains("smi")) //crude fix for supress notifications crash
					//    continue;
					var value = Traverse.Create(source).Field(srcProp.Name).GetValue();
					//if(value != null)
					//SgtLogger.l(value.ToString(), srcProp.Name);
					Traverse.Create(destination).Field(srcProp.Name).SetValue(value);
				}
			}
		}
		[HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.TryPlace), new Type[] { typeof(GameObject), typeof(Vector3), typeof(Orientation), typeof(IList<Tag>), typeof(string), typeof(bool), typeof(int) })]
		public class BuildingDef_TryPlace
		{
			public static void Postfix(BuildingDef __instance, GameObject __result, Vector3 pos)
			{
				if (__result == null)
					return;
				int cell = Grid.PosToCell(pos);

				if (BuildSettingsPreservationData.Instance.TryGetEntry(new(cell, __instance.ObjectLayer), out var cachedData, out var cachedPrefabId)
					)
				{
					if (cachedData.TryGetComponent<ConduitDirectionInfo>(out var targetConduitDirectionInfo) && targetConduitDirectionInfo.initialized)
					{

						GameScheduler.Instance.ScheduleNextFrame("delayed conduit applying", (_) =>
						{
							targetConduitDirectionInfo.ApplyConduitConnections(false);
							if (__result.TryGetComponent<KAnimGraphTileVisualizer>(out var targetAnimGraphTileVisualizer))
							{
								targetAnimGraphTileVisualizer.UpdateConnections(targetConduitDirectionInfo.connectedDirections);
							}
						});
					}

					if (cachedData.TryGetComponent<Prioritizable>(out _) && __result.TryGetComponent<Prioritizable>(out var targetPrio))
						targetPrio.OnCopySettings(cachedData);

				}
			}
		}


		[HarmonyPatch(typeof(Constructable), nameof(Constructable.OnCancel))]
		public class Constructable_OnCancel
		{
			public static void Prefix(Constructable __instance)
			{
				var cell = __instance.building.NaturalBuildingCell();
				var layer = __instance.building.Def.ObjectLayer;
				BuildSettingsPreservationData.Instance.RemoveEntry(new(cell, layer));
			}
		}

		[HarmonyPatch(typeof(SaveGame), nameof(SaveGame.OnPrefabInit))]
		public class SaveGame_OnPrefabInit_Patch
		{
			public static void Postfix(SaveGame __instance)
			{
				BuildSettingsPreservationData.Instance = __instance.gameObject.AddOrGet<BuildSettingsPreservationData>();
			}
		}

		//[HarmonyPatch(typeof(DetailsScreenMaterialPanel), nameof(DetailsScreenMaterialPanel.SetTarget))]
		//public class DetailsScreenMaterialPanel_SetTarget
		//{
		//    public static void Postfix(DetailsScreenMaterialPanel __instance, GameObject target)
		//    {
		//        if (target == null)
		//            return;

		//        GameScheduler.Instance.Schedule("instantly open material selection panel",0.2f, (_) =>
		//        {
		//            __instance.OpenMaterialSelectionPanel();
		//            __instance.RefreshMaterialSelectionPanel();
		//            __instance.RefreshOrderChangeMaterialButton();
		//        });
		//    }
		//}
		[HarmonyPatch(typeof(BuildingConfigManager), nameof(BuildingConfigManager.OnPrefabInit))]
		public class BuildingConfigManager_OnPrefabInit
		{
			public static void Postfix(BuildingConfigManager __instance)
			{
				__instance.baseTemplate.AddComponent<AutomatedBrokenRebuild>();
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
                    if (bonusData.building == null || bonusData.building.Def == null )
                        return;

                    var pos = bonusData.building.NaturalBuildingCell();
					var layer = bonusData.building.Def.ObjectLayer;
					var targetPos = new Tuple<int, ObjectLayer>(pos, layer);

					var targetBuilding = bonusData.building.gameObject;
					if (BuildSettingsPreservationData.Instance.TryGetEntry(targetPos, out var cachedGameObject, out var previousPrefabId)
						&& targetBuilding != null
						&& cachedGameObject != null
						&& previousPrefabId == bonusData.building.Def.PrefabID)
					{
						GameScheduler.Instance.ScheduleNextFrame("delayed settings application", (_) =>
						{
							targetBuilding.Trigger((int)GameHashes.CopySettings, cachedGameObject);
							if (targetBuilding.TryGetComponent<KAnimGraphTileVisualizer>(out var targetAnimGraphTileVisualizer))
							{
								targetAnimGraphTileVisualizer.Refresh();
							}
							BuildSettingsPreservationData.Instance.RemoveEntry(targetPos);
						});
					}
				}
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}


		[HarmonyPatch(typeof(DetailsScreenMaterialPanel), "RefreshOrderChangeMaterialButton", new Type[] { typeof(object) })]
		public static class DetailsScreenMaterialPanel_RefreshOrderChangeMaterialButton
		{
			public static void Postfix(DetailsScreenMaterialPanel __instance)
			{
				__instance.orderChangeMaterialButton.isInteractable = __instance.materialSelectionPanel.CurrentSelectedElement != null;
			}
		}
	}
}

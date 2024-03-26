using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AmbienceManager;
using static Grid.Restriction;
using static RebuildPreserve.ModAssets;
using static STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.FACADES;
using static STRINGS.UI.CLUSTERMAP;

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
                var go = __instance.gameObject;
                __instance.TryGetComponent<BuildingComplete>(out var building);
                List<Traverse> traversed = new List<Traverse>();

                GameObject cachedSource = null;

                foreach (var comp in go.GetComponents(typeof(KMonoBehaviour)))
                {
                    if (comp != null)
                    {
                        if (cachedSource == null)
                            cachedSource = new GameObject("cachedSource_" + go.name);
                        cachedSource.SetActive(false);

                        var method = new Traverse(comp).Method("OnCopySettings", new[] { typeof(object) });
                        if (method.MethodExists())
                        {
                            var cache = cachedSource.AddComponent(comp.GetType());
                            CopyProperties(comp, cache);
                            //SgtLogger.l(message: "Caching " + comp.GetType().ToString());
                        }
                    }
                }

                if (cachedSource != null)
                {
                    //SgtLogger.l(message: "adding to dic ");
                    var targetPos = new Tuple<int, ObjectLayer>(building.NaturalBuildingCell(), building.Def.ObjectLayer);

                    BuildSettingsPreservationData.Instance.ToCopyFromComponents.Remove(targetPos);
                    BuildSettingsPreservationData.Instance.ToCopyFromComponents.Add(targetPos, cachedSource);

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

                FieldInfo[] srcProps = typeSrc.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
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

                    Traverse.Create(destination).Field(srcProp.Name).SetValue(Traverse.Create(source).Field(srcProp.Name).GetValue());
                }
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
                //SgtLogger.l("onbuildingconstructed");
                if (data is BonusEvent.GameplayEventData bonusData)
                {

                    var pos = bonusData.building.NaturalBuildingCell();
                    var layer = bonusData.building.Def.ObjectLayer;
                    var targetPos = new Tuple<int, ObjectLayer>(pos, layer);

                    if (!BuildSettingsPreservationData.Instance.ToCopyFromComponents.ContainsKey(targetPos))
                    {
                        return;
                    }
                    var targetBuilding = Grid.Objects[pos, (int)layer];
                    if (targetBuilding != null)
                    {
                        GameScheduler.Instance.ScheduleNextFrame("delayed settings application", (_) =>
                        {
                            targetBuilding.Trigger((int)GameHashes.CopySettings, BuildSettingsPreservationData.Instance.ToCopyFromComponents[targetPos]);
                            BuildSettingsPreservationData.Instance.ToCopyFromComponents.Remove(targetPos);
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


        [HarmonyPatch(typeof(DetailsScreenMaterialPanel), "RefreshOrderChangeMaterialButton", new Type[] {typeof(object)})]
        public static class DetailsScreenMaterialPanel_RefreshOrderChangeMaterialButton
        {
            public static void Postfix(DetailsScreenMaterialPanel __instance)
            {
                __instance.orderChangeMaterialButton.isInteractable = __instance.materialSelectionPanel.CurrentSelectedElement != null;
            }
        }
    }
}

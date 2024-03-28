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
                        { 
                            cachedSource = new GameObject("cachedSource_" + go.name);
                            cachedSource.SetActive(false);
                        }

                        var method = new Traverse(comp).Method("OnCopySettings", new[] { typeof(object) });
                        var method2 = new Traverse(comp).Method("OnCopySettingsDelegate", new[] { typeof(object) });
                        if (method.MethodExists()||method2.MethodExists())
                        {
                            var cache = cachedSource.AddComponent(comp.GetType());
                            //SgtLogger.l(message: "Caching " + comp.GetType().ToString());
                            CopyProperties(comp, cache);
                        }
                    }
                }

                if (cachedSource != null)
                {
                    //SgtLogger.l(message: "adding to dic ");
                    var targetPos = new Tuple<int, ObjectLayer>(building.NaturalBuildingCell(), building.Def.ObjectLayer);
                    BuildSettingsPreservationData.Instance.ReplaceEntry(targetPos, cachedSource);

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
                while(currentParentType != null && currentParentType != typeof(KMonoBehaviour) && currentParentType != typeof(StateMachineComponent))
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
        [HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.TryPlace), new Type[] { typeof(GameObject ), typeof(Vector3), typeof(Orientation), typeof(IList < Tag >), typeof(string), typeof(bool), typeof(int) } )]
        public class BuildingDef_TryPlace
        {
            public static void Postfix(BuildingDef __instance,GameObject __result, Vector3 pos)
            {
                if (__result == null)
                    return;
                int cell = Grid.PosToCell(pos);

                if(BuildSettingsPreservationData.Instance.TryGetEntry(new(cell, __instance.ObjectLayer),out var cachedData)
                    && cachedData.TryGetComponent<Prioritizable> (out _)                  
                    )
                {
                    if (__result.TryGetComponent<Prioritizable>(out var targetPrio))
                        targetPrio.OnCopySettings(cachedData);
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
                //SgtLogger.l("onbuildingconstructed");
                if (data is BonusEvent.GameplayEventData bonusData)
                {

                    var pos = bonusData.building.NaturalBuildingCell();
                    var layer = bonusData.building.Def.ObjectLayer;
                    var targetPos = new Tuple<int, ObjectLayer>(pos, layer);

                    var targetBuilding = bonusData.building.gameObject;
                    if (BuildSettingsPreservationData.Instance.TryGetEntry(targetPos, out var cachedGameObject) && targetBuilding != null && cachedGameObject != null)
                    {
                        GameScheduler.Instance.ScheduleNextFrame("delayed settings application", (_) =>
                        {
                            targetBuilding.Trigger((int)GameHashes.CopySettings, cachedGameObject);
                            BuildSettingsPreservationData.Instance.RemoveEntry(targetPos);
                        });
                    }
                }
            }
            [HarmonyPatch(typeof(Automatable), "OnCopySettings")]
            public static class Automatable_OnCopySettings
            {
                public static bool Prefix(Automatable __instance, object data)
                {
                    Automatable component = ((GameObject)data).GetComponent<Automatable>();
                    SgtLogger.l(component + " : " + (component != null), "automatable existing");

                    if (component != null)
                    {
                        SgtLogger.l(__instance.automationOnly + " <-instance, target-> " + component.automationOnly, "automatable");
                        __instance.automationOnly = component.automationOnly;
                    }
                    return false;
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

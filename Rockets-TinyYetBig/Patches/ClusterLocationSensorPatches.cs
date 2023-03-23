using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rockets_TinyYetBig.Patches
{
    internal class ClusterLocationSensorPatches
    {
        static AxialI DisabledLocation = new AxialI(999, 999);

        [HarmonyPatch(typeof(ClusterLocationFilterSideScreen))]
        [HarmonyPatch(nameof(ClusterLocationFilterSideScreen.Build))]
        public static class AddSpecificLocation
        {

            static GameObject SpecificLocationSpaceRow;
            public static void Postfix(ClusterLocationFilterSideScreen __instance)
            {
                return;
                var locationDummy = new AxialI(2, 2); 
                Util.KDestroyGameObject(SpecificLocationSpaceRow);
                
                SpecificLocationSpaceRow = Util.KInstantiateUI(__instance.rowPrefab, __instance.listContainer,true);

                SpecificLocationSpaceRow.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText((string)"Dedicated Location: "+ locationDummy.ToString());//STRINGS.UI.UISIDESCREENS.CLUSTERLOCATIONFILTERSIDESCREEN.EMPTY_SPACE_ROW);
                SpecificLocationSpaceRow.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite((object)"hex").first;
                SpecificLocationSpaceRow.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Color.black;
                SpecificLocationSpaceRow.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = (System.Action)(() => {
                    __instance.sensor.SetLocationEnabled(locationDummy, !__instance.sensor.CheckLocationSelected(locationDummy));
                    __instance.Refresh();
                });
            }
        }
        [HarmonyPatch(typeof(LogicClusterLocationSensorConfig))]
        [HarmonyPatch(nameof(LogicClusterLocationSensorConfig.DoPostConfigureComplete))]
        public static class AddClusterSelector
        {

            static GameObject SpecificLocationSpaceRow;
            public static void Postfix(GameObject go)
            {
                ClusterDestinationSelector destinationSelector = go.AddOrGet<ClusterDestinationSelector>();
                destinationSelector.assignable = true;
                destinationSelector.requireAsteroidDestination = false;
                destinationSelector.m_destination = (DisabledLocation);
            }
        }
        [HarmonyPatch(typeof(LogicClusterLocationSensor))]
        [HarmonyPatch(nameof(LogicClusterLocationSensor.CheckCurrentLocationSelected))]
        public static class AdditionalCheck
        {

            static GameObject SpecificLocationSpaceRow;
            public static bool Prefix(LogicClusterLocationSensor __instance, ref bool __result)
            {
                if(__instance.TryGetComponent<ClusterDestinationSelector>(out var selector))
                {
                    if(selector.GetDestination() == selector.gameObject.GetMyWorldLocation()&& selector.GetDestination() != DisabledLocation)
                    {
                        __result = true;
                        return false;
                    }
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ClusterDestinationSideScreen))]
        [HarmonyPatch(nameof(ClusterDestinationSideScreen.OnClickClearDestination))]
        public static class ProperClear
        {
            public static bool Prefix(ClusterDestinationSideScreen __instance)
            {
                if (__instance.targetSelector.TryGetComponent<LogicClusterLocationSensor>(out var logicSensor))
                {
                    __instance.targetSelector.SetDestination(DisabledLocation);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ClusterDestinationSideScreen))]
        [HarmonyPatch(nameof(ClusterDestinationSideScreen.Refresh))]
        public static class BetterViewOfLocation
        {
            public static void GetLocationDescriptionWithPOIs(AxialI location, out Sprite sprite, out string label)
            {
                List<ClusterGridEntity> visibleEntitiesAtCell = ClusterGrid.Instance.GetVisibleEntitiesAtCell(location);
                ClusterGridEntity clusterGridEntity = visibleEntitiesAtCell.Find((ClusterGridEntity x) => x.Layer == EntityLayer.Asteroid || x.Layer == EntityLayer.POI);
                ClusterGridEntity visibleEntityOfLayerAtAdjacentCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(location, EntityLayer.Asteroid);
                if (clusterGridEntity != null)
                {
                    sprite = clusterGridEntity.Layer == EntityLayer.POI ?  
                        Def.GetUISpriteFromMultiObjectAnim(clusterGridEntity.AnimConfigs.First().animFile,clusterGridEntity.AnimConfigs.First().initialAnim) 
                        : clusterGridEntity.GetUISprite();

                    label = clusterGridEntity.Name;
                }
                else if (visibleEntityOfLayerAtAdjacentCell != null)
                {
                    sprite = visibleEntityOfLayerAtAdjacentCell.GetUISprite();
                    label = global::STRINGS.UI.SPACEDESTINATIONS.ORBIT.NAME_FMT.Replace("{Name}", visibleEntityOfLayerAtAdjacentCell.Name);
                }
                else if (ClusterGrid.Instance.IsCellVisible(location))
                {
                    sprite = Assets.GetSprite("hex_unknown");
                    label = global::STRINGS.UI.SPACEDESTINATIONS.EMPTY_SPACE.NAME;
                }
                else
                {
                    sprite = Assets.GetSprite("unknown_far");
                    label = global::STRINGS.UI.SPACEDESTINATIONS.FOG_OF_WAR_SPACE.NAME;
                }
            }

            public static bool Prefix(ClusterDestinationSideScreen __instance)
            {
                if (__instance.targetSelector.TryGetComponent<LogicClusterLocationSensor>(out var logicSensor))
                {
                    var selector = __instance.targetSelector;
                    if(!(selector.GetDestination() == DisabledLocation))
                    {
                        GetLocationDescriptionWithPOIs(selector.GetDestination(), out var sprite, out var label);
                        __instance.destinationImage.sprite = sprite;
                        __instance.destinationLabel.text = (string)global::STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE + ": " + label;
                        __instance.clearDestinationButton.isInteractable = true;
                    }
                    else
                    {
                        __instance.destinationImage.sprite = Assets.GetSprite((HashedString)"hex_unknown");
                        __instance.destinationLabel.text = (string)global::STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE + ": " + (string)global::STRINGS.UI.SPACEDESTINATIONS.NONE.NAME;
                        __instance.clearDestinationButton.isInteractable = false;
                    }


                    __instance.launchPadDropDown.gameObject.SetActive(false);
                    __instance.repeatButton.gameObject.SetActive(false);
                    return false;
                }
                return true;
            }
        }
    }
}

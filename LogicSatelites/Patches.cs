using HarmonyLib;
using LogicSatelites.Behaviours;
using LogicSatelites.Buildings;
using LogicSatelites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static ComplexRecipe;

namespace LogicSatelites
{
    class Patches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                RocketryUtils.AddRocketModuleToBuildList(SateliteCarrierModuleConfig.ID);
            }
        }

        [HarmonyPatch(typeof(LogicBroadcastReceiver))]
        [HarmonyPatch(nameof(LogicBroadcastReceiver.IsSpaceVisible))]
        public static class BroadcastRecieverInSpace_patch
        {
            public static bool Prefix(LogicBroadcastReceiver __instance, ref bool __result)
            {
                if (__instance.gameObject.GetComponent<SatelliteGridEntity>() != null)
                {
                    //Debug.Log("satellite is in Space");
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(LogicBroadcaster))]
        [HarmonyPatch(nameof(LogicBroadcaster.IsSpaceVisible))]
        public static class BroadcasterInSpace_patch
        {
            public static bool Prefix(LogicBroadcaster __instance, ref bool __result)
            {
                if (__instance.gameObject.GetComponent<SatelliteGridEntity>() != null)
                {
                    //Debug.Log("satellite is in Space");
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(LogicBroadcastChannelSideScreen), "Refresh")]
        public static class BroadCasterSidescreenWorldPatch
        {
           // (int) Traverse.Create(__instance).Field("pad_cell").GetValue());
            public static bool Prefix(LogicBroadcastChannelSideScreen __instance, Dictionary<LogicBroadcaster, GameObject> ___broadcasterRows, LogicBroadcastReceiver ___sensor)
            {
                foreach (KeyValuePair<LogicBroadcaster, GameObject> broadcasterRow in ___broadcasterRows)
                {
                        KeyValuePair<LogicBroadcaster, GameObject> kvp = broadcasterRow;
                        kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(kvp.Key.gameObject.GetProperName());
                        kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("DistanceLabel").SetText((string)(LogicBroadcastReceiver.CheckRange(___sensor.gameObject, kvp.Key.gameObject) ? global::STRINGS.UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.IN_RANGE : global::STRINGS.UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.OUT_OF_RANGE));
                        kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite((object)kvp.Key.gameObject).first;
                        kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Def.GetUISprite((object)kvp.Key.gameObject).second;
                        WorldContainer myWorld = kvp.Key.GetMyWorld();
                    if (myWorld != null) { 
                        kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").sprite = myWorld.IsModuleInterior ? Assets.GetSprite((HashedString)"icon_category_rocketry") : Def.GetUISprite((object)myWorld.GetComponent<ClusterGridEntity>()).first;
                        kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").color = myWorld.IsModuleInterior ? Color.white : Def.GetUISprite((object)myWorld.GetComponent<ClusterGridEntity>()).second;
                    }
                    else
                    {
                        var sat = kvp.Key.gameObject.GetComponent<SatelliteGridEntity>();
                        if(sat != null)
                        {
                            kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").sprite =  Def.GetUISprite((object)sat.GetComponent<ClusterGridEntity>()).first;
                            kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").color = Def.GetUISprite((object)sat.GetComponent<ClusterGridEntity>()).second;
                        }
                        else
                        {
                            throw new ArgumentNullException("No world or Satellite found");
                        }

                    }
                    kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = (System.Action)(() =>
                        {
                            ___sensor.SetChannel(kvp.Key);
                            Traverse.Create(__instance).Method("Refresh");
                        });
                        kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState((UnityEngine.Object)___sensor.GetChannel() == (UnityEngine.Object)kvp.Key ? 1 : 0);
                    }
                    return false;
            }
        }



        //[HarmonyPatch(typeof(ModuleFlightUtilitySideScreen), "SetTarget")]
        //[HarmonyPatch(nameof(ModuleFlightUtilitySideScreen.SetTarget))]
        //public static class ModuleFlightUtilitySideScreen_Gibinfo
        //{
        //    public static void Postfix(ModuleFlightUtilitySideScreen __instance)
        //    {
        //        Debug.Log("FLIGHTSCREEN MONO");
        //        UIUtils.ListAllChildren(__instance.transform);
        //    }
        //}

        [HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
        public static class SatelitePartsPatch
        {
            public static void Postfix()
            {
                AddSatellitePartsRecipe();
            }
            private static void AddSatellitePartsRecipe()
            {
                RecipeElement[] input = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.Glass.CreateTag(), 12f),
                    new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 3f),
                    new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 15f)
                };

                ComplexRecipe.RecipeElement[] output = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SatelliteComponentConfig.ID, 1f)
                };

                string product = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

                SatelliteComponentConfig.recipe = new ComplexRecipe(product, input, output)
                {
                    time = 45,
                    description = "Satellite parts, the bread and butter of satellite construction",
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag>()
                    {
                        CraftingTableConfig.ID
                    },
                };

            }
        }

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_SatelliteCarrier
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                UIUtils.AddClonedSideScreen<SatelliteCarrierModuleSideScreen>("SatelliteCarrierModuleSideScreen", "ModuleFlightUtilitySideScreen", typeof(ModuleFlightUtilitySideScreen));
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
    }
}

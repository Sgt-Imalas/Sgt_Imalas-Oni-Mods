using ClothingLockerMod;
using Database;
using HarmonyLib;
using Klei.AI;
using LessAnnoyingPajamas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ComplexRecipe;

namespace StoreDreamJournals
{
    class Patches
    {

        [HarmonyPatch(typeof(GravitasContainerConfig))]
        [HarmonyPatch(nameof(GravitasContainerConfig.ConfigureBuildingTemplate))]
        public static class MakePajamaDispenserQueueable_Patch
        {
            static Tag fillerItem = SimHashes.Water.CreateTag();
            public static void Postfix(GameObject go, CraftingTableConfig __instance)
            {

                return;
                go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
                go.AddOrGet<DropAllWorkable>();
                Prioritizable.AddRef(go);
                ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
                go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
                {
                    Assets.GetAnim((HashedString) "anim_interacts_gravitas_container_kanim")
                };
                go.AddOrGet<ComplexFabricatorWorkable>();
                fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
                fabricator.heatedTemperature = UtilMethods.GetKelvinFromC(20f);
                go.AddOrGet<FabricatorIngredientStatusManager>();
                ConfigureRecipe();
                BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);

            }
            private static void ConfigureRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(fillerItem, 0.1f)
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(QueueableSleepClinicPajamas.ID, 1f,RecipeElement.TemperatureOperation.Heated)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(GravitasContainerConfig.ID, input, output);

                QueueableSleepClinicPajamas.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = 30f,
                    description = global::STRINGS.EQUIPMENT.PREFABS.SLEEPCLINICPAJAMAS.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { GravitasContainerConfig.ID }
                };
            }
        }

        [HarmonyPatch(typeof(GravitasContainerConfig))]
        [HarmonyPatch(nameof(GravitasContainerConfig.DoPostConfigureComplete))]
        public static class RemovePajamaDispenserComponent
        {
            public static bool Prefix(GameObject go)
            {
                return true;// false;
            }
        }

        [HarmonyPatch(typeof(SleepClinicPajamas), "CreateEquipmentDef")]
        public class SleepClinicPajamas_CreateEquipmentDef_Patch
        {
            public static void Postfix(EquipmentDef __result)
            {
                __result.OnUnequipCallBack = eq => OnUnEquip(eq, __result.OnUnequipCallBack);
                __result.OnEquipCallBack = eq => OnEquip(eq, __result.OnUnequipCallBack);
            }

            private static void OnUnEquip(Equippable eq, Action<Equippable> originalCb)
            {
                CoolVestConfig.OnUnequipVest(eq);
                if (eq.assignee == null)

                    return;
                Ownables soleOwner = eq.assignee.GetSoleOwner();
                if (soleOwner == null)
                    return;
                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
                if (targetGameObject == null)
                    return;
                Navigator component4 = targetGameObject.GetComponent<Navigator>();
                if (component4 != null)
                    component4.ClearFlags(PathFinder.PotentialPath.Flags.HasJetPack);

            }
            private static void OnEquip(Equippable eq, Action<Equippable> originalCb)
            {
                originalCb(eq);

                Ownables soleOwner = eq.assignee.GetSoleOwner();
                if (!(soleOwner != null))
                    return;
                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
                if (targetGameObject == null)
                    return;
                Navigator component1 = targetGameObject.GetComponent<Navigator>();
                if (component1 != null)
                    component1.SetFlags(ModAssets.PajamasFlag);

            }
        }
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, ClothingLockerConfig.ID, EspressoMachineConfig.ID);
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, ClothingLockerMarkerConfig.ID, EspressoMachineConfig.ID);
            }
        }

        //[HarmonyPatch(typeof(ComplexFabricator))]
        //[HarmonyPatch("TransferCurrentRecipeIngredientsForBuild")]
        //public static class ComplexFab_patch
        //{
        //    public static bool Prefix(ComplexRecipe recipe, ComplexRecipe[] ___recipe_list, int ___workingOrderIdx)
        //    {
        //        if(___recipe_list[___workingOrderIdx] == QueueableSleepClinicPajamas.recipe)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //}
    }

}

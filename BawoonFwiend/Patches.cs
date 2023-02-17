using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static BawoonFwiend.ModAssets;

namespace BawoonFwiend
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, BawoonBuildingConfig.ID);
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
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public static class Patch_ElementLoader_Load
        {
            public static void Postfix()
            {
                var uran = ElementLoader.GetElement(SimHashes.Hydrogen.CreateTag());
                if (uran.oreTags is null)
                {
                    uran.oreTags = new Tag[] { };
                }
                uran.oreTags = uran.oreTags.AddToArray(ModAssets.Tags.BalloonGas);

                var lead = ElementLoader.GetElement(SimHashes.Helium.CreateTag());
                if (lead.oreTags is null)
                {
                    lead.oreTags = new Tag[] { };
                }
                lead.oreTags = lead.oreTags.AddToArray(ModAssets.Tags.BalloonGas);
            }
        }

    }
}

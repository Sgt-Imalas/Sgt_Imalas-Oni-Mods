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
using static BuildingToken.ModAssets;

namespace BuildingToken
{
    internal class Patches
    {
        /// <summary>
        /// add tokens to recipes
        /// </summary>
        [HarmonyPatch(typeof(BuildingTemplates))]
        [HarmonyPatch(nameof(BuildingTemplates.CreateBuildingDef))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix(string id, ref float[] construction_mass, ref string[] construction_materials)
            {
                if (ModAssets.TokenBuildings.Contains(id))
                {
                    var constructionMassToModify = ((float[])construction_mass.Clone()).ToList();
                    constructionMassToModify.Add(1f);
                    construction_mass = constructionMassToModify.ToArray();

                    var constructionMaterialsToModify = ((string[])construction_materials.Clone()).ToList();
                    constructionMaterialsToModify.Add(BuildingTokenTags[id].ToString());
                    construction_materials = constructionMaterialsToModify.ToArray();                    
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
    }
}

using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class KilnConfig_Patches
    {

        [HarmonyPatch(typeof(KilnConfig), nameof(KilnConfig.ConfigureRecipes))]
        public class KilnConfig_ConfigureRecipes_Patch
        {
            public static void Postfix(KilnConfig __instance)
            {
                AdditionalRecipes.RegisterRecipes_Kiln();
            }
        }
    }
}

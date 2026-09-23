using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class ClothingFabricator_Patches
	{

        [HarmonyPatch(typeof(ClothingFabricatorConfig), nameof(ClothingFabricatorConfig.ConfigureRecipes))]
        public class ClothingFabricatorConfig_ConfigureRecipes_Patch
        {
            public static void Postfix(ClothingFabricatorConfig __instance)
            {
                ModRecipes.AdditionalRecipes_ClothingFabricator();
			}
        }
	}
}

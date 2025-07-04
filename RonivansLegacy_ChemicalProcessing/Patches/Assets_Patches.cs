using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Assets_Patches
    {

        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "icon_mining_occurence");
				InjectionMethods.AddSpriteToAssets(__instance, "dreamIcon_Processing_AIO_Ronivan");
			}
        }
    }
}

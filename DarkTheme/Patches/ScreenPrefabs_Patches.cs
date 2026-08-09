using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace DarkTheme.Patches
{
	internal class ScreenPrefabs_Patches
	{

        [HarmonyPatch(typeof(ScreenPrefabs), nameof(ScreenPrefabs.OnPrefabInit))]
        public class ScreenPrefabs_OnPrefabInit_Patch
        {
            public static void Postfix()
            {
                ModAssets.DarkenScreenPrefabs();
            }
        }
	}
}

using HarmonyLib;
using ShapeProjector.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace ShapeProjector.Patches
{
	internal class DetailsScreen_Patches
	{

        [HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.OnPrefabInit))]
        public class DetailsScreen_OnPrefabInit_Patch
        {
            public static void Postfix(DetailsScreen __instance)
			{
				UIUtils.AddCustomSideScreen<ShapeProjectorSideScreen>("SP_ShapePickerSidescreen", ModAssets.ShapePickerSidescreenPrefab);
			}
        }
	}
}

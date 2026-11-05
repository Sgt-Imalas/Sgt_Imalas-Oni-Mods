using HarmonyLib;
using ShapeProjector.Content.Defs.Buildings;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace ShapeProjector.Patches
{
	internal class Db_Patches
	{

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
			{
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.InteriorDecor, ShapeProjectorConfig.ID);
				ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, ShapeProjectorConfig.ID);
			}
        }
	}
}

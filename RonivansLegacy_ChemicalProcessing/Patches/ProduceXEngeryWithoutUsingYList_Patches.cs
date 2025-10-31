using Database;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomGenerators;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class ProduceXEngeryWithoutUsingYList_Patches
    {

        [HarmonyPatch(typeof(ProduceXEngeryWithoutUsingYList), MethodType.Constructor, [typeof(float),typeof(List<Tag>)])]
        public class ProduceXEngeryWithoutUsingYList_Constructor_Patch
		{
            public static void Postfix(ProduceXEngeryWithoutUsingYList __instance)
            {
                if(__instance.disallowedBuildings.Contains("Generator")) //checking for coal gen to ensure its the correct achievment (maybe someone reused it for a modded achievment?
                {
                    GeneratorList.AppendCombustionGenerators(ref __instance.disallowedBuildings);
					GeneratorList.AchievmentInstance = __instance;

				}
            }
        }
    }
}

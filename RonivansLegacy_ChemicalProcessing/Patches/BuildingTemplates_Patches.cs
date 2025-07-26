using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class BuildingTemplates_Patches
    {
        /// <summary>
        /// Using steel tag directly does not work, as it breaks recipe deliveries, causing steel-tagged materials to be delivered.
        /// using "hardened alloy" as a substitute and adding that to all recipes that want steel directly
        /// </summary>
        [HarmonyPatch(typeof(BuildingTemplates), nameof(BuildingTemplates.CreateBuildingDef))]
        public class BuildingTemplates_CreateBuildingDef_Patch
        {
            public static void Prefix(BuildingTemplates __instance, string[] construction_materials)
            {
				for (int i = 0; i < construction_materials.Length; i++)
				{
					if (construction_materials[i] == GameTags.Steel.ToString())
					{
						construction_materials[i] += "&"+ ModAssets.Tags.AIO_HardenedAlloy.ToString();
					}
				}
			}
        }
    }
}

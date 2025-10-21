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
            public static void Prefix(string[] construction_materials)
            {
                string hardenedAlloyTag = ModAssets.Tags.AIO_HardenedAlloy.ToString();
                string steelTag = GameTags.Steel.ToString();

                //if another mod does this:
                string steelChained = steelTag + "&";

				for (int i = 0; i < construction_materials.Length; i++)
				{
                    string mat = construction_materials[i];
					if (mat == steelTag || mat.Contains(steelChained))
					{
						construction_materials[i] += "&"+ hardenedAlloyTag;
					}
				}
			}
        }
    }
}

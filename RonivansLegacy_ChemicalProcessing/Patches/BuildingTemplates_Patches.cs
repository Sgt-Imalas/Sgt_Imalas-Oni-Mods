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
                string brickTag = SimHashes.Brick.CreateTag().ToString();
				string ceramicTag = SimHashes.Ceramic.CreateTag().ToString();
				AppendAlternative(ref construction_materials, ceramicTag, brickTag);

				string hardenedAlloyTag = ModAssets.Tags.AIO_HardenedAlloy.ToString();
				string steelTag = GameTags.Steel.ToString();
				AppendAlternative(ref construction_materials, steelTag, hardenedAlloyTag);
			}
            static void AppendAlternative(ref string[] construction_materials, string targetMaterialTag, string appendAdditionalTag)
			{
				//if another mod does this:
				string targetChained = targetMaterialTag + "&";

				for (int i = 0; i < construction_materials.Length; i++)
				{
					string mat = construction_materials[i];
					if (mat == targetMaterialTag || mat.Contains(targetChained))
					{
						construction_materials[i] += "&" + appendAdditionalTag;
					}
				}
			}
        }
    }
}

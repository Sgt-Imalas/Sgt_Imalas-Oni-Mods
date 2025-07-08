using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class BuildTool_Patches
	{
		//Credit: DecorPackA by aki
		// Change building def places by build tool based on what material is selected
		[HarmonyPatch(typeof(BuildTool), nameof(BuildTool.Activate), typeof(BuildingDef), typeof(IList<Tag>))]
		public static class BuildTool_Activate_Patch
		{
			public static void Prefix(BuildTool __instance, ref BuildingDef def, IList<Tag> selected_elements)
			{
				if (def.PrefabID == MonoElementTileConfig.DEFAULT_ID)
				{
					RemoveVisualizer(__instance);

					if (MonoElementTileConfig.TryGetBuildingVariant(selected_elements[0], out var buildingTag))
						def = Assets.GetBuildingDef(buildingTag.ToString());
				}
			}

			// this prevents ghost preview blocks from appearing
			private static void RemoveVisualizer(BuildTool __instance)
			{
				if (__instance.visualizer != null)
				{
					__instance.ClearTilePreview();
					UnityEngine.Object.Destroy(__instance.visualizer);
				}
			}
		}
	}
}

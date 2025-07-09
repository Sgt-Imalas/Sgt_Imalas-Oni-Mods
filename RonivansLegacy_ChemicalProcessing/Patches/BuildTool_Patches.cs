using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

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
				var selectedMaterial = selected_elements[0];

				if (MultivariantBuildings.HasMaterialVariant(def.Tag, selectedMaterial, out var targetForMaterial))
				{
					if (def.isKAnimTile)
						RemoveVisualizer(__instance);
					def = Assets.GetBuildingDef(targetForMaterial.ToString());

				}
				if(MultivariantBuildings.HasFacadeVariant(def.Tag, __instance.facadeID, out var targetForFacade))
				{
					if (def.isKAnimTile)
						RemoveVisualizer(__instance);
					def = Assets.GetBuildingDef(targetForFacade.ToString());
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

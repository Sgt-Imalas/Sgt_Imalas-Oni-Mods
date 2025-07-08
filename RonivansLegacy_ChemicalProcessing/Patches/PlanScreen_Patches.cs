using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class PlanScreen_Patches
	{
		// Makes the Copy Building button target default MonoElementTile in the build menu.
		[HarmonyPatch(typeof(PlanScreen), nameof(PlanScreen.OnClickCopyBuilding))]
		public static class PlanScreen_OnClickCopyBuilding_Patch
		{
			public static bool Prefix()
			{
				if (SelectTool.Instance?.selected == null)
					return true;

				if (SelectTool.Instance?.selected?.TryGetComponent(out Building building) ?? false)
				{
					if (MonoElementTileConfig.VariantIDs.Contains(building.Def.PrefabID))
					{
						OpenBuildMenu(building);
						return false;
					}
				}

				return true;
			}

			private static void OpenBuildMenu(Building building)
			{
				foreach (var planInfo in TUNING.BUILDINGS.PLANORDER)
				{
					foreach (var buildingAndSubCategory in planInfo.buildingAndSubcategoryData)
					{
						if (buildingAndSubCategory.Key == MonoElementTileConfig.DEFAULT_ID)
						{
							var defaultStainedDef = Assets.GetBuildingDef(MonoElementTileConfig.DEFAULT_ID);

							PlanScreen.Instance.OpenCategoryByName(HashCache.Get().Get(planInfo.category));
							var gameObject = PlanScreen.Instance.activeCategoryBuildingToggles[defaultStainedDef].gameObject;

							PlanScreen.Instance.OnSelectBuilding(gameObject, defaultStainedDef);

							if (PlanScreen.Instance.ProductInfoScreen == null)
								return;

							PlanScreen.Instance.ProductInfoScreen.materialSelectionPanel.SelectSourcesMaterials(building);

							return;
						}
					}
				}
			}
		}
	}
}

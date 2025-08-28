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
	class PlanScreen_Patches
	{
		// Makes the Copy Building button target default MonoElementTile in the build menu.
		[HarmonyPatch(typeof(PlanScreen), nameof(PlanScreen.OnClickCopyBuilding))]
		public static class PlanScreen_OnClickCopyBuilding_Patch
		{
			public static bool Prefix(PlanScreen __instance)
			{
				Building lastBuilding = null;

				if(!__instance.LastSelectedBuilding.IsNullOrDestroyed() && __instance.LastSelectedBuilding.gameObject.activeInHierarchy && (!__instance.lastSelectedBuilding.Def.DebugOnly || DebugHandler.InstantBuildMode))
				{
					if(__instance.LastSelectedBuilding.TryGetComponent(out Building building))
					{
						lastBuilding = building;
					}
				}
				else if (__instance.lastSelectedBuildingDef != null && (!__instance.lastSelectedBuildingDef.DebugOnly || DebugHandler.InstantBuildMode))
				{
					if(__instance.lastSelectedBuildingDef.BuildingComplete.TryGetComponent<Building>(out Building fromprefab))
					{
						lastBuilding = fromprefab;
					}
				}
				if (lastBuilding == null)
					return true;

				if (MultivariantBuildings.IsMaterialVariant(lastBuilding.Def.PrefabID, out var parent))
				{
					OpenBuildMenu(lastBuilding, parent, true, false);
					return false;
				}
				else if (MultivariantBuildings.IsFacadeVariant(lastBuilding.Def.PrefabID, out var parent2))
				{
					OpenBuildMenu(lastBuilding, parent2, false, true);
					return false;
				}

				return true;
			}

			private static void OpenBuildMenu(Building building, Tag parentBuilding, bool selectMaterial, bool SelectSkin)
			{
				foreach (var planInfo in TUNING.BUILDINGS.PLANORDER)
				{
					foreach (var buildingAndSubCategory in planInfo.buildingAndSubcategoryData)
					{
						if (buildingAndSubCategory.Key == parentBuilding)
						{
							var defaultStainedDef = Assets.GetBuildingDef(parentBuilding.ToString());
							PlanScreen.Instance.OpenCategoryByName(HashCache.Get().Get(planInfo.category));
							var gameObject = PlanScreen.Instance.activeCategoryBuildingToggles[defaultStainedDef].gameObject;

							string facade = null;
							if (SelectSkin && MultivariantBuildings.TryGetFacadeFromChild(building.Def.PrefabID, out string skin))
							{
								facade = skin;
							}
							PlanScreen.Instance.OnSelectBuilding(gameObject, defaultStainedDef, facade);

							if (PlanScreen.Instance.ProductInfoScreen == null)
								return;

							if(selectMaterial)
								PlanScreen.Instance.ProductInfoScreen.materialSelectionPanel.SelectSourcesMaterials(building);
						}
					}
				}
			}
		}

		[HarmonyPatch(typeof(FacadeSelectionPanel), nameof(FacadeSelectionPanel.RefreshTogglesForBuilding))]
		public class FacadeSelectionPanel_RefreshTogglesForBuildinge_Patch
		{
			public static void Prefix(FacadeSelectionPanel __instance, ref bool __state)
			{
				bool isSkinVariantBuilding = MultivariantBuildings.IsFacadeVariantParent(__instance.selectedBuildingDefID, out string defaultSKin);
				if (isSkinVariantBuilding) 
				{
					__state = true;
					if(__instance.SelectedFacade == "DEFAULT_FACADE")
						__instance.SelectedFacade = defaultSKin;
				}
			}
			public static void Postfix(FacadeSelectionPanel __instance, bool __state) 
			{
				if(__state)
					__instance.activeFacadeToggles["DEFAULT_FACADE"].gameObject.SetActive(false);
			}
		}
	}
}

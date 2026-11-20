using ComplexFabricatorRibbonController.Content.Scripts.Buildings;
using ComplexFabricatorRibbonController.Content.UI;
using Database;
using UnityEngine;
using UtilLibs;
using static STRINGS.ELEMENTS;

namespace ComplexFabricatorRibbonController
{
	internal class ModAssets
	{
		public static GameObject RecipeSelectionSidescreenGO; //This is required to keep the GO of RecipeSelectionSidescreen from getting GCed!
		public static KScreen RecipeSelectionSecondarySidescreen;
		public static GameObject SelectRecipeSecondarySidescreenGO;
		public const string NoRecipeSelection = "NoRecipeSelection";

		public static class StatusItems
		{
			public static StatusItem NotAttachedToFabricator, GreenBitNoRecipe;
			public static void RegisterStatusItems(BuildingStatusItems bsi)
			{
				NotAttachedToFabricator = bsi.CreateStatusItem(
						  "CFRC_NOTLINKEDTOHEAD",
						  STRINGS.BUILDING.STATUSITEMS.CFRC_NOTLINKEDTOHEAD.NAME,
						  STRINGS.BUILDING.STATUSITEMS.CFRC_NOTLINKEDTOHEAD.TOOLTIP,
						  "status_item_not_linked",
						  StatusItem.IconType.Custom,
						  NotificationType.BadMinor,
						  false,
						  OverlayModes.None.ID);

				GreenBitNoRecipe = bsi.CreateStatusItem(
						  "CFRC_GREENBITNORECIPE",
						  STRINGS.BUILDING.STATUSITEMS.CFRC_GREENBITNORECIPE.NAME,
						  STRINGS.BUILDING.STATUSITEMS.CFRC_GREENBITNORECIPE.TOOLTIP,
						  "status_item_no_filter_set",
						  StatusItem.IconType.Custom,
						  NotificationType.BadMinor,
						  false,
						  OverlayModes.None.ID)
					.TooltipCallback(ComplexFabricatorRecipeControlAttachment.GreenBitRecipeCallback);
			}
		}
		

		private static Tag _microchip_Buildable = null;

		public static Tag Microchip_Buildable
		{
			get
			{
				if (_microchip_Buildable == null)
				{
					_microchip_Buildable = TagManager.Create("CFRC_Microchip_Buildable");
				}
				return _microchip_Buildable;
			}

		}

		public static void LoadAssets()
		{
			SgtLogger.l("Loading UI from asset bundle");
			AssetBundle bundle = AssetUtils.LoadAssetBundle("complexfabricatorribbon_assets", platformSpecific: true);
			//AssetBundle bundle = AssetUtils.LoadAssetBundle("rocketryexpanded_ui_assets", platformSpecific: true);


			//var DupeTransferSecondarySideScreenWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/DockingTransferScreen.prefab");


			//SgtLogger.Assert("DupeTransferSecondarySideScreenWindowPrefab", DupeTransferSecondarySideScreenWindowPrefab);

			//GroupSelectionSidescreen = DupeTransferSecondarySideScreenWindowPrefab.AddComponent<KScreen>();
			//return;


			RecipeSelectionSidescreenGO = bundle.LoadAsset<GameObject>("Assets/UIs/RibbonSelectionSideScreen.prefab");
			SelectRecipeSecondarySidescreenGO = bundle.LoadAsset<GameObject>("Assets/UIs/ListSelection_SecondarySidescreen.prefab");
			SgtLogger.Assert("RecipeSelectionSidescreenGO", RecipeSelectionSidescreenGO);
			SgtLogger.Assert("SelectRecipeSecondarySidescreen", SelectRecipeSecondarySidescreenGO);

			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(RecipeSelectionSidescreenGO);
			TMPConverter.ReplaceAllText(SelectRecipeSecondarySidescreenGO);

			RecipeSelectionSecondarySidescreen = SelectRecipeSecondarySidescreenGO.AddComponent<RibbonRecipeController_SecondarySidescreen>();
		}
		public static string GetRecipeText(ComplexRecipe recipe, bool includeDesc)
		{
			if (recipe == null)
			{
				return STRINGS.UI.RFRC_NO_RECIPE;
			}
			string desc = recipe.description;
			string text = string.Empty; //recipe.GetUIName(false);

			string ingredients = string.Empty, products = string.Empty;


			foreach (var ingredient in recipe.ingredients)
			{
				if (ingredient.amount > 0 && ingredient.material != null)
				{
					ingredients += ingredient.material.ProperName();
					ingredients += " & ";
				}
			}
			ingredients = ingredients.Trim(['&', ' ']);
			foreach (var result in recipe.results)
			{
				if (result.amount > 0 && result.material != null)
				{
					products += result.material.ProperName();
					products += " & ";
				}
			}
			products = products.Trim(['&', ' ']);

			text = string.Format(global::STRINGS.UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH, ingredients, products);

			if (includeDesc && !string.IsNullOrEmpty(desc))
			{
				text += "\n\n" + desc;
			}

			return text;
		}
	}
}

using ComplexFabricatorRibbonController.UI;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ComplexFabricatorRibbonController
{
	internal class ModAssets
	{
		public static GameObject RecipeSelectionSidescreenGO; //This is required to keep the GO of RecipeSelectionSidescreen from getting GCed!
		public static KScreen RecipeSelectionSecondarySidescreen;
		public static GameObject SelectRecipeSecondarySidescreenGO;
		public const string NoRecipeSelection = "NoRecipeSelection";
		public static StatusItem NotAttachedToFabricator
		{
			get
			{
				if (_notAttachedToFabricator == null)
					RegisterStatusItems();
				return _notAttachedToFabricator;
			}
		}

		static StatusItem _notAttachedToFabricator;

		public static void RegisterStatusItems()
		{
			_notAttachedToFabricator = new StatusItem(
					  "CFRC_NotAttached",
					  STRINGS.BUILDING.STATUSITEMS.CFRC_NOTLINKEDTOHEAD.NAME,
					  STRINGS.BUILDING.STATUSITEMS.CFRC_NOTLINKEDTOHEAD.TOOLTIP, 
					  "status_item_not_linked", 
					  StatusItem.IconType.Custom, 
					  NotificationType.BadMinor, 
					  false,
					  OverlayModes.None.ID);
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
	}
}

using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InventoryOrganization;
using static UtilLibs.SupplyClosetUtils;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS.AIO_FACILITYDOOR.FACADES;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS.LOGICALERTLIGHT.FACADES;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS.SPACERWALL.FACADES;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.FACADES_STANDALONE;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Walls;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomReservoirs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{

	class SkinDatabase
	{
		static readonly string SubCategoryID = "RONIVAN_AIO_SKINS";

		internal static void AddSkins()
		{
			var kanim = Assets.GetAnim("filter_media_kanim");
			var uiSprite = Def.GetUISpriteFromMultiObjectAnim(kanim);

			SkinCollection.CategoryInit(InventoryPermitCategories.BUILDINGS, SubCategoryID, uiSprite, 1);

			SkinCollection.Create(FacilityDoorConfig.ID,SubCategoryID)
				.Skin("FacilityDoorWhite", FACILITYDOORWHITE.NAME, FACILITYDOORWHITE.DESC, "facility_door_white_kanim")
				.Skin("FacilityDoorYellow", FACILITYDOORYELLOW.NAME, FACILITYDOORYELLOW.DESC, "facility_door_yellow_kanim")
				.Skin("FacilityDoorRed", FACILITYDOORRED.NAME, FACILITYDOORRED.DESC, "facility_door_red_kanim");

			SkinCollection.Create(OilRefineryConfig.ID, SubCategoryID)
				.Skin("ChemicalProcessing_Refinery", PETROLEUMDISTILLERY.NAME, PETROLEUMDISTILLERY.DESC, "petroleum_distillery_kanim");


			SkinCollection.Create(LogicAlertLightConfig.ID, SubCategoryID)
				.Skin("AlertLightGreen", LOGICALERTLIGHTGREEN.NAME, LOGICALERTLIGHTGREEN.DESC, "alert_light_green_kanim")
				.Skin("AlertLightYellow", LOGICALERTLIGHTYELLOW.NAME, LOGICALERTLIGHTYELLOW.DESC, "alert_light_yellow_kanim")
				.Skin("AlertLightRed", LOGICALERTLIGHTRED.NAME, LOGICALERTLIGHTRED.DESC, "alert_light_red_kanim");

			LEDTint.AddSkinLightTint("AlertLightGreen", Color.green);
			LEDTint.AddSkinLightTint("AlertLightYellow", Color.yellow);
			LEDTint.AddSkinLightTint("AlertLightRed", Color.red);


			SkinCollection.Create(SpacerWallConfig.ID, SubCategoryID)
				.Skin("SpacerDanger", SPACERDANGER.NAME, SPACERDANGER.DESC, "spacer_danger_kanim")
				.Skin("SpacerDangerCorner", SPACERDANGERCORNER.NAME, SPACERDANGERCORNER.DESC, "spacer_danger_corner_kanim")
				.Skin("SpacerPanel", SPACERPANEL.NAME, SPACERPANEL.DESC, "spacer_panel_kanim");


			SkinCollection.Create(ExteriorWallConfig.ID, SubCategoryID)
				.Skin("BrickWall", EXTERIORWALL.BRICKWALL.NAME, EXTERIORWALL.BRICKWALL.DESC, "brick_wall_kanim")
				.Skin("WoodenDrywall", EXTERIORWALL.WOODENDRYWALL.NAME, EXTERIORWALL.WOODENDRYWALL.DESC, "wooden_walls_kanim")
				.Skin("WoodenDrywallB", EXTERIORWALL.WOODENDRYWALL_B.NAME, EXTERIORWALL.WOODENDRYWALL_B.DESC, "wooden_B_walls_kanim")
				;

			SkinCollection.Create(LadderConfig.ID, SubCategoryID)
				.Skin("WoodenLadder", WOODENLADDER.NAME, WOODENLADDER.DESC, "ladder_wooden_kanim")
				;
			SkinCollection.Create(SpacerWindowSmallConfig.ID, SubCategoryID)
				.Skin("SpacerWindow_B", SPACERWINDOWWALL.FACADES.SPACERWINDOW_B.NAME, SPACERWINDOWWALL.FACADES.SPACERWINDOW_B.DESC, "spacer_window_B_kanim");

			SkinCollection.Create(SmallGasReservoirConfig.ID, SubCategoryID)
				.Skin(SmallGasReservoirInvertedConfig.NORMAL, SMALLGASRESERVOIR.NAME, SMALLGASRESERVOIR.DESC, SmallGasReservoirInvertedConfig.KANIMNORMAL)
				.Skin(SmallGasReservoirInvertedConfig.INVERTED, INVERTEDSMALLGASRESERVOIR.NAME, INVERTEDSMALLGASRESERVOIR.DESC, SmallGasReservoirInvertedConfig.KANIMINVERTED);

			SkinCollection.RegisterAllSkins();
		}
	}
}

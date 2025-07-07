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
using UtilLibs;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS.LOGICALERTLIGHT.FACADES;

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
				.Skin("ChemicalProcessing_Refinery", STRINGS.BUILDINGS.FACADES_STANDALONE.PETROLEUMDISTILLERY.NAME, STRINGS.BUILDINGS.FACADES_STANDALONE.PETROLEUMDISTILLERY.DESC, "petroleum_distillery_kanim");


			SkinCollection.Create(LogicAlertLightConfig.ID, SubCategoryID)
				.Skin("AlertLightGreen", LOGICALERTLIGHTGREEN.NAME, LOGICALERTLIGHTGREEN.DESC, "alert_light_green_kanim")
				.Skin("AlertLightYellow", LOGICALERTLIGHTYELLOW.NAME, LOGICALERTLIGHTYELLOW.DESC, "alert_light_yellow_kanim")
				.Skin("AlertLightRed", LOGICALERTLIGHTRED.NAME, LOGICALERTLIGHTRED.DESC, "alert_light_red_kanim");

			LEDTint.AddSkinLightTint("AlertLightGreen", Color.green);
			LEDTint.AddSkinLightTint("AlertLightYellow", Color.yellow);
			LEDTint.AddSkinLightTint("AlertLightRed", Color.red);

			SkinCollection.RegisterAllSkins();
		}
	}
}

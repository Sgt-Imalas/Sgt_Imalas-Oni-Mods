using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InventoryOrganization;
using static UtilLibs.SupplyClosetUtils;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS.FACILITYDOOR.FACADES;

using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS;
using UtilLibs;

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

			SkinCollection.RegisterAllSkins();
		}
	}
}

using System.Collections.Generic;
using UtilLibs;
using static InventoryOrganization;
using static Rockets_TinyYetBig.STRINGS.BUILDINGS.PREFABS;
using static Rockets_TinyYetBig.STRINGS.UI.KLEI_INVENTORY_SCREEN;
using static UtilLibs.SupplyClosetUtils;

namespace Rockets_TinyYetBig.Content.ModDb
{

	class SkinDatabase
	{

		static readonly string SubCategoryID = "RTB_MODULE_SKINS";

		static readonly Dictionary<string, bool> SkinOverrideModuleWide = [];

		public static bool SkinOverridesWidth(string skinId, out bool isWide)
		{
			//SgtLogger.l("Checking Width override: " + skinId);
			return SkinOverrideModuleWide.TryGetValue(skinId, out isWide);
		}


		internal static void AddSkins()
		{
			;
			var uiSprite = Assets.GetSprite("rocket_landing");

			SkinCollection.CategoryInit(InventoryPermitCategories.BUILDINGS, SubCategoryID, uiSprite, 1);

			SkinCollection.Create(OxidizerTankLiquidClusterConfig.ID, SubCategoryID)
				.Skin("RTB_LOX_PINK", RTB_SKINS.LOX_PINK, RTB_SKINS.SKIN_GENERIC_DESC, "rocket_oxidizer_tank_liquid_pink_kanim")
				.Skin("RTB_LOX_GREEN", RTB_SKINS.LOX_GREEN, RTB_SKINS.SKIN_GENERIC_DESC, "rocket_oxidizer_tank_liquid_green_kanim")
				;

			string wideSolarPanel = "RTB_WIDE_SOLARPANELSKIN";
			SkinCollection.Create(SolarPanelModuleConfig.ID, SubCategoryID)
				.Skin(wideSolarPanel, RTB_SOLARPANELMODULE_WIDE.NAME, RTB_SOLARPANELMODULE_WIDE.DESC, "rocket_solar_panel_module_wide_kanim")
				;
			SkinOverrideModuleWide[wideSolarPanel] = true;

			SkinCollection.Create(LiquidFuelTankClusterConfig.ID, SubCategoryID)
				.Skin("RTB_FUELTANK_SCIFI", RTB_SKINS.FUEL_LARGE_SCIFI, RTB_SKINS.SKIN_FUTURISTIC_DESC, "rtb_rocket_fuel_tank_scifi_kanim")
				.Skin("RTB_FUELTANK_PINK", RTB_SKINS.FUEL_LARGE_PINK, RTB_SKINS.SKIN_GENERIC_DESC, "rocket_fuel_tank_pink_kanim")
				;


			SkinCollection.RegisterSkinInjectionPatch();
		}
	}
}

using UtilLibs;
using static InventoryOrganization;
using static STRINGS.CREATURES.SPECIES;
using static UtilLibs.SupplyClosetUtils;

namespace KleiPaintings
{

	class SkinDatabase
	{
		internal static void AddSkins()
		{
			string modPrefix = "KleiArtstreamPainting_";

			SkinCollection.Create(CanvasConfig.ID, PermitSubcategories.BUILDING_CANVAS_STANDARD, true)
				.MakeArtable(true, "canvas")
				.ArtSkin(modPrefix +"Puft", PUFT.NAME, PUFT.DESC, "painting_art_puft_kanim")
				.ArtSkin(modPrefix +"PuftPrince", PUFT.VARIANT_ALPHA.NAME, PUFT.VARIANT_ALPHA.DESC, "painting_art_puftprince_kanim")
				.ArtSkin(modPrefix +"PuftDense", PUFT.VARIANT_OXYLITE.NAME, PUFT.VARIANT_OXYLITE.DESC, "painting_art_puftdense_kanim")
				.ArtSkin(modPrefix +"PuftSqueaky", PUFT.VARIANT_BLEACHSTONE.NAME, PUFT.VARIANT_BLEACHSTONE.DESC, "painting_art_puftsqueaky_kanim")
				;

			SkinCollection.Create(CanvasTallConfig.ID, PermitSubcategories.BUILDING_CANVAS_PORTRAIT, true)
				.MakeArtable(true,"canvas")
				.ArtSkin(modPrefix + "IceBelly", ICEBELLY.NAME, ICEBELLY.DESC, "painting_tall_art_bammoth_kanim")
				.ArtSkin(modPrefix + "PufferFish", PUFFERFISH.NAME, PUFFERFISH.DESC, "painting_tall_art_blowter_kanim")
				.ArtSkin(modPrefix + "Dupe_Bubbles", GetDupeName("BUBBLES"), GetDupeDesc("BUBBLES"), "painting_tall_art_bubbles_kanim")
				.ArtSkin(modPrefix + "CuddlePip", SQUIRREL.VARIANT_HUG.NAME, SQUIRREL.VARIANT_HUG.DESC, "painting_tall_art_cuddlepip_kanim")
				.ArtSkin(modPrefix + "Dartle", CHAMELEON.NAME, CHAMELEON.DESC, "painting_tall_art_dartle_kanim")
				.ArtSkin(modPrefix + "Dupe_Edwiredo", GetDupeName("EDWIREDO"), GetDupeDesc("EDWIREDO"), "painting_tall_art_edwiredo_kanim")
				.ArtSkin(modPrefix + "Dupe_Freyja", GetDupeName("FREYJA"), GetDupeDesc("FREYJA"), "painting_tall_art_freyja_kanim")
				.ArtSkin(modPrefix + "Dupe_Higby", GetDupeName("HIGBY"), GetDupeDesc("HIGBY"), "painting_tall_art_higby_kanim")
				.ArtSkin(modPrefix + "Lumb", STEGO.NAME, STEGO.DESC, "painting_tall_art_lumb_kanim")
				.ArtSkin(modPrefix + "Morb", GLOM.NAME, GLOM.DESC, "painting_tall_art_morb_kanim")
				.ArtSkin(modPrefix + "Rhex", RAPTOR.NAME, RAPTOR.DESC, "painting_tall_art_rhex_kanim")
				.ArtSkin(modPrefix + "Satellite", STRINGS.UI.SPACEDESTINATIONS.DEBRIS.SATELLITE.NAME, STRINGS.UI.SPACEDESTINATIONS.DEBRIS.SATELLITE.DESCRIPTION, "painting_tall_art_satellite_kanim")
				.ArtSkin(modPrefix + "Seaquine", SEAHORSE.NAME, SEAHORSE.DESC, "painting_tall_art_seaquine_kanim")
				.ArtSkin(modPrefix + "Dupe_Sena", GetDupeName("Sena"), GetDupeDesc("Sena"), "painting_tall_art_sena_kanim")
				.ArtSkin(modPrefix + "SomniumSynthesizer", STRINGS.CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.NAME, STRINGS.CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.DESCRIPTION, "painting_tall_art_somniumsynth_kanim")
				.ArtSkin(modPrefix + "Dupe_Stinky", GetDupeName("Stinky"), GetDupeDesc("Stinky"), "painting_tall_art_stinky_kanim")
				;

			SkinCollection.RegisterSkinInjectionPatch();
		}
		static string GetDupeName(string dupeID)
		{
			string upper = dupeID.ToUpperInvariant();
			return Strings.Get($"STRINGS.DUPLICANTS.PERSONALITIES.{upper}.NAME");
		}
		static string GetDupeDesc(string dupeID)
		{
			string upper = dupeID.ToUpperInvariant();

			var dupeName = Strings.Get($"STRINGS.DUPLICANTS.PERSONALITIES.{upper}.NAME");
			var dupeDesc = Strings.Get($"STRINGS.DUPLICANTS.PERSONALITIES.{upper}.DESC");
			return string.Format(dupeDesc, dupeName);
		}
	}
}

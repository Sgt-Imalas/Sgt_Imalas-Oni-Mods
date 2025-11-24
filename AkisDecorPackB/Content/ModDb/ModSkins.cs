using AkisDecorPackB.Content.Defs.Buildings;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static Database.ArtableStatuses;
using static LogicGate.LogicGateDescriptions;

namespace AkisDecorPackB.Content.ModDb
{
	internal class ModSkins
	{
		public static readonly HashSet<string> hangables =
			[
				"DecorPackB_DecorPackB_GiantFossilDisplay_livayatan"
			];

		public static HashSet<string> fossils = new();
		public static HashSet<string> pots = new();
		public static HashSet<string> fountains = new();

		public static PermitCategory fossil = (PermitCategory)Hash.SDBMLower("DecorPackB_Fossil");

		public static HashSet<string> myFacades = [];
		public static HashSet<string> useMuseumDefs =
		[
			FossilDisplayConfig.ID,
			PotConfig.ID,
			// fountain
			// tile
		];

		public class SUB_CATEGORIES
		{
			public const string
				FOSSILS = "DECORPACKB_FOSSILS",
				POTS = "DECORPACKB_POTS",
				FOUNTAINS = "DECORPACKB_FOUNTAINS";
		}
		public static void RegisterArtables(ArtableStages stages)
		{
			// smaller fossil
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "human", "decorpackb_fossildisplay_human_kanim", ArtableStatusType.LookingUgly));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "doggy", "decorpackb_fossildisplay_doggy_kanim", ArtableStatusType.LookingUgly));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "pacu", "decorpackb_fossildisplay_pacu_kanim", ArtableStatusType.LookingOkay));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "ammonite", "decorpackb_fossildisplay_ammonite_kanim", ArtableStatusType.LookingOkay));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "pawprints", "decorpackb_fossildisplay_pawprints_kanim", ArtableStatusType.LookingOkay));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "ancientspecimenamber", "decorpackb_fossildisplay_ancientspecimentamber_kanim", ArtableStatusType.LookingOkay));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "dodo", "decorpackb_fossildisplay_dodo_kanim", ArtableStatusType.LookingGreat));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "beefalo", "decorpackb_fossildisplay_beefalo_kanim",  ArtableStatusType.LookingGreat));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "minipara", "decorpackb_fossildisplay_minipara_kanim", ArtableStatusType.LookingGreat));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "trilobite", "decorpackb_fossildisplay_trilobite_kanim", ArtableStatusType.LookingGreat));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "microraptor", "decorpackb_fossildisplay_microraptor_kanim", ArtableStatusType.LookingGreat));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "ancientspeciment", "decorpackb_fossildisplay_ancientspecimen_kanim",  ArtableStatusType.LookingGreat));
			fossils.Add(AddArtableStage(stages, FossilDisplayConfig.ID, "glommer", "decorpackb_fossildisplay_glommer_kanim",  ArtableStatusType.LookingGreat));

			// pot
			pots.Add(AddArtableStage(stages, PotConfig.ID, "hatch", "decorpackb_pot_hatch_kanim", ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "morb", "decorpackb_pot_morb_kanim",  ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "swirlies", "decorpackb_pot_swirlies_kanim",  ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "swirlies_purple", "decorpackb_pot_swirlies_purple_kanim",  ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "swirlies_bluegold", "decorpackb_pot_swirlies_bluegold_kanim",  ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "pinkyfluffylettuce", "decorpackb_pot_pinkyfluffylettuce_kanim", ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "angrylettuce", "decorpackb_pot_angrylettuce_kanim", ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "generic_tall", "decorpackb_pot_generic_tall_kanim", ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "muckroot", "decorpackb_pot_muckroot_kanim", ArtableStatusType.LookingGreat, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "rectangular", "decorpackb_pot_rectangular_kanim",  ArtableStatusType.LookingUgly, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "weird", "decorpackb_pot_weird_kanim",  ArtableStatusType.LookingOkay, "off"));
			pots.Add(AddArtableStage(stages, PotConfig.ID, "red", "decorpackb_pot_red_kanim", ArtableStatusType.LookingOkay, "off"));

			// fountains
			fountains.Add(AddArtableStage(stages, FountainConfig.ID, "fish", "decorpackb_fountain_fish_kanim",  ArtableStatusType.LookingGreat));
			fountains.Add(AddArtableStage(stages, FountainConfig.ID, "bowls", "decorpackb_fountain_bowls_kanim", ArtableStatusType.LookingGreat));
		}
		static string AddArtableStage(ArtableStages stages, string artableID, string stageID, string anim, ArtableStatusType statusType, string variant = "idle")
		{
			return ArtHelper.AddStatueStage(stages, artableID, stageID, Strings.Get($"STRINGS.BUILDINGS.PREFABS.{artableID.ToUpperInvariant()}.VARIANT.{stageID.ToUpperInvariant()}.NAME"), Strings.Get($"STRINGS.BUILDINGS.PREFABS.{artableID.ToUpperInvariant()}.VARIANT.{stageID.ToUpperInvariant()}.DESCRIPTION"), anim, statusType, variant);
		}


		public static void ConfigureSubCategories()
		{
			SupplyClosetUtils.AddOrGetSubCategory(
				SUB_CATEGORIES.FOSSILS,
				InventoryOrganization.InventoryPermitCategories.ARTWORK,
				Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim("decorpackb_fossildisplay_minipara_kanim")))
				.AddRange(fossils);

			SupplyClosetUtils.AddOrGetSubCategory(
				SUB_CATEGORIES.POTS,
				InventoryOrganization.InventoryPermitCategories.ARTWORK,
				Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim("decorpackb_pot_muckroot_kanim")))
				.AddRange(pots);

			SupplyClosetUtils.AddOrGetSubCategory(
				SUB_CATEGORIES.FOUNTAINS,
				InventoryOrganization.InventoryPermitCategories.ARTWORK,
				Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim("dp_fountain_kanim")))
				.AddRange(fountains);

		}
	}
}

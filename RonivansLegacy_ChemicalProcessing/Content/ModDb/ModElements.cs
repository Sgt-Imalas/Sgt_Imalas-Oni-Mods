using ElementUtilNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class ModElements
	{
		/// Chemical Processing Industrial overhaul
		public static readonly Color32 LOWGRADESAND_COLOR = new Color32(59, 46, 12, 255);
		public static readonly Color32 BASEGRADESAND_COLOR = new Color32(74, 66, 60, 255);
		public static readonly Color32 HIGHGRADESAND_COLOR = new Color32(130, 73, 92, 255);

		public static readonly Color32 AMMONIA_COLOR = new Color32(215, 227, 252, 255);
		public static readonly Color32 AMMONIUM_SALT_COLOR = new Color32(186, 245, 255, 255);
		public static readonly Color32 ARGENTITE_COLOR = new Color32(140, 131, 104, 255);
		public static readonly Color32 AURICHALCITE_COLOR = new Color32(162, 235, 187, 255);
		public static readonly Color32 BORAX_COLOR = new Color32(245, 241, 211, 255);
		public static readonly Color32 BRASS_COLOR = new Color32(250, 224, 152, 255);
		public static readonly Color32 CARBONFIBER_COLOR = new Color32(39, 39, 41, 255);
		public static readonly Color32 CHLOROSCHIST_COLOR = new Color32(168, 181, 123, 255);
		public static readonly Color32 CONCRETEBLOCK_COLOR = new Color32(117, 110, 99, 255);
		public static readonly Color32 FIBERGLASS_COLOR = new Color32(242, 242, 201, 255);
		public static readonly Color32 GALENA_COLOR = new Color32(225, 241, 242, 255);
		public static readonly Color32 ISOPROPANE_COLOR = new Color32(167, 151, 232, 255);
		public static readonly Color32 METEOR_COLOR = new Color32(57, 57, 66, 255);
		public static readonly Color32 NITRIC_COLOR = new Color32(255, 68, 0, 255);
		public static readonly Color32 SHALE_COLOR = new Color32(64, 60, 53, 255);
		public static readonly Color32 BRONZE_COLOR = new Color32(237, 70, 9, 255);
		public static readonly Color32 PLASTEEL_COLOR = new Color32(133, 255, 180, 255);
		public static readonly Color32 RAWGAS_COLOR = new Color32(48, 48, 19, 255);
		public static readonly Color32 SILVER_COLOR = new Color32(107, 117, 125, 255);
		public static readonly Color32 SLAG_COLOR = new Color32(102, 95, 79, 255);
		public static readonly Color32 MOLTENSLAG_COLOR = new Color32(148, 9, 9, 255);
		public static readonly Color32 SOURWATER_COLOR = new Color32(130, 104, 65, 255);
		public static readonly Color32 SULFURIC_COLOR = new Color32(252, 252, 3, 255);
		public static readonly Color32 TOXIC_COLOR = new Color32(130, 51, 5, 255);
		public static readonly Color32 ZINC_COLOR = new Color32(201, 201, 195, 255);
		//unburied from older version:
		public static readonly Color32 NITROGEN_COLOR = new Color32(205, 194, 255, 255);


		/// Chemical Processing BioChemistry
		public static readonly Color32 BIODIESEL_COLOR = new Color32(149, 245, 66, 255);
		public static readonly Color32 BIOMASS_COLOR = new Color32(148, 124, 58, 255);
		public static readonly Color32 BIOPLASTIC_COLOR = new Color32(201, 201, 195, 255);
		public static readonly Color32 VEGEOIL_COLOR = new Color32(201, 235, 52, 255);


		/// Chemical Processing Industrial overhaul
		public static ElementInfo
			//GradeSands
			LowGradeSand_Solid = ElementInfo.Solid("LowGradeSand", "solid_lowgradesand_kanim", LOWGRADESAND_COLOR),
			BaseGradeSand_Solid = ElementInfo.Solid("BaseGradeSand", "solid_basegradesand_kanim", BASEGRADESAND_COLOR),
			HighGradeSand_Solid = ElementInfo.Solid("HighGradeSand", "solid_highgradesand_kanim", HIGHGRADESAND_COLOR),
			//Ammonia
			Ammonia_Solid = ElementInfo.Solid("SolidAmmonia", "solid_ammonia_kanim", AMMONIA_COLOR),
			Ammonia_Liquid = ElementInfo.Liquid("LiquidAmmonia", AMMONIA_COLOR),
			Ammonia_Gas = ElementInfo.Gas("AmmoniaGas", AMMONIA_COLOR),
			//Ammonium Salt
			AmmoniumSalt_Solid = ElementInfo.Solid("AmmoniumSalt", "solid_ammonium_salt_kanim", AMMONIUM_SALT_COLOR),
			//Ammonium Water
			AmmoniumWater_Liquid = ElementInfo.Liquid("AmmoniumWater", AMMONIUM_SALT_COLOR),
			//silver ore
			Argentite_Solid = ElementInfo.Solid("ArgentiteOre", "raw_silverore_kanim", ARGENTITE_COLOR),
			//zinc ore
			Aurichalcite_Solid = ElementInfo.Solid("AurichalciteOre", "raw_zincore_kanim", AURICHALCITE_COLOR),
			Borax_Solid = ElementInfo.Solid("SolidBorax", "solid_borax_kanim", BORAX_COLOR),
			Brass_Solid = ElementInfo.Solid("SolidBrass", "solid_brass_kanim", BRASS_COLOR),
			//carbon composite
			CarbonFiber_Solid = ElementInfo.Solid("CarbonFiber", "carbonfibre_new_kanim", CARBONFIBER_COLOR),
			Chloroschist_Solid = ElementInfo.Solid("Chloroschist", "raw_chloroschist_kanim", CHLOROSCHIST_COLOR),
			ConcreteBlock_Solid = ElementInfo.Solid("ConcreteBlock", "slabs_new_kanim", CONCRETEBLOCK_COLOR),
			FiberGlass_Solid = ElementInfo.Solid("SolidFiberglass", "solid_fiberglass_kanim", FIBERGLASS_COLOR),
			Galena_Solid = ElementInfo.Solid("Galena", "raw_galena_kanim", GALENA_COLOR),
			Isopropane_Gas = ElementInfo.Gas("IsopropaneGas", ISOPROPANE_COLOR),
			MeteorOre_Solid = ElementInfo.Solid("MeteorOre", "raw_meteor_ore_kanim", METEOR_COLOR),
			//Nitric Acid
			NitricAcid_Liquid = ElementInfo.Liquid("LiquidNitric", NITRIC_COLOR),
			OilShale_Solid = ElementInfo.Solid("SolidOilShale", "solid_oilshale_kanim", SHALE_COLOR),
			PhosphorBronze = ElementInfo.Solid("PhosphorBronze", "solid_phosphor_bronze_kanim", BRONZE_COLOR),
			Plasteel_Solid = ElementInfo.Solid("Plasteel", "solid_plasteel_kanim", PLASTEEL_COLOR),
			RawNaturalGas_Gas = ElementInfo.Gas("RawNaturalGas", RAWGAS_COLOR),
			//Silver
			Silver_Solid = ElementInfo.Solid("SolidSilver", "solid_silver_kanim", SILVER_COLOR),
			Silver_Liquid = ElementInfo.Liquid("MoltenSilver", SILVER_COLOR),
			Silver_Gas = ElementInfo.Gas("SilverGas", SILVER_COLOR),
			//Slag
			Slag_Solid = ElementInfo.Solid("SolidSlag", "solid_slag_kanim", SLAG_COLOR),
			Slag_Liquid = ElementInfo.Liquid("MoltenSlag", MOLTENSLAG_COLOR),

			SourWater_Liquid = ElementInfo.Liquid("SourWater", SOURWATER_COLOR),

			SulphuricAcid_Liquid = ElementInfo.Liquid("LiquidSulfuric", SULFURIC_COLOR),
			SulphuricAcid_Gas = ElementInfo.Gas("SulfuricGas", SULFURIC_COLOR),

			ToxicMix_Solid = ElementInfo.Solid("ToxicClay", "solid_toxic_mud_kanim", TOXIC_COLOR),
			ToxicMix_Liquid = ElementInfo.Liquid("ToxicSlurry", TOXIC_COLOR),
			ToxicMix_Gas = ElementInfo.Gas("ToxicGas", TOXIC_COLOR),

			Zinc_Solid = ElementInfo.Solid("SolidZinc", "solid_zinc_kanim", ZINC_COLOR),
			Zinc_Liquid = ElementInfo.Liquid("MoltenZinc", ZINC_COLOR),
			Zinc_Gas = ElementInfo.Gas("ZincGas", ZINC_COLOR),

			//unburied from older version:
			Nitrogen_Solid = ElementInfo.Solid("SolidNitrogen", "solid_nitrogen_kanim", NITROGEN_COLOR),
			Nitrogen_Liquid = ElementInfo.Liquid("LiquidNitrogen", NITROGEN_COLOR),
			Nitrogen_Gas = ElementInfo.Gas("NitrogenGas", NITROGEN_COLOR)
			;



		//Chemical Processing Bio Chemistry
		public static ElementInfo
			BioDiesel_Solid = ElementInfo.Solid("SolidBiodiesel", "solid_biodiesel_kanim", BIODIESEL_COLOR),
			BioDiesel_Liquid = ElementInfo.Liquid("LiquidBiodiesel", BIODIESEL_COLOR),

			BioMass_Solid = ElementInfo.Solid("SolidBiomass", "biomass_kanim", BIOMASS_COLOR),

			BioPlastic_Solid = ElementInfo.Solid("Bioplastic", "bioplastic_kanim", BIOPLASTIC_COLOR),

			VegetableOil_Solid = ElementInfo.Solid("SolidVegeOil", "solid_veg_oil_kanim", VEGEOIL_COLOR),
			VegetableOil_Liquid = ElementInfo.Liquid("LiquidVegeOil", VEGEOIL_COLOR),
			VegetableOil_Gas = ElementInfo.Gas("VegeOilGas", VEGEOIL_COLOR)
			;
		public static void RegisterSubstances(List<Substance> list)
		{
			var ChemicalProcessing_IO_Elements = new HashSet<Substance>()
			{
				BaseGradeSand_Solid.CreateSubstanceFromElementTinted(SimHashes.FoolsGold),
				HighGradeSand_Solid.CreateSubstanceFromElementTinted(SimHashes.Steel),
				LowGradeSand_Solid.CreateSubstanceFromElementTinted(SimHashes.Cuprite),

				Ammonia_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidOxygen),
				Ammonia_Liquid.CreateSubstance(),
				Ammonia_Gas.CreateSubstance(),

				AmmoniumSalt_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidOxygen),
				AmmoniumWater_Liquid.CreateSubstance(),

				Argentite_Solid.CreateSubstanceFromElementTinted(SimHashes.Aluminum),
				Aurichalcite_Solid.CreateSubstanceFromElementTinted(SimHashes.Lead),
				Borax_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidCarbonDioxide),
				Brass_Solid.CreateSubstanceFromElementTinted(SimHashes.Aluminum),
				CarbonFiber_Solid.CreateSubstanceFromElementTinted(SimHashes.Algae),
				Chloroschist_Solid.CreateSubstanceFromElementTinted(SimHashes.Fullerene),
				ConcreteBlock_Solid.CreateSubstanceFromElementTinted(SimHashes.Aerogel),
				FiberGlass_Solid.CreateSubstanceFromElementTinted(SimHashes.Fullerene),
				Galena_Solid.CreateSubstanceFromElementTinted(SimHashes.Rust),
				Isopropane_Gas.CreateSubstance(),
				Galena_Solid.CreateSubstanceFromElementTinted(SimHashes.Rust),
				MeteorOre_Solid.CreateSubstanceFromElementTinted(SimHashes.CrushedRock),
				NitricAcid_Liquid.CreateSubstance(),
				OilShale_Solid.CreateSubstanceFromElementTinted(SimHashes.MaficRock),
				PhosphorBronze.CreateSubstanceFromElementTinted(SimHashes.FoolsGold),
				Plasteel_Solid.CreateSubstanceFromElementTinted(SimHashes.Aluminum),
				RawNaturalGas_Gas.CreateSubstance(),

				Silver_Solid.CreateSubstanceFromElementTinted(SimHashes.Electrum),
				Silver_Liquid.CreateSubstance(),
				Silver_Gas.CreateSubstance(),

				Slag_Solid.CreateSubstanceFromElementTinted(SimHashes.Regolith),
				Slag_Liquid.CreateSubstance(),

				SourWater_Liquid.CreateSubstance(),

				SulphuricAcid_Liquid.CreateSubstance(),
				SulphuricAcid_Gas.CreateSubstance(),

				ToxicMix_Solid.CreateSubstanceFromElementTinted(SimHashes.Clay),
				ToxicMix_Liquid.CreateSubstance(),
				ToxicMix_Gas.CreateSubstance(),

				Zinc_Solid.CreateSubstanceFromElementTinted(SimHashes.Gold),
				Zinc_Liquid.CreateSubstance(),
				Zinc_Gas.CreateSubstance(),

				//unburied from older version:
				Nitrogen_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidHydrogen),
				Nitrogen_Liquid.CreateSubstance(),
				Nitrogen_Gas.CreateSubstance(),
			};


			var ChemicalProcessing_BioChem_Elements = new HashSet<Substance>()
			{
				BioDiesel_Solid.CreateSubstanceFromElementTinted(SimHashes.SuperInsulator),
				BioDiesel_Liquid.CreateSubstance(),

				BioMass_Solid.CreateSubstanceFromElementTinted(SimHashes.Algae),

				BioPlastic_Solid.CreateSubstanceFromElementTinted(SimHashes.Polypropylene),

				VegetableOil_Solid.CreateSubstanceFromElementTinted(SimHashes.Isoresin),
				VegetableOil_Liquid.CreateSubstance(),
				VegetableOil_Gas.CreateSubstance(),
			};

			list.AddRange(ChemicalProcessing_IO_Elements);
			list.AddRange(ChemicalProcessing_BioChem_Elements);

		}
		public static void OverrideDebrisAnims()
		{
			///Regolith
			Substance regolith_substance = ElementLoader.FindElementByHash(SimHashes.Regolith).substance;
			KAnimFile regolith_anim = Assets.GetAnim("regolith_new_kanim");
			if (regolith_anim != null)
			{
				regolith_substance.anim = regolith_anim;
			}
			else
			{
				Debug.LogError("KAnimFile not found");
			}
			///Radium
			Substance radium_substance = ElementLoader.FindElementByHash(SimHashes.Radium).substance;
			KAnimFile radium_anim = Assets.GetAnim("solid_radium_kanim");
			if (regolith_anim != null)
			{
				radium_substance.anim = radium_anim;
			}
			else
			{
				Debug.LogError("KAnimFile not found");
			}

			///Yellowcake
			Substance yellowcake_substance = ElementLoader.FindElementByHash(SimHashes.Yellowcake).substance;
			Material material = new Material(ElementLoader.FindElementByHash(SimHashes.Sulfur).substance.material)
			{
				name = "matYellowcake",
				mainTexture = Assets.GetAnim("new_yellowcake_kanim").textureList[0]
			};
			yellowcake_substance.material = material;
			KAnimFile yellowcake_anim = Assets.GetAnim("solid_yellowcake_kanim");
			if (yellowcake_anim != null)
			{
				yellowcake_substance.anim = yellowcake_anim;
			}
			else
			{
				Debug.LogError("KAnimFile not found");
			}
		}

		internal static void ModifyExistingElements()
		{
			//=[ SYNGAS ENABLING PATCH ]===============================================
			Element syngas_material = ElementLoader.FindElementByHash(SimHashes.Syngas);
			syngas_material.oreTags = syngas_material.oreTags.Append(GameTags.CombustibleGas);
			syngas_material.disabled = false;

			//=[ PROPANE PATCH ]=======================================================
			Element propane_material = ElementLoader.FindElementByHash(SimHashes.Propane);
			propane_material.oreTags = syngas_material.oreTags.Append(GameTags.CombustibleGas);
			propane_material.disabled = false;

			//=[ NAPHTHA PATCH ]=======================================================
			Element naphtha_material = ElementLoader.FindElementByHash(SimHashes.Naphtha);
			naphtha_material.oreTags = naphtha_material.oreTags.Append(GameTags.CombustibleLiquid);

			//=[ ENABLING ELECTRUM ]===================================================
			Element electrum_material = ElementLoader.FindElementByHash(SimHashes.Electrum);
			electrum_material.highTempTransitionOreID = ModElements.Silver_Solid;
			electrum_material.highTempTransitionOreMassConversion = 0.6f;
			electrum_material.disabled = false;

			//=[ BITUMEN PATCH ]=======================================================
			Element bitumen_material = ElementLoader.FindElementByHash(SimHashes.Bitumen);
			bitumen_material.materialCategory = GameTags.ManufacturedMaterial;
			bitumen_material.oreTags = bitumen_material.oreTags.Append(GameTags.ManufacturedMaterial);
			bitumen_material.disabled = false;

			//=[ CRUSHED ROCK PATCH ]====================================================
			Element crushedRock_material = ElementLoader.FindElementByHash(SimHashes.CrushedRock);
			crushedRock_material.disabled = false;

			//=[ PHOSPHATE NODULES PATCH ]================================================
			Element phosphate_material = ElementLoader.FindElementByHash(SimHashes.PhosphateNodules);
			phosphate_material.oreTags = phosphate_material.oreTags.Append(GameTags.ConsumableOre);
			phosphate_material.disabled = false;


			//=[ MAFIC ROCK PATCH ]==========================================================
			Element mafic_material = ElementLoader.FindElementByHash(SimHashes.MaficRock);
			mafic_material.oreTags = mafic_material.oreTags.Append(GameTags.Crushable);

			//=[ PHYTO OIL PATCH ]==========================================================
			Element phytooil_material = ElementLoader.FindElementByHash(SimHashes.PhytoOil);
			phytooil_material.oreTags = phytooil_material.oreTags.Append(ModAssets.Tags.BioOil_Composition);


			//=[ BIODIESEL PATCH ]==========================================================
			Element biodiesel_material = ElementLoader.FindElementByHash(SimHashes.RefinedLipid);
			biodiesel_material.oreTags = biodiesel_material.oreTags.Append(ModAssets.Tags.Biodiesel_Composition);


			//=[ ENABLING RADIUM ]===================================================
			var radium = ElementLoader.FindElementByHash(SimHashes.Radium);
			if (radium != null)
			{
				radium.disabled = DlcManager.IsPureVanilla();
				radium.oreTags = [GameTags.ConsumableOre];
			}
			//=[ ENABLING YellowCake ]===================================================
			var yellowcake = ElementLoader.FindElementByHash(SimHashes.Yellowcake);
			if (yellowcake != null)
			{
				yellowcake.disabled = DlcManager.IsPureVanilla();
				yellowcake.oreTags = [GameTags.ManufacturedMaterial];
			}
			//=[ ENABLING Cement ]===================================================
			ElementLoader.FindElementByHash(SimHashes.Cement).disabled = false;

		}
	}
}

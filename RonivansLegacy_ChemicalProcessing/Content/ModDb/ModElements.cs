using ElementUtilNamespace;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.ElementUtilNamespace;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class ModElements
	{
		public static readonly float VeggiOilToWaterRatio = 0.15f;

		public static ElementGrouping PlasticGroup;

		public static string SteelAndTungstenMaterial => GameTags.Steel + "&" + SimHashes.Tungsten;

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
		///Permendur, alloy of cobalt and iron
		public static readonly Color32 PERMENDUR_COLOR = UIUtils.rgb(140, 207, 255);

		//chromium color
		public static readonly Color32 CHROMIUM_COLOR = UIUtils.rgb(223, 236, 247);
		//chrome ore
		public static readonly Color32 CHROMITE_COLOR = UIUtils.rgb(87, 89, 83);
		//Ferrochrome color
		public static readonly Color32 FERROCHROME_COLOR = UIUtils.rgb(255, 167, 156);
		public static readonly Color32 INVAR_COLOR = UIUtils.rgb(248, 243, 185);
		public static readonly Color32 STAINLESSSTEEL_COLOR = UIUtils.rgb(213, 216, 221);


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
			Nitrogen_Gas = ElementInfo.Gas("NitrogenGas", NITROGEN_COLOR),

			///Permendur, alloy of cobalt and iron, added as an alloy for swampy starts
			Permendur_Solid = ElementInfo.Solid("AIO_Permendur_Solid", "solid_permendur_kanim", PERMENDUR_COLOR),

			///Chromite ore, for Ferrochrome production
			ChromiteOre_Solid = ElementInfo.Solid("AIO_ChromiteOre_Solid", "solid_chromite_kanim", CHROMITE_COLOR),
			///Ferrochrome, alloy of iron and chromium, used in stainless steel production, acts as an alloy on its own already
			FerroChrome_Solid = ElementInfo.Solid("AIO_FerroChrome_Solid", "solid_ferrochrome_kanim", FERROCHROME_COLOR),
			FerroChrome_Liquid = ElementInfo.Liquid("AIO_FerroChrome_Liquid", FERROCHROME_COLOR),
			///Chromium, fallback element if someone is wild enough to boil molten ferrochrome
			Chromium_Solid = ElementInfo.Solid("AIO_Chromium_Solid", "solid_chromium_kanim", CHROMIUM_COLOR),
			Chromium_Liquid = ElementInfo.Liquid("AIO_Chromium_Liquid", CHROMIUM_COLOR),
			Chromium_Gas = ElementInfo.Gas("AIO_Chromium_Gas", CHROMIUM_COLOR),
			///Stainless steel
			StainlessSteel_Solid = ElementInfo.Solid("AIO_StainlessSteel_Solid", "solid_stainless_steel_kanim", STAINLESSSTEEL_COLOR),
			///alloy of nickel and iron
			Invar_Solid = ElementInfo.Solid("AIO_Invar_Solid", "solid_invar_kanim", INVAR_COLOR)
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

		static void SetAtmosphere(SimHashes element, Rottable.RotAtmosphereQuality quality)
		{
			Rottable.AtmosphereModifier.Add((int)element, quality);
		}

		public static HashSet<Substance> ChemicalProcessing_IO_Elements = [], ChemicalProcessing_BioChem_Elements = [];

		public static void RegisterSubstances(List<Substance> list)
		{
			var oreMaterial = list.Find(e => e.elementID == SimHashes.AluminumOre).material;
			ChemicalProcessing_IO_Elements = new HashSet<Substance>()
			{
				BaseGradeSand_Solid.CreateSubstanceFromElementTinted(SimHashes.FoolsGold),
				HighGradeSand_Solid.CreateSubstanceFromElementTinted(SimHashes.Steel),
				LowGradeSand_Solid.CreateSubstanceFromElementTinted(SimHashes.Cuprite),

				Ammonia_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidOxygen),
				Ammonia_Liquid.CreateSubstance(),
				Ammonia_Gas.CreateSubstance(),

				AmmoniumSalt_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidOxygen),
				AmmoniumWater_Liquid.CreateSubstance(),

				Argentite_Solid.CreateSubstanceFromElementTinted(SimHashes.Electrum, SILVER_COLOR),
				//Aurichalcite_Solid.CreateSubstanceFromElementTinted(SimHashes.Lead),
				Aurichalcite_Solid.CreateSubstance(true,oreMaterial),
				Borax_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidCarbonDioxide),
				Brass_Solid.CreateSubstanceFromElementTinted(SimHashes.Aluminum),
				CarbonFiber_Solid.CreateSubstanceFromElementTinted(SimHashes.Algae),
				Chloroschist_Solid.CreateSubstanceFromElementTinted(SimHashes.Fullerene),
				ConcreteBlock_Solid.CreateSubstanceFromElementTinted(SimHashes.Aerogel),
				FiberGlass_Solid.CreateSubstanceFromElementTinted(SimHashes.Fullerene),
				Isopropane_Gas.CreateSubstance(),
				Galena_Solid.CreateSubstanceFromElementTinted(SimHashes.Rust),
				MeteorOre_Solid.CreateSubstanceFromElementTinted(SimHashes.CrushedRock),
				NitricAcid_Liquid.CreateSubstance(),
				OilShale_Solid.CreateSubstanceFromElementTinted(SimHashes.SolidCrudeOil),
				PhosphorBronze.CreateSubstanceFromElementTinted(SimHashes.FoolsGold),
				Plasteel_Solid.CreateSubstanceFromElementTinted(SimHashes.Aluminum),

				Permendur_Solid.CreateSubstanceFromElementTinted(SimHashes.Aluminum),
				Invar_Solid.CreateSubstanceFromElementTinted(SimHashes.Iron),
				StainlessSteel_Solid.CreateSubstanceFromElementTinted(SimHashes.Steel),

				ChromiteOre_Solid.CreateSubstanceFromElementTinted(SimHashes.IronOre),
				FerroChrome_Solid.CreateSubstanceFromElementTinted(SimHashes.Iron),
				FerroChrome_Liquid.CreateSubstance(),
				Chromium_Solid.CreateSubstanceFromElementTinted(SimHashes.Steel),
				Chromium_Liquid.CreateSubstance(),
				Chromium_Gas.CreateSubstance(),


				RawNaturalGas_Gas.CreateSubstance(),

				Silver_Solid.CreateSubstanceFromElementTinted(SimHashes.Gold),
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

			ChemicalProcessing_BioChem_Elements = new HashSet<Substance>()
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

			SetElementRottables();
		}

		static void SetElementRottables()
		{
			SetAtmosphere(Nitrogen_Liquid, Rottable.RotAtmosphereQuality.Sterilizing);
			SetAtmosphere(Nitrogen_Gas, Rottable.RotAtmosphereQuality.Sterilizing);

			SetAtmosphere(ToxicMix_Gas, Rottable.RotAtmosphereQuality.Contaminating);
			SetAtmosphere(ToxicMix_Liquid, Rottable.RotAtmosphereQuality.Contaminating);

			SetAtmosphere(SulphuricAcid_Gas, Rottable.RotAtmosphereQuality.Sterilizing);
			SetAtmosphere(SulphuricAcid_Liquid, Rottable.RotAtmosphereQuality.Sterilizing);
			SetAtmosphere(NitricAcid_Liquid, Rottable.RotAtmosphereQuality.Sterilizing);

			SetAtmosphere(Ammonia_Gas, Rottable.RotAtmosphereQuality.Sterilizing);
			SetAtmosphere(Ammonia_Liquid, Rottable.RotAtmosphereQuality.Sterilizing);
			SetAtmosphere(AmmoniumWater_Liquid, Rottable.RotAtmosphereQuality.Sterilizing);

			SetAtmosphere(Isopropane_Gas, Rottable.RotAtmosphereQuality.Sterilizing);

			SetAtmosphere(RawNaturalGas_Gas, Rottable.RotAtmosphereQuality.Contaminating);

		}

		public static void OverrideDebrisAnims()
		{
			///Regolith
			Substance regolith_substance = ElementLoader.FindElementByHash(SimHashes.Regolith)?.substance;
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
			Substance radium_substance = ElementLoader.FindElementByHash(SimHashes.Radium)?.substance;
			if (radium_substance != null)
			{
				KAnimFile radium_anim = Assets.GetAnim("solid_radium_kanim");
				if (regolith_anim != null)
				{
					radium_substance.anim = radium_anim;
				}
				else
				{
					Debug.LogError("KAnimFile not found");
				}
			}

			///Yellowcake
			Substance yellowcake_substance = ElementLoader.FindElementByHash(SimHashes.Yellowcake)?.substance;
			Material material_yellowcake = new Material(ElementLoader.FindElementByHash(SimHashes.Sulfur).substance.material)
			{
				name = "matYellowcake",
				mainTexture = Assets.GetAnim("new_yellowcake_kanim").textureList[0]
			};
			if (yellowcake_substance != null)
			{
				yellowcake_substance.material = material_yellowcake;
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

			///Cement
			Substance cement_substance = ElementLoader.FindElementByHash(SimHashes.Cement)?.substance;
			Material material_cement = new Material(ElementLoader.FindElementByHash(SimHashes.Cement)?.substance.material)
			{
				name = "matCement",
				mainTexture = Assets.GetAnim("new_cement_kanim").textureList[0]
			};
			cement_substance.material = material_cement;
			KAnimFile cement_anim = Assets.GetAnim("solid_cement_kanim");
			if (cement_anim != null)
			{
				cement_substance.anim = cement_anim;
			}
			else
			{
				Debug.LogError("KAnimFile not found");
			}
			///Brick
			Substance brick_substance = ElementLoader.FindElementByHash(SimHashes.Brick)?.substance;
			Material material_brick = new Material(ElementLoader.FindElementByHash(SimHashes.Brick)?.substance.material)
			{
				name = "matBrick",
				mainTexture = Assets.GetAnim("new_brick_kanim").textureList[0]
			};
			if (brick_substance != null)
				brick_substance.material = material_brick;

			KAnimFile brick_anim = Assets.GetAnim("solid_brick_kanim");
			if (brick_anim != null)
			{
				brick_substance.anim = brick_anim;
			}
			else
			{
				Debug.LogError("KAnimFile not found");
			}
		}

		public static void ClearReenabledVanillaElementCodexTags(ref List<ElementLoader.ElementEntry> elementList)
		{
			HashSet<string> ToUnhide = [];


			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				UnhideElement(SimHashes.Syngas);
				UnhideElement(SimHashes.MoltenSyngas);
				UnhideElement(SimHashes.SolidSyngas);

				UnhideElement(SimHashes.Propane);
				UnhideElement(SimHashes.LiquidPropane);
				UnhideElement(SimHashes.SolidPropane);

				UnhideElement(SimHashes.Electrum);
				UnhideElement(SimHashes.Bitumen);
				UnhideElement(SimHashes.PhosphateNodules);
			}
			if (Config.Instance.DupesEngineering_Enabled || Config.Instance.DupesMachinery_Enabled || Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				UnhideElement(SimHashes.CrushedRock);
			}
			if (DlcManager.IsExpansion1Active() && Config.Instance.NuclearProcessing_Enabled)
			{
				UnhideElement(SimHashes.Radium);
				UnhideElement(SimHashes.Yellowcake);
			}
			if (Config.Instance.DupesEngineering_Enabled)
			{
				UnhideElement(SimHashes.Brick);
				UnhideElement(SimHashes.Cement);
			}
			HashSet<string> toHide = [];

			if (!Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				foreach (var element in ChemicalProcessing_BioChem_Elements)
				{
					toHide.Add(element.elementID.ToString());
				}
			}
			if (!Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				foreach (var element in ChemicalProcessing_IO_Elements)
				{
					toHide.Add(element.elementID.ToString());
				}
			}
			foreach (var element in elementList)
			{
				if (ToUnhide.Contains(element.elementId) && element.tags != null)
				{
					var oreTags = element.tags.ToList();
					oreTags.Remove(GameTags.HideFromCodex.ToString());
					oreTags.Remove(GameTags.HideFromSpawnTool.ToString());
					element.tags = oreTags.ToArray();
				}
				if (toHide.Contains(element.elementId))
				{
					var oreTags = element.tags?.ToList() ?? [];
					oreTags.Add(GameTags.HideFromCodex.ToString());
					oreTags.Add(GameTags.HideFromSpawnTool.ToString());
					element.tags = oreTags.ToArray();
				}
			}
			void UnhideElement(SimHashes element)
			{
				ToUnhide.Add(element.ToString());
			}

		}

		static void FixCachedStateTransitions()
		{
			var isopropane = ElementLoader.FindElementByHash(Isopropane_Gas);
			if (isopropane != null)
				isopropane.highTempTransition = ElementLoader.FindElementByHash(SimHashes.Propane);
		}

		static Dictionary<SimHashes, bool> CachedModElements = [];
		public static bool IsModElement(SimHashes element)
		{
			if (CachedModElements.TryGetValue(element, out var isModElement))
				return isModElement;

			bool modElement = false;
			if (ChemicalProcessing_IO_Elements.Any(s => s.elementID == element))
				modElement = true;
			else if (ChemicalProcessing_BioChem_Elements.Any(s => s.elementID == element))
				modElement = true;

			CachedModElements.Add(element, modElement);
			return modElement;
		}

		internal static void ModifyExistingElements()
		{
			PlasticGroup = ElementGrouping.GroupAllWith(GameTags.Plastic);

			if (Config.Instance.MineralProcessing_Metallurgy_Enabled)
			{
				AddTagToElementAndEnable(SimHashes.Hydrogen, ModAssets.Tags.AIO_CarrierGas);
				AddTagToElementAndEnable(SimHashes.Helium, ModAssets.Tags.AIO_CarrierGas);
			}

			///needs to always be active or fullerene page crashes
			FixCachedStateTransitions();

			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				//=[ SYNGAS ENABLING PATCH ]===============================================
				AddTagToElementAndEnable(SimHashes.Syngas, GameTags.CombustibleGas);
				AddTagToElementAndEnable(SimHashes.MoltenSyngas, GameTags.CombustibleLiquid);

				//=[ PROPANE PATCH ]=======================================================
				AddTagToElementAndEnable(SimHashes.Propane, GameTags.CombustibleGas);
				AddTagToElementAndEnable(SimHashes.LiquidPropane);
				AddTagToElementAndEnable(SimHashes.SolidPropane);


				////=[ NAPHTHA PATCH ]=======================================================
				///disabled, its kinda op
				//AddTagToElementAndEnable(SimHashes.Naphtha, GameTags.CombustibleLiquid);

				//=[ ENABLING ELECTRUM ]===================================================

				var gold = ElementLoader.FindElementByHash(SimHashes.Gold);
				var silver = ElementLoader.FindElementByHash(ModElements.Silver_Solid);
				float ratio = 0.6f;
				float highTempLerp = Mathf.Lerp(silver.highTemp, gold.highTemp, ratio);

				Element electrum_material = ElementLoader.FindElementByHash(SimHashes.Electrum);
				electrum_material.highTemp = highTempLerp;
				electrum_material.highTempTransitionTarget = ModElements.Silver_Liquid;
				electrum_material.highTempTransition = ElementLoader.FindElementByHash(Silver_Liquid);
				electrum_material.highTempTransitionOreID = SimHashes.Gold;
				electrum_material.highTempTransitionOreMassConversion = 0.6f;
				electrum_material.disabled = false;

				//=[ BITUMEN PATCH ]=======================================================
				AddTagsToElementAndEnable(SimHashes.Bitumen, [GameTags.ManufacturedMaterial, GameTags.BuildableAny], true);

				//=[ PHOSPHATE NODULES PATCH ]================================================
				AddTagToElementAndEnable(SimHashes.PhosphateNodules, GameTags.ConsumableOre);

				//=[ MAFIC ROCK PATCH ]==========================================================
				AddTagToElementAndEnable(SimHashes.MaficRock, GameTags.Crushable);


				///add hardened alloy tag to thermium
				AddTagToElementAndEnable(SimHashes.TempConductorSolid, ModAssets.Tags.AIO_HardenedAlloy);
			}
			if (Config.Instance.DupesEngineering_Enabled || Config.Instance.DupesMachinery_Enabled || Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				//=[ CRUSHED ROCK PATCH ]====================================================
				Element crushedRock_material = ElementLoader.FindElementByHash(SimHashes.CrushedRock);
				crushedRock_material.disabled = false;
				AddTagToElementAndEnable(SimHashes.CrushedRock, GameTags.ConsumableOre);
			}
			if (DlcManager.IsExpansion1Active() && Config.Instance.NuclearProcessing_Enabled)
			{//=[ ENABLING RADIUM ]===================================================
				AddTagToElementAndEnable(SimHashes.Radium, GameTags.ConsumableOre, true);
				//=[ ENABLING YellowCake ]===================================================
				AddTagToElementAndEnable(SimHashes.Yellowcake, GameTags.ManufacturedMaterial, true);

				AddTagToElementAndEnable(SimHashes.UraniumOre, ModAssets.Tags.AIO_RadEmitterInputMaterial);
				AddTagToElementAndEnable(SimHashes.EnrichedUranium, ModAssets.Tags.AIO_RadEmitterInputMaterial);
			}

			if (Config.Instance.DupesEngineering_Enabled)
			{
				//=[ ENABLING Cement ]===================================================
				var cement = ElementLoader.FindElementByHash(SimHashes.Cement);
				cement.disabled = false;
				cement.thermalConductivity = 3.11f;
				cement.radiationAbsorptionFactor = 1;

				//=[ ENABLING Bricks ]===================================================
				///https://material-properties.org/brick-density-heat-capacity-thermal-conductivity/
				var brick = ElementLoader.FindElementByHash(SimHashes.Brick);
				brick.highTemp = 2000;
				AddTagsToElementAndEnable(SimHashes.Brick, [GameTags.Crushable, GameTags.Insulator, GameTags.BuildableRaw]);
			}


			// adding combustible solid tag to coal and peat
			AddTagToElementAndEnable(SimHashes.Carbon, GameTags.CombustibleSolid);
			AddTagToElementAndEnable(SimHashes.RefinedCarbon, GameTags.CombustibleSolid);
			AddTagToElementAndEnable(SimHashes.Peat, GameTags.CombustibleSolid);
			AddTagToElementAndEnable(SimHashes.RefinedLipid, ModAssets.Tags.AIO_BioFuel);
		}
		static void AddTagToElementAndEnable(SimHashes element, Tag? tag = null, bool setMatCat = false) => AddTagsToElementAndEnable(element, tag.HasValue ? [tag.Value] : null, setMatCat);

		static void AddTagsToElementAndEnable(SimHashes element, Tag[] tags = null, bool setMatCat = false)
		{
			var elementMaterial = ElementLoader.FindElementByHash(element);
			if (elementMaterial == null)
				return;
			elementMaterial.disabled = false;

			if (tags == null || tags.Length == 0)
				return;

			if (setMatCat)
			{
				elementMaterial.materialCategory = tags.FirstOrDefault();
			}

			if (elementMaterial.oreTags == null)
				elementMaterial.oreTags = tags;

			List<Tag> newTags = new List<Tag>();
			foreach (var tag in tags)
			{
				if (elementMaterial.oreTags.Contains(tag))
					continue;
				newTags.Add(tag);
			}
			if (tags.Any())
				elementMaterial.oreTags = elementMaterial.oreTags.Concat(newTags).ToArray();
		}

		public static void AddElementOverheatModifier(SimHashes element, float degreeIncrease)
		{
			var elementMaterial = ElementLoader.FindElementByHash(element);
			if (elementMaterial == null)
				return;
			var attributeModifiers = Db.Get().BuildingAttributes;
			AttributeModifier overheatModifier = new AttributeModifier(attributeModifiers.OverheatTemperature.Id, degreeIncrease, elementMaterial.name);
			elementMaterial.attributeModifiers.Add(overheatModifier);
		}
		public static void AddElementDecorModifier(SimHashes element, float decorBonusMultiplier)
		{
			var elementMaterial = ElementLoader.FindElementByHash(element);
			if (elementMaterial == null)
				return;
			var attributeModifiers = Db.Get().BuildingAttributes;
			AttributeModifier decorModifier = new AttributeModifier(attributeModifiers.Decor.Id, decorBonusMultiplier, elementMaterial.name, true, false, true);
			elementMaterial.attributeModifiers.Add(decorModifier);
		}
		internal static void ConfigureElements()
		{
			AddElementOverheatModifier(ConcreteBlock_Solid, 100);
			AddElementDecorModifier(ConcreteBlock_Solid, -0.15f);

			AddElementOverheatModifier(SimHashes.Brick, 100);
			AddElementDecorModifier(SimHashes.Brick, 0.1f);

			//=: Giving Brass Decor and Temperature modifications :=====================================================
			AddElementDecorModifier(Brass_Solid, 0.25f);
			AddElementOverheatModifier(Brass_Solid, 80);

			//=: Giving Galena Temperature modifications :==============================================================
			AddElementDecorModifier(Galena_Solid, 0.1f);
			AddElementOverheatModifier(Galena_Solid, -30);

			//=: Giving Carbon Fibre Temperature modifications :========================================================
			AddElementOverheatModifier(CarbonFiber_Solid, 5000);

			//=: Giving Plasteel Temperature modifications :============================================================
			AddElementOverheatModifier(Plasteel_Solid, 400);
			//AddElementOverheatModifier(Plasteel_Solid, 800);


			float silverDegreeBonus = 40;
			///---own additions----
			///Silver as slightly worse gold
			AddElementDecorModifier(Silver_Solid, 0.45f);
			AddElementOverheatModifier(Silver_Solid, silverDegreeBonus);

			/// zinc - vanilla refined metal multiplers
			AddElementDecorModifier(Zinc_Solid, 0.2f);
			AddElementOverheatModifier(Zinc_Solid, 50);
			///ores:
			///zinc ore
			AddElementDecorModifier(Aurichalcite_Solid, 0.1f);
			///silver ore - mirroring gold in slightly worse
			AddElementDecorModifier(Argentite_Solid, 0.1f);
			AddElementOverheatModifier(Argentite_Solid, silverDegreeBonus);
			///electrum is silver+gold
			AddElementOverheatModifier(SimHashes.Electrum, silverDegreeBonus);

			///mirroring brass, less heat because it contains lead, compensating with higher decor
			AddElementDecorModifier(PhosphorBronze, 0.5f);
			AddElementOverheatModifier(PhosphorBronze, 35);

			///permendur, cobalt+iron based "steel lite"
			AddElementOverheatModifier(Permendur_Solid, 125);
			AddElementDecorModifier(Permendur_Solid, 0.20f);


			///the following values are somewhat WIP and bound to change.

			///invar: mirror permendur
			AddElementOverheatModifier(Invar_Solid, 125);
			AddElementDecorModifier(Invar_Solid, 0.20f);

			///chromite ore, unrefined ferrochrome
			AddElementOverheatModifier(ChromiteOre_Solid, 75);
			AddElementDecorModifier(ChromiteOre_Solid, 0.30f);

			///ferrochrome, iron+chromium alloy
			AddElementOverheatModifier(FerroChrome_Solid, 150);
			AddElementDecorModifier(FerroChrome_Solid, 0.60f);

			///raw chromium,
			AddElementOverheatModifier(Chromium_Solid, 225);
			AddElementDecorModifier(Chromium_Solid, 1.00f);

			///stainless steel, iron+chromium+nickel alloy, the "better" steel
			AddElementOverheatModifier(StainlessSteel_Solid, 250);
			AddElementDecorModifier(StainlessSteel_Solid, 0.75f);
		}

		internal static void RegisterTraitElementBandModifications()
		{
			foreach (var worldTrait in ProcGen.SettingsCache.GetCachedWorldTraits())
			{
				if (worldTrait.elementBandModifiers == null || !worldTrait.elementBandModifiers.Any())
					continue;
				var copperOreMods = worldTrait.elementBandModifiers.FirstOrDefault(modifier => modifier.element == SimHashes.Cuprite.ToString());

				if (copperOreMods != null)
				{
					float massMult = copperOreMods.massMultiplier;
					float bandMult = copperOreMods.bandMultiplier;

					foreach(var element in RefinementRecipeHelper.GetAllOres())
					{
						if (worldTrait.elementBandModifiers.Any(band => band.element == element.id.ToString()))
							continue;

						///is an ore and not an alloy
						if(element.HasTag(GameTags.Metal) && !element.HasTag(GameTags.RefinedMetal))
						{
							SgtLogger.l("adding "+ element.id +" ore band multiplier to " + worldTrait.filePath);
							worldTrait.elementBandModifiers.Add(new() { element = element.id.ToString(), bandMultiplier = bandMult, massMultiplier = massMult });
						}
					}
				}
			}
		}
	}
}

using HarmonyLib;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HarvestablePOIConfig;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class SpaceMiningAdditions
	{
		public static void AddExtraPOIElements()
		{
			if (DlcManager.IsExpansion1Active())
			{
				///Spaced out
				///manual patch to not break types
				var targetMethod = AccessTools.Method(typeof(HarvestablePOIConfig), "GenerateConfigs");
				var postfix = AccessTools.Method(typeof(SpaceMiningAdditions), "AddPOIs");
				Mod.HarmonyInstance.Patch(targetMethod, postfix: new(postfix));
			}
			else ///base game
			{
				AddBaseGamePoiElements();
			}
		}
		public static void AddPOIs(List<HarvestablePOIConfig.HarvestablePOIParams> __result)
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				AddPOIs_IndustrialOverhaul(__result);
		}
		static void AddBaseGamePoiElements()
		{
			if (!Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				return;
			var sdt = Db.Get().SpaceDestinationTypes;

			sdt.GasGiant.elementTable.Add(ModElements.Ammonia_Gas, new MathUtil.MinMax(100f, 200f));
			sdt.HydrogenGiant.elementTable.Add(ModElements.Ammonia_Gas, new MathUtil.MinMax(100f, 200f));
			sdt.IceGiant.elementTable.Add(ModElements.Ammonia_Solid, new MathUtil.MinMax(100f, 200f));
			sdt.ChlorinePlanet.elementTable.Add(ModElements.Chloroschist_Solid, new MathUtil.MinMax(100f, 200f));
			sdt.RockyAsteroid.elementTable.Add(ModElements.AmmoniumSalt_Solid, new MathUtil.MinMax(100f, 200f));
			sdt.OilyAsteroid.elementTable.Add(ModElements.OilShale_Solid, new MathUtil.MinMax(100f, 200f));
			sdt.Satellite.elementTable.Add(ModElements.Aurichalcite_Solid, new MathUtil.MinMax(100f, 200f));
			sdt.TerraPlanet.elementTable.Add(ModElements.Argentite_Solid, new MathUtil.MinMax(100f, 200f));
			sdt.RedDwarf.elementTable.Add(ModElements.Galena_Solid, new MathUtil.MinMax(100f, 200f));
		}

		public static void AddPOIs_IndustrialOverhaul(List<HarvestablePOIConfig.HarvestablePOIParams> __result)
		{
			foreach (HarvestablePOIConfig.HarvestablePOIParams param in __result)
			{
				//=: METALLIC ASTEROID FIELD :===================================================================
				if (param.poiType.id == HarvestablePOIConfig.MetallicAsteroidField)
				{
					param.poiType.harvestableElements.Add(ModElements.Galena_Solid, 1.5f);
					param.poiType.harvestableElements.Add(ModElements.Silver_Liquid, 1f);
					param.poiType.harvestableElements.Add(ModElements.LowGradeSand_Solid, 1f);
					param.poiType.harvestableElements.Add(ModElements.BaseGradeSand_Solid, 0.2f);
				}
				//=: SATELLITE FIELD :===========================================================================
				else if (param.poiType.id == HarvestablePOIConfig.SatelliteField)
				{
					param.poiType.harvestableElements.Add(ModElements.Aurichalcite_Solid, 3f);
					param.poiType.harvestableElements.Add(SimHashes.Carbon, 3f);
				}
				// Oily asteroid
				else if (param.poiType.id == HarvestablePOIConfig.OilyAsteroidField)
				{
					param.poiType.harvestableElements.Add(ModElements.OilShale_Solid, 1.75f);
				}
				//=: ICE FIELD :=================================================================================
				else if (param.poiType.id == HarvestablePOIConfig.IceAsteroidField)
				{
					param.poiType.harvestableElements.Add(ModElements.Ammonia_Solid, 0.25f);
					param.poiType.harvestableElements.Add(ModElements.AmmoniumWater_Liquid, 0.4f);
				}
				//=: ROCKY ASTEROID FIELD :======================================================================
				else if (param.poiType.id == HarvestablePOIConfig.RockyAsteroidField)
				{
					param.poiType.harvestableElements.Add(SimHashes.SandStone, 2f);
					//param.poiType.harvestableElements.Add(SimHashes.Granite, 2f); //added by vanilla game now
					param.poiType.harvestableElements.Add(SimHashes.MaficRock, 2f);
					param.poiType.harvestableElements.Add(ModElements.AmmoniumSalt_Solid, 1f);

				}
				//=: INTERSTELLAR ICE FIELD :====================================================================
				else if (param.poiType.id == HarvestablePOIConfig.InterstellarIceField)
				{
					param.poiType.harvestableElements.Add(ModElements.Ammonia_Solid, 0.5f);
					param.poiType.harvestableElements.Add(ModElements.AmmoniumWater_Liquid, 2f);
					param.poiType.harvestableElements.Add(ModElements.AmmoniumSalt_Solid, 0.5f);
				}
				//=: ORGANIC FIELD :====================================================================
				else if (param.poiType.id == HarvestablePOIConfig.OrganicMassField)
				{
					param.poiType.harvestableElements.Add(SimHashes.PhosphateNodules, 3f);
					param.poiType.harvestableElements.Add(SimHashes.Fossil, 0.1f);
					param.poiType.harvestableElements.Add(ModElements.AmmoniumSalt_Solid, 0.3f);
				}
				//=: GAS GIANT :=================================================================================
				else if (param.poiType.id == HarvestablePOIConfig.GasGiantCloud)
				{
					param.poiType.harvestableElements.Add(ModElements.Ammonia_Gas, 1f);
					param.poiType.harvestableElements.Add(ModElements.Nitrogen_Gas, 0.3f);
				}
				// Helium Cloud
				else if (param.poiType.id == HarvestablePOIConfig.HeliumCloud)
				{
					param.poiType.harvestableElements.Add(ModElements.Ammonia_Gas, 2f);
				}
				//=: CHLORINE CLOUD FIELD :======================================================================
				else if (param.poiType.id == HarvestablePOIConfig.ChlorineCloud)
				{
					param.poiType.harvestableElements.Add(ModElements.Chloroschist_Solid, 2f);
				}
				//=: GILDED ASTEROID FIELD :=====================================================================
				else if (param.poiType.id == HarvestablePOIConfig.GildedAsteroidField)
				{
					param.poiType.harvestableElements.Add(ModElements.MeteorOre_Solid, 0.25f);
					param.poiType.harvestableElements.Add(ModElements.LowGradeSand_Solid, 2f);
					param.poiType.harvestableElements.Add(ModElements.BaseGradeSand_Solid, 1f);
				}
				//=: GLIMMERING ASTEROID FIELD :=================================================================
				else if (param.poiType.id == HarvestablePOIConfig.GlimmeringAsteroidField)
				{
					param.poiType.harvestableElements.Add(ModElements.MeteorOre_Solid, 0.3f);
					param.poiType.harvestableElements.Add(ModElements.LowGradeSand_Solid, 0.5f);
					param.poiType.harvestableElements.Add(ModElements.BaseGradeSand_Solid, 1.2f);
				}
				//=: OXIDIZED ASTEROID FIELD :===================================================================
				else if (param.poiType.id == HarvestablePOIConfig.OxidizedAsteroidField)
				{
					param.poiType.harvestableElements.Add(SimHashes.PhosphateNodules, 3f);
					param.poiType.harvestableElements.Add(ModElements.Chloroschist_Solid, 1f);
					param.poiType.harvestableElements.Add(ModElements.SulphuricAcid_Gas, 0.5f);
				}
				//=: SALTY ASTEROID FIELD :======================================================================
				else if (param.poiType.id == HarvestablePOIConfig.SaltyAsteroidField)
				{
					param.poiType.harvestableElements.Add(ModElements.Borax_Solid, 0.5f);
					param.poiType.harvestableElements.Add(ModElements.Chloroschist_Solid, 2f);
					param.poiType.harvestableElements.Add(ModElements.AmmoniumSalt_Solid, 1f);
				}
				//=: FOREST ASTEROID FIELD :=====================================================================
				else if (param.poiType.id == HarvestablePOIConfig.ForestyOreField)
				{
					param.poiType.harvestableElements.Add(ModElements.Argentite_Solid, 3f);
					param.poiType.harvestableElements.Add(SimHashes.PhosphateNodules, 3f);
				}
				//=: SWAMPY ORE FIELD :==========================================================================
				else if (param.poiType.id == HarvestablePOIConfig.SwampyOreField)
				{
					param.poiType.harvestableElements.Add(SimHashes.PhosphateNodules, 2f);
				}
				//=: FROZEN ORE FIELD :==========================================================================
				else if (param.poiType.id == HarvestablePOIConfig.FrozenOreField)
				{
					param.poiType.harvestableElements.Add(ModElements.Nitrogen_Liquid, 1f);
					param.poiType.harvestableElements.Add(ModElements.Ammonia_Liquid, 0.4f);
				}
				//=: SAND ORE ASTEROID FIELD :===================================================================
				else if (param.poiType.id == HarvestablePOIConfig.SandyOreField)
				{
					param.poiType.harvestableElements.Add(ModElements.Aurichalcite_Solid, 2f);
					param.poiType.harvestableElements.Add(ModElements.Argentite_Solid, 2f);
				}
				//=: RADIOACTIVE ASTEROID FIELD :================================================================
				else if (param.poiType.id == HarvestablePOIConfig.RadioactiveAsteroidField)
				{
					param.poiType.harvestableElements.Add(ModElements.Borax_Solid, 0.6f);
				}
			}
		}
	}
}

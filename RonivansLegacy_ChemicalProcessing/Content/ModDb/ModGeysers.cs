using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class ModGeysers
	{

		public static void RegisterGeysers(List<GeyserGenericConfig.GeyserPrefabParams> configs)
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				RegisterGeysers_IndustrialOverhaul(configs);
		}

		public static void RegisterGeysers_IndustrialOverhaul(List<GeyserGenericConfig.GeyserPrefabParams> configs)
		{
			bool generic = Config.Instance.ModGeysersGeneric;

			//===[ CHEMICAL: AMMONIA GEYSER ]=====================================================================
			configs.Add(new("geyser_ammonia_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("AmmoniaGeyser",
				ModElements.Ammonia_Liquid,
				GeyserConfigurator.GeyserShape.Liquid,
				210.15f,
				1f, //modeled after oildrip(leaky oil fissure) i assume
				250f,
				250,
				null,
				null,
				50,
				600,
				0.1f,
				0.4f,
				15000,
				135000,
				0.4f,
				0.8f,
				210.15f),
				generic));

			//===[ CHEMICAL: AMMONIUM WATER GEYSER ]=====================================================================
			configs.Add(new("geyser_ammonium_water_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("AmmoniumWaterGeyser",
				ModElements.AmmoniumWater_Liquid,
				GeyserConfigurator.GeyserShape.Liquid,
				272.15f,
				1000f,
				2000f,
				500,
				null,
				null,
				60,
				1140,
				0.1f,
				0.9f,
				15000,
				135000,
				0.4f,
				0.8f,
				272.15f),
				generic));

			//===[ CHEMICAL: HOT MERCURY GAS VENT ]=====================================================================
			configs.Add(new("geyser_hot_mercurygas_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("HotMercuryGasVent",
				SimHashes.MercuryGas,
				GeyserConfigurator.GeyserShape.Gas,
				720.15f,
				70f,
				140f,
				5,
				null,
				null,
				60,
				1140,
				0.1f,
				0.9f,
				15000,
				135000,
				0.4f,
				0.8f,
				272.15f),
				generic));

			//===[ CHEMICAL: MOLTEN SALT GEYSER ]=====================================================================
			configs.Add(new("geyser_molten_salt_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("MoltenSaltGeyser",
				SimHashes.MoltenSalt,
				GeyserConfigurator.GeyserShape.Liquid,
				1300.15f,
				70f,
				120f,
				250,
				null,
				null,
				5,
				940,
				0.1f,
				0.7f,
				15000,
				135000,
				0.4f,
				0.8f,
				1300.15f),
				generic));

			//===[ CHEMICAL: LIQUID PHOSPHORUS GEYSER ]=====================================================================
			configs.Add(new("geyser_liquidPhosphorus_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("PhosphorusGeyser",
				SimHashes.LiquidPhosphorus,
				GeyserConfigurator.GeyserShape.Liquid,
				392.15f,
				70f,
				120f,
				250,
				null,
				null,
				5,
				940,
				0.1f,
				0.7f,
				15000,
				135000,
				0.4f,
				0.8f,
				392.15f),
				generic));

			//===[ CHEMICAL: HOT RAW NATURAL GAS VENT ]=====================================================================
			configs.Add(new("geyser_rawnaturalgas_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("RawGasVent",
				ModElements.RawNaturalGas_Gas,
				GeyserConfigurator.GeyserShape.Gas,
				773.15f,
				70f,
				140f,
				5,
				null,
				null,
				60,
				1140,
				0.1f,
				0.9f,
				15000,
				135000,
				0.4f,
				0.8f,
				773.15f),
				generic));

			//===[ CHEMICAL: SOUR WATER GEYSER ]=====================================================================
			configs.Add(new("geyser_sour_water_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("SourWaterGeyser",
				ModElements.RawNaturalGas_Gas,
				GeyserConfigurator.GeyserShape.Gas,
				278.15f,
				1000f,
				2000f,
				500,
				null,
				null,
				60,
				1140,
				0.1f,
				0.9f,
				15000,
				135000,
				0.4f,
				0.8f,
				278.15f),
				generic));

		}
	}
}

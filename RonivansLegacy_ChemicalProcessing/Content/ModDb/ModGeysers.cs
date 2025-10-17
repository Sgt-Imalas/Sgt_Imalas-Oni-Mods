using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	static class GeyserTypeExtension
	{
		public static GeyserConfigurator.GeyserType Log(this GeyserConfigurator.GeyserType type)
		{
			ModGeysers.GeyserIDs.Add("GeyserGeneric_"+type.id);
			return type;
		}
	}

	class ModGeysers
	{
		public static HashSet<string> GeyserIDs = [];
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
				150f, //modeled after oil_drip(leaky oil fissure) i assume
				400f,
				50,
				null,
				null,
				600,
				600,
				1f,
				1f,
				100,
				500,
				0.4f,
				0.8f,
				210.15f).Log(),
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
				272.15f).Log(),
				generic));

			//===[ CHEMICAL: HOT MERCURY GAS VENT ]=====================================================================
			configs.Add(new("geyser_hot_mercurygas_kanim", 2, 4,
				new GeyserConfigurator.GeyserType("HotMercuryGasVent",
				SimHashes.MercuryGas,
				GeyserConfigurator.GeyserShape.Gas,
				720.15f,
				350f,
				750f,
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
				272.15f).Log(),
				generic));

			//===[ CHEMICAL: MOLTEN SALT GEYSER ]=====================================================================
			configs.Add(new("geyser_molten_salt_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("MoltenSaltGeyser",
				SimHashes.MoltenSalt,
				GeyserConfigurator.GeyserShape.Molten,
				1300.15f,
				500f,
				1000f,
				500,
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
				1300.15f).Log(),
				generic));

			//===[ CHEMICAL: LIQUID PHOSPHORUS GEYSER ]=====================================================================
			configs.Add(new("geyser_liquidPhosphorus_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("PhosphorusGeyser",
				SimHashes.LiquidPhosphorus,
				GeyserConfigurator.GeyserShape.Liquid,
				392.15f,
				500f,
				1000f,
				500,
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
				392.15f).Log(),
				generic));

			//===[ CHEMICAL: HOT RAW NATURAL GAS VENT ]=====================================================================
			configs.Add(new("geyser_rawnaturalgas_kanim", 2, 4,
				new GeyserConfigurator.GeyserType("RawGasVent",
				ModElements.RawNaturalGas_Gas,
				GeyserConfigurator.GeyserShape.Gas,
				773.15f,
				140f,
				280f,
				10,
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
				773.15f).Log(),
				generic));

			//===[ CHEMICAL: SOUR WATER GEYSER ]=====================================================================
			configs.Add(new("geyser_sour_water_kanim", 4, 2,
				new GeyserConfigurator.GeyserType("SourWaterGeyser",
				ModElements.SourWater_Liquid,
				GeyserConfigurator.GeyserShape.Liquid,
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
				278.15f).Log(),
				generic));

			//===[ CHEMICAL: HOT NITROGEN VENT ]=====================================================================
			configs.Add(new("vent_hot_nitrogen_kanim", 2, 4,
				new GeyserConfigurator.GeyserType("NitrogenVent",
				ModElements.Nitrogen_Gas,
				GeyserConfigurator.GeyserShape.Gas,
				673.15f,
				100,
				350f,
				100,
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
				320.15f).Log(),
				false)); //nitrogen is too limited in use, keep it in the mod, but disable it from generic geyser list
		}
	}
}

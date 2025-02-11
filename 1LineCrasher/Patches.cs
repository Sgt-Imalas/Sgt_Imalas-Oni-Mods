using HarmonyLib;
using Klei;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _1LineCrasher
{
	/// <summary>
	/// "CLEAN" this project once to generate publicizer files
	/// </summary>
	internal class Patches
	{
		/// <summary>
		/// makes system clock more accurate
		/// </summary>
		[HarmonyPatch(typeof(DateTime))]
		[HarmonyPatch(nameof(DateTime.Update))]
		public static class CommaToNumber
		{
			public static void Postfix(DateTime __instance)
			{
				__instance.text.text = (GameClock.Instance.GetTimeInCycles() + 1).ToString("n3");
			}
		}


		/// <summary>
		/// make the entire grid space exposure
		/// </summary>
		//[HarmonyPatch(typeof(SubworldZoneRenderData), nameof(SubworldZoneRenderData.InitSimZones))]
		//public class SubworldZoneRenderData_InitSimZones_Patch
		//{
		//	//bytes is the sim zone type of the entire grid
		//	public static void Prefix(SubworldZoneRenderData __instance, ref byte[] bytes)
		//	{
		//		Debug.Log("Making the entire grid space exposure");
		//		for(int i = 0; i < bytes.Length; ++i)
		//		{
		//			bytes[i] = byte.MaxValue; //space exposure is defined as byte.max in the sim
		//			__instance.worldZoneTypes[i] = ProcGen.SubWorld.ZoneType.Space;
		//		}
		//	}
		//}

		//[HarmonyPatch(typeof(SubworldZoneRenderData), nameof(SubworldZoneRenderData.GenerateTexture))]
		//public class SubworldZoneRenderData_GenerateTexture_Patch
		//{
		//	public static void Prefix(SubworldZoneRenderData __instance)
		//	{

		//		Debug.Log("Making the entire grid space exposure");
		//		WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
		//		for(int i = 0; i < clusterDetailSave.overworldCells.Count; ++i)
		//		{
		//			clusterDetailSave.overworldCells[i].zoneType = ProcGen.SubWorld.ZoneType.Space;
		//		}
		//	}
		//}

		//[HarmonyPatch(typeof(CodexEntryGenerator))]
		//[HarmonyPatch(nameof(CodexEntryGenerator.PopulateCategoryEntries))]
		//[HarmonyPatch(new Type[] { typeof(List<CategoryEntry>), typeof(Comparison<CodexEntry>) })]
		//public static class FixMoped
		//{
		//	public static void Prefix(ref Comparison<CodexEntry> comparison)
		//	{
		//		comparison = new Comparison<CodexEntry>((entryA, entryB) => entryA.id.CompareTo(entryB.id));
		//	}
		//}


		//public class GeyserDumping
		//{
		//	public void DumpData()
		//	{

		//		Console.WriteLine("generic geysers in base game:");
		//		foreach (var entry in GenerateConfigs(true))
		//		{
		//			if (entry.isGenericGeyser)
		//			{
		//				Console.WriteLine(entry.id);
		//			}
		//		}
		//		Console.WriteLine("generic geysers in dlc:");
		//		foreach (var entry in GenerateConfigs(false))
		//		{
		//			if (entry.isGenericGeyser)
		//			{
		//				Console.WriteLine(entry.id);
		//			}
		//		}
		//	}

		//	class GeyserPrefabParams
		//	{
		//		public string id;
		//		public bool isGenericGeyser;
		//		public string dlcId;
		//		public geyserType geyserType;
		//		public GeyserPrefabParams(string anim, int width, int height, geyserType type, bool generic)
		//		{
		//			id = type.id;
		//			isGenericGeyser = generic;
		//			dlcId = type.DlcID;
		//			geyserType = type;
		//		}
		//	}
		//	class geyserType
		//	{
		//		public string id;
		//		public string DlcID;
		//		public geyserType(string id, SimHashes sss, float _, float __, float ___, float ____, float _____ = 0, float geyserTemperature = 0, float _______ = 0, float ________ = 0, float _________ = 0, float __________ = 0, string DlcID = null)
		//		{
		//			this.id = id;
		//			this.DlcID = DlcID;
		//		}
		//		public geyserType AddDisease(SimUtil.DiseaseInfo disease)
		//		{
		//			return this;
		//		}
		//	}


		//	private List<GeyserPrefabParams> GenerateConfigs(bool rmDlc)
		//	{
		//		List<GeyserPrefabParams> configs = new List<GeyserPrefabParams>();
		//		configs.Add(new GeyserPrefabParams("geyser_gas_steam_kanim", 2, 4, new geyserType("steam", SimHashes.Steam, 383.15f, 1000f, 2000f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_gas_steam_hot_kanim", 2, 4, new geyserType("hot_steam", SimHashes.Steam, 773.15f, 500f, 1000f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_water_hot_kanim", 4, 2, new geyserType("hot_water", SimHashes.Water, 368.15f, 2000f, 4000f, 500f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_water_slush_kanim", 4, 2, new geyserType("slush_water", SimHashes.DirtyWater, 263.15f, 1000f, 2000f, 500f, geyserTemperature: 263f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_water_filthy_kanim", 4, 2, new geyserType("filthy_water", SimHashes.DirtyWater, 303.15f, 2000f, 4000f, 500f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_salt_water_cool_slush_kanim", 4, 2, new geyserType("slush_salt_water", SimHashes.Brine, 263.15f, 1000f, 2000f, 500f, geyserTemperature: 263f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_salt_water_kanim", 4, 2, new geyserType("salt_water", SimHashes.SaltWater, 368.15f, 2000f, 4000f, 500f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_volcano_small_kanim", 3, 3, new geyserType("small_volcano", SimHashes.Magma, 2000f, 400f, 800f, 150f, 6000f, 12000f, 0.005f, 0.01f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_volcano_big_kanim", 3, 3, new geyserType("big_volcano", SimHashes.Magma, 2000f, 800f, 1600f, 150f, 6000f, 12000f, 0.005f, 0.01f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_co2_kanim", 4, 2, new geyserType("liquid_co2", SimHashes.LiquidCarbonDioxide, 218f, 100f, 200f, 50f, geyserTemperature: 218f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_gas_co2_hot_kanim", 2, 4, new geyserType("hot_co2", SimHashes.CarbonDioxide, 773.15f, 70f, 140f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_gas_hydrogen_hot_kanim", 2, 4, new geyserType("hot_hydrogen", SimHashes.Hydrogen, 773.15f, 70f, 140f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_gas_po2_hot_kanim", 2, 4, new geyserType("hot_po2", SimHashes.ContaminatedOxygen, 773.15f, 70f, 140f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_gas_po2_slimy_kanim", 2, 4, new geyserType("slimy_po2", SimHashes.ContaminatedOxygen, 333.15f, 70f, 140f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_gas_chlorine_kanim", 2, 4, new geyserType("chlorine_gas", SimHashes.ChlorineGas, 333.15f, 70f, 140f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_gas_methane_kanim", 2, 4, new geyserType("methane", SimHashes.Methane, 423.15f, 70f, 140f, 5f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_copper_kanim", 3, 3, new geyserType("molten_copper", SimHashes.MoltenCopper, 2500f, 200f, 400f, 150f, 480f, 1080f, 0.01666667f, 0.1f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_iron_kanim", 3, 3, new geyserType("molten_iron", SimHashes.MoltenIron, 2800f, 200f, 400f, 150f, 480f, 1080f, 0.01666667f, 0.1f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_gold_kanim", 3, 3, new geyserType("molten_gold", SimHashes.MoltenGold, 2900f, 200f, 400f, 150f, 480f, 1080f, 0.01666667f, 0.1f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_aluminum_kanim", 3, 3, new geyserType("molten_aluminum", SimHashes.MoltenAluminum, 2000f, 200f, 400f, 150f, 480f, 1080f, 0.01666667f, 0.1f, DlcID: "EXPANSION1_ID"), true));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_tungsten_kanim", 3, 3, new geyserType("molten_tungsten", SimHashes.MoltenTungsten, 4000f, 200f, 400f, 150f, 480f, 1080f, 0.01666667f, 0.1f, DlcID: "EXPANSION1_ID"), false));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_niobium_kanim", 3, 3, new geyserType("molten_niobium", SimHashes.MoltenNiobium, 3500f, 800f, 1600f, 150f, 6000f, 12000f, 0.005f, 0.01f, DlcID: "EXPANSION1_ID"), false));
		//		configs.Add(new GeyserPrefabParams("geyser_molten_cobalt_kanim", 3, 3, new geyserType("molten_cobalt", SimHashes.MoltenCobalt, 2500f, 200f, 400f, 150f, 480f, 1080f, 0.01666667f, 0.1f, DlcID: "EXPANSION1_ID"), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_oil_kanim", 4, 2, new geyserType("oil_drip", SimHashes.CrudeOil, 600f, 1f, 250f, 50f, 600f, 600f, 1f, 1f, 100f, 500f), true));
		//		configs.Add(new GeyserPrefabParams("geyser_liquid_sulfur_kanim", 4, 2, new geyserType("liquid_sulfur", SimHashes.LiquidSulfur, 438.35f, 1000f, 2000f, 500f, DlcID: "EXPANSION1_ID"), true));

		//		if (rmDlc)
		//			configs.RemoveAll((Predicate<GeyserPrefabParams>)(geyser => !geyser.geyserType.DlcID.IsNullOrWhiteSpace() && (geyser.geyserType.DlcID == DlcManager.EXPANSION1_ID)));
		//		return configs;
		//	}

		//}

		//[HarmonyPatch(typeof(Localization), nameof(Localization.OverloadStrings))]
		//[HarmonyPatch(new Type[] { typeof(Dictionary<string,string>) })]
		//public static class Localization_Initialize_Patch
		//{
		//    [HarmonyPriority(Priority.VeryLow)]
		//    public static void Prefix(ref Dictionary<string, string> translated_strings)
		//    {
		//        var keys = new List<string>(translated_strings.Keys);
		//        foreach (var key in keys)
		//        {
		//            translated_strings[key] = "Moped";
		//        }
		//    }
		//}
		//[HarmonyPatch(typeof(RetiredColonyInfoScreen), nameof(RetiredColonyInfoScreen.DisplayWorlds))]
		////[HarmonyPatch(new Type[] { typeof(Dictionary<string, string>) })]
		//public static class MopedPatch2
		//{
		//    [HarmonyPriority(Priority.VeryLow)]
		//    public static void Prefix(ref Dictionary<string, string> translated_strings)
		//    {
		//        var keys = new List<string>(translated_strings.Keys);
		//        foreach (var key in keys)
		//        {
		//            translated_strings[key] = "Moped";
		//        }
		//    }
		//}

		//[HarmonyPatch(typeof(RailGun.States))]
		//[HarmonyPatch(nameof(RailGun.States.InitializeStates))]
		//public static class Modify_RailgunCooldown
		//{
		//    public static bool Prepare() => false;
		//    static float NewCooldownTimer = 20f;
		//    public static void Postfix(RailGun.States __instance)
		//    {
		//        __instance.on.cooldown.pre.Enter((smi) => smi.sm.cooldownTimer.Set(NewCooldownTimer, smi));
		//    }
		//}

		/// <summary>
		/// custom meteor example code
		/// </summary>
		//[HarmonyPatch(typeof(Db), "Initialize")]
		//public static class Db_addSeason
		//{
		//    public static void Postfix(Db __instance)
		//    {
		//        __instance.GameplayEvents.Add(
		//            new MeteorShowerSeason(
		//                "AllShowersInOnceID",
		//                GameplaySeason.Type.World,
		//                "EXPANSION1_ID",
		//                20f,
		//                false,
		//                startActive: true,
		//                clusterTravelDuration: 6000f)
		//            .AddEvent(Db.Get().GameplayEvents.MeteorShowerDustEvent)
		//            .AddEvent(Db.Get().GameplayEvents.ClusterCopperShower)
		//            .AddEvent(Db.Get().GameplayEvents.ClusterGoldShower)
		//            .AddEvent(Db.Get().GameplayEvents.ClusterIronShower)
		//            .AddEvent(Db.Get().GameplayEvents.ClusterIceShower)
		//            .AddEvent(Db.Get().GameplayEvents.ClusterBiologicalShower)
		//            .AddEvent(Db.Get().GameplayEvents.ClusterBleachStoneShower)
		//            .AddEvent(Db.Get().GameplayEvents.ClusterUraniumShower));
		//        ///obv. not all events
		//    }
		//}
	}
}

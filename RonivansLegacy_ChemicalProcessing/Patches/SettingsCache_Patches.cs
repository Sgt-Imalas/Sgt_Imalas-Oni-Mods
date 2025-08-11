using HarmonyLib;
using Klei;
using ProcGen;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class SettingsCache_Patches
	{

		//[HarmonyPatch(typeof(SettingsCache), nameof(SettingsCache.LoadBiome))]
		//public class SettingsCache_LoadBiome_Patch
		//{

		//	[HarmonyPrepare]
		//	public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;

		//	public static void Prefix(string longName, List<YamlIO.Error> errors, ref bool __state)
		//	{
		//		//getting biome path
		//		SettingsCache.SplitNameFromPath(longName, out var path, out var name);
		//		if (!SettingsCache.biomeSettingsCache.ContainsKey(path))
		//		{
		//			///biome is loaded for the first time, only inserting custom elements here
		//			__state = true;
		//		}
		//	}
		//	public static void Postfix(string longName, List<YamlIO.Error> errors, bool __state)
		//	{
		//		if (!__state)
		//			return;

		//		SettingsCache.SplitNameFromPath(longName, out var path, out var name);
		//		//for whatever reason the biome failed to load, abort injections
		//		if (!SettingsCache.biomeSettingsCache.ContainsKey(path))
		//		{
		//			return;
		//		}

		//		BiomeElementInjections.Inject(longName);
		//	}
		//}
	}
}

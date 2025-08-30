using HarmonyLib;
using Klei;
using ProcGen;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.WORLD_TRAITS;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class SettingsCache_Patches
	{
		[HarmonyPatch(typeof(SettingsCache), nameof(SettingsCache.LoadBiome))]
		public class SettingsCache_LoadBiome_Patch
		{

			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled && Config.Instance.WorldgenElementInjection;

			public static void Prefix(string longName, List<YamlIO.Error> errors, ref bool __state)
			{
				//getting biome path
				SettingsCache.SplitNameFromPath(longName, out var path, out var name);
				if (!SettingsCache.biomeSettingsCache.ContainsKey(path))
				{
					///biome is loaded for the first time, only inserting custom elements here
					__state = true;
				}
			}
			public static void Postfix(string longName, List<YamlIO.Error> errors, bool __state)
			{
				if (!__state)
					return;

				SettingsCache.SplitNameFromPath(longName, out var path, out var name);
				//for whatever reason the biome failed to load, abort injections
				if (!SettingsCache.biomeSettingsCache.ContainsKey(path))
				{
					return;
				}

				ChemProc_BiomeInjection.InjectIntoBiome(longName);
			}
		}
		[HarmonyPatch(typeof(SettingsCache), nameof(SettingsCache.LoadFeature))]
		public class SettingsCache_LoadFeature_Patch
		{

			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled && Config.Instance.WorldgenElementInjection;

			public static void Prefix(string longName, List<YamlIO.Error> errors, ref bool __state)
			{
				if (!SettingsCache.featureSettings.ContainsKey(longName))
				{
					///feature is loaded for the first time, only inserting custom elements here
					__state = true;
				}
			}
			public static void Postfix(string longName, List<YamlIO.Error> errors, bool __state)
			{
				if (!__state)
					return;

				//for whatever reason the feature failed to load, abort injections
				if (!SettingsCache.featureSettings.ContainsKey(longName))
				{
					return;
				}

				ChemProc_BiomeInjection.InjectElementsIntoFeature(longName);
			}
		}

		[HarmonyPatch(typeof(SettingsCache), nameof(SettingsCache.LoadSubworld))]
		public class SettingsCache_LoadSubworld_Patch
		{

			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled && Config.Instance.WorldgenElementInjection;

			public static void Prefix(WeightedSubworldName subworldWeightedName, List<YamlIO.Error> errors, ref bool __state)
			{
				string longName = subworldWeightedName.name; 
				if (!SettingsCache.subworlds.ContainsKey(longName))
				{
					///feature is loaded for the first time, only inserting custom elements here
					__state = true;
				}
			}
			public static void Postfix(WeightedSubworldName subworldWeightedName, List<YamlIO.Error> errors, bool __state)
			{
				if (!__state)
					return;
				string longName = subworldWeightedName.name;
				//for whatever reason the feature failed to load, abort injections
				if (!SettingsCache.subworlds.TryGetValue(longName, out var subWorld))
				{
					return;
				}

				ChemProc_BiomeInjection.InjectFeaturesIntoSubworld(subWorld,longName);

				foreach (Feature feature in subWorld.features)
				{
					feature.type = SettingsCache.LoadFeature(feature.type, errors);
				}
			}
		}
	}
	
}

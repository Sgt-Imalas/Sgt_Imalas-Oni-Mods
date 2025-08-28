using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomGenerators;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class GeneratorList
	{
		public static void AddGeneratorToIgnore(string ID) { GeneratorsToIgnore.Add(ID); SgtLogger.l("Adding generator to screen ignore list: " + ID); }
		public static void AddCombustionGenerator(string ID) { CombustionGenerators.Add(ID); SgtLogger.l("Adding generator to combustion generator list: " + ID); }
		public static HashSet<string> GeneratorsToIgnore = [];
		public static HashSet<string> CombustionGenerators = [];
		internal static void AppendCombustionGenerators(ref List<Tag> disallowedBuildings)
		{
			SgtLogger.l("Appending combustion generators to supersustainable list, entry count: "+CombustionGenerators.Count);
			foreach (var gen in CombustionGenerators)
			{
				if (!disallowedBuildings.Contains(gen))
				{
					SgtLogger.l("Adding combustion generator to supersustainables list: " + gen);
					disallowedBuildings.Add(gen);
				}
			}
		}
	}
}

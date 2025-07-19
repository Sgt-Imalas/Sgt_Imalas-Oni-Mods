
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class BiomeElementInjections
	{
		public class ElementInjection
		{
			public SimHashes Element;
			public float bandSize;
		}
		public class BiomeElementInjection
		{
			public string activeSubBiome;
			Dictionary<string, List<ElementInjection>> SubBiomeInjections = [];
			public static BiomeElementInjection Create(string biome)
			{
				Debug.Assert(BiomeInjections.ContainsKey(biome) == false, "BiomeElementInjection for biome already exists: " + biome);
				var injection = new BiomeElementInjection();
				BiomeInjections[biome] = injection;
				return injection;
			}
			public BiomeElementInjection SubBiome(string subBiome)
			{
				if (!SubBiomeInjections.ContainsKey(subBiome))
				{
					SubBiomeInjections[subBiome] = new List<ElementInjection>();
				}
				activeSubBiome = subBiome;
				return this;
			}
			public BiomeElementInjection Element(SimHashes element, float bandSize = 1f)
			{
				if (activeSubBiome == null)
				{
					throw new InvalidOperationException("No active sub-biome set for ElementInjection, please call SubBiome() first.");
				}
				if (!SubBiomeInjections.ContainsKey(activeSubBiome))
				{
					SgtLogger.l("Active sub-biome not found in SubBiomeInjections dictionary.");
					return this;
				}
				SubBiomeInjections[activeSubBiome].Add(new ElementInjection { Element = element, bandSize = bandSize });
				return this;
			}
		}

		static Dictionary<string, BiomeElementInjection> BiomeInjections = new();
		static BiomeElementInjections()
		{
			BiomeElementInjection.Create("Ocean")
				.SubBiome("Basic")
					.Element(ModElements.Chloroschist_Solid, 0.05f)
				.SubBiome("Dry")
					.Element(ModElements.Chloroschist_Solid, 0.05f)
				.SubBiome("Briny")
					.Element(ModElements.Chloroschist_Solid, 0.05f)
				.SubBiome("Frozen")
					.Element(ModElements.Chloroschist_Solid, 0.05f)
					;


		}

		internal static void Inject(string biomePath, ProcGen.BiomeSettings biomeSettings)
		{
			string biome = Directory.GetParent(biomePath).Name;
			string subBiome = Path.GetFileNameWithoutExtension(biomePath);
			SgtLogger.l("Injecting Chemical Processing elements into biome: " + biome+" with sub-biome: "+ subBiome);
		}	
	}
}

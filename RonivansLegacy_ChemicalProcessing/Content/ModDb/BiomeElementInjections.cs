
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.GAMEPLAY_EVENTS;

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
			public Dictionary<string, List<ElementInjection>> SubBiomeInjections = [];
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
					.Element(ModElements.Chloroschist_Solid,0.25f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
				.SubBiome("Dry")
					.Element(ModElements.Chloroschist_Solid, 0.25f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
				.SubBiome("Briny")
					.Element(ModElements.Chloroschist_Solid, 0.25f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
				.SubBiome("Frozen")
					.Element(ModElements.Chloroschist_Solid, 0.25f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
					;

			BiomeElementInjection.Create("Oil")
				.SubBiome("OilPockets")
					.Element(ModElements.OilShale_Solid, 1f)
					.Element(ModElements.Galena_Solid, 1f)
				.SubBiome("OilField")
					.Element(ModElements.OilShale_Solid, 1.5f)
					.Element(ModElements.Galena_Solid, 1f)
				.SubBiome("OilPocketsFrozen")
					.Element(ModElements.OilShale_Solid, 1.5f)
					.Element(ModElements.Galena_Solid, 1)
				.SubBiome("OilPocketsSandy")
					.Element(ModElements.OilShale_Solid, 1.5f)
					.Element(ModElements.Galena_Solid, 1f)
				.SubBiome("OilPatch")
					.Element(ModElements.OilShale_Solid, 1.5f)
					.Element(ModElements.Galena_Solid, 1f)
				.SubBiome("OilPatch")
					.Element(ModElements.OilShale_Solid, 1.5f)
					.Element(ModElements.Galena_Solid, 1f)
					;


			BiomeElementInjection.Create("Misc")
				.SubBiome("Surface")
					.Element(ModElements.MeteorOre_Solid, 2f)
				.SubBiome("HospitableClassicSurface")
					.Element(ModElements.MeteorOre_Solid, 2f)
				.SubBiome("IcySurface")
					.Element(ModElements.MeteorOre_Solid, 2f)
				.SubBiome("SoftDust")
					.Element(ModElements.MeteorOre_Solid, 2f)
					;

			BiomeElementInjection.Create("Metallic")
				.SubBiome("Golden")
					.Element(SimHashes.Electrum, 1)
					.Element(ModElements.Argentite_Solid, 1)
					;
			BiomeElementInjection.Create("Frozen")
				.SubBiome("Wet")
					.Element(ModElements.Argentite_Solid, 1)
				.SubBiome("Dry")
					.Element(ModElements.Argentite_Solid, 1)
				.SubBiome("Solid")
					.Element(ModElements.Argentite_Solid, 1)
				.SubBiome("Core")
					.Element(ModElements.Argentite_Solid, 1)
					;
			BiomeElementInjection.Create("Barren")
				.SubBiome("Granite")
					.Element(ModElements.Argentite_Solid, 0.08f)
				.SubBiome("GraniteTunnels")
					.Element(ModElements.Argentite_Solid, 0.03f)
					.Element(ModElements.Silver_Solid, 0.03f)
				.SubBiome("RockyChasm")
					.Element(ModElements.Argentite_Solid, 0.03f)
				.SubBiome("RockyCaves")
					.Element(ModElements.Argentite_Solid, 0.1f)
				.SubBiome("GraniteOre")
					.Element(ModElements.Argentite_Solid, 0.04f)
					;
		}

		internal static void Inject(string biomePath)
		{
			string biome = Directory.GetParent(biomePath).Name;
			string subBiome = Path.GetFileNameWithoutExtension(biomePath);
			SgtLogger.l("Injecting Chemical Processing elements into biome: " + biome + " with sub-biome: " + subBiome);



			if (BiomeInjections.TryGetValue(biome, out var biomeInjection)
				&& biomeInjection.SubBiomeInjections.TryGetValue(subBiome, out var injections)
				&& ProcGen.SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.TryGetValue(biomePath, out ElementBandConfiguration elementBandConfiguration))
			{

				foreach (var injection in injections)
				{
					SgtLogger.l("Injecting element: " + injection.Element + " with band size: " + injection.bandSize);
					elementBandConfiguration.Add(new ElementGradient(injection.Element.CreateTag().ToString(), injection.bandSize, new()));
				}
				

				SgtLogger.l("Dumping ElementBandConfiguration for: "+ biome + "/" + subBiome);
				foreach (var ban in elementBandConfiguration)
				{
					SgtLogger.l("Element band: " + ban.content + ", Band Size: " + ban.bandSize);
				}
			}
		}
	}
}

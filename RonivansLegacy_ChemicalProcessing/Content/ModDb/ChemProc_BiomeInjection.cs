
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.GAMEPLAY_EVENTS;
using static STRINGS.SUBWORLDS;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class ChemProc_BiomeInjection
	{
		const string CustomFeaturePrefix = "features/ronivan_aio_custom/";

		public class BiomeElementInjection
		{
			public SimHashes Element;
			public float bandSize;
			public int InjectionIndex;
		}
		public class FeatureElementInjection
		{
			public string ChoiceGroupName;
			public SimHashes Element;
			public float weight;
			public ProcGen.SampleDescriber.Override Override = null;
		}
		public class SubWorldFeatureInjections
		{
			public static Dictionary<string, List<string>> FeatureToSubworldInjections;

			public static void Inject_SO(string subworldPath, string customFeatureName) => Inject_SO(subworldPath, [customFeatureName]);
			public static void Inject_SO(string subworldPath, string[] customFeatureName) => Inject("expansion1::" + subworldPath, customFeatureName);
			public static void Inject(string subworldPath, string customFeatureName) => Inject(subworldPath, [customFeatureName]);
			public static void Inject(string subworldPath, string[] customFeatureNames)
			{
				if (FeatureToSubworldInjections == null)
					FeatureToSubworldInjections = [];
				if (!FeatureToSubworldInjections.ContainsKey(subworldPath))
					FeatureToSubworldInjections[subworldPath] = [];

				foreach(var customFeatureName in  customFeatureNames)
					FeatureToSubworldInjections[subworldPath].Add(CustomFeaturePrefix + customFeatureName);
			}
		}

		public class FeatureElementInjections
		{
			public List<FeatureElementInjection> ElementInjections = [];
			public static FeatureElementInjections Create(string featurePath)
			{
				Debug.Assert(FeatureInjections.ContainsKey(featurePath) == false, "FeatureElementInjection for feature already exists: " + featurePath);
				FeatureInjections[featurePath] = new FeatureElementInjections();
				return FeatureInjections[featurePath];
			}
			string activeChoiceGroup;
			public List<Tuple<string, SimHashes>> ToRemove = [];
			public FeatureElementInjections TargetGroup(string ElementChoiceGroup)
			{
				activeChoiceGroup = ElementChoiceGroup;
				return this;
			}
			public FeatureElementInjections RemoveExisting(SimHashes elementToRemove)
			{
				ToRemove.Add(new(activeChoiceGroup, elementToRemove));
				return this;
			}

			public FeatureElementInjections Element(SimHashes element, float weight, SampleDescriber.Override _override = null)
			{
				if (activeChoiceGroup == null)
				{
					throw new InvalidOperationException("No active element choice group set for ElementInjection, please call TargetGroup() first.");
				}
				ElementInjections.Add(new FeatureElementInjection { Element = element, weight = weight, ChoiceGroupName = activeChoiceGroup, Override = _override });
				return this;
			}
			public FeatureElementInjections MassOverride(int massOverride)
			{
				if (ElementInjections.Count == 0)
					throw new InvalidOperationException("no elements defined yet!");
				ElementInjections.Last().Override = new() { massOverride = massOverride };
				return this;
			}
		}
		public class BiomeElementInjections
		{
			public string activeSubBiome;
			public Dictionary<string, List<BiomeElementInjection>> SubBiomeInjections = [];
			public static BiomeElementInjections Create(string biome)
			{
				Debug.Assert(BiomeInjections.ContainsKey(biome) == false, "BiomeElementInjection for biome already exists: " + biome);
				var injection = new BiomeElementInjections();
				BiomeInjections[biome] = injection;
				return injection;
			}
			public BiomeElementInjections SubBiome(string subBiome)
			{
				if (!SubBiomeInjections.ContainsKey(subBiome))
				{
					SubBiomeInjections[subBiome] = new List<BiomeElementInjection>();
				}
				activeSubBiome = subBiome;
				return this;
			}
			public BiomeElementInjections Element(SimHashes element, float bandSize = 1f, int injectionIndex = -1)
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
				SubBiomeInjections[activeSubBiome].Add(new BiomeElementInjection { Element = element, bandSize = bandSize, InjectionIndex = injectionIndex });
				return this;

			}
		}
		static Dictionary<string, FeatureElementInjections> FeatureInjections = [];
		static Dictionary<string, BiomeElementInjections> BiomeInjections = [];

		public static void ElementToBiomeInjection()
		{
			BiomeElementInjections.Create("Ocean")
				.SubBiome("Basic")
					.Element(ModElements.Chloroschist_Solid, 1f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
				.SubBiome("Dry")
					.Element(ModElements.Chloroschist_Solid, 1f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
				.SubBiome("Briny")
					.Element(ModElements.Chloroschist_Solid, 1f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
				.SubBiome("Frozen")
					.Element(ModElements.Chloroschist_Solid, 1f)
					.Element(ModElements.Aurichalcite_Solid, 1f)
					;

			BiomeElementInjections.Create("Oil")
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


			BiomeElementInjections.Create("Misc")
				.SubBiome("Surface")
					.Element(ModElements.MeteorOre_Solid, 2f)
				.SubBiome("HospitableClassicSurface")
					.Element(ModElements.MeteorOre_Solid, 2f)
				.SubBiome("IcySurface")
					.Element(ModElements.MeteorOre_Solid, 2f)
				.SubBiome("SoftDust")
					.Element(ModElements.MeteorOre_Solid, 2f)
					;

			BiomeElementInjections.Create("Metallic")
				.SubBiome("Golden")
					.Element(SimHashes.Electrum, 1)
					.Element(ModElements.Argentite_Solid, 1)
					;
			BiomeElementInjections.Create("Frozen")
				.SubBiome("Wet")
					.Element(ModElements.Argentite_Solid, 1)
				.SubBiome("Dry")
					.Element(ModElements.Argentite_Solid, 1)
				.SubBiome("Solid")
					.Element(ModElements.Argentite_Solid, 1)
				.SubBiome("Core")
					.Element(ModElements.Argentite_Solid, 1)
					;
			BiomeElementInjections.Create("Barren")
				.SubBiome("Granite")
					.Element(ModElements.Argentite_Solid, 1f)
				.SubBiome("GraniteTunnels")
					.Element(ModElements.Argentite_Solid, 1f)
					.Element(ModElements.Silver_Solid, 1f)
				.SubBiome("RockyChasm")
					.Element(ModElements.Argentite_Solid, 1f)
				.SubBiome("RockyCaves")
					.Element(ModElements.Argentite_Solid, 1f)
				.SubBiome("GraniteOre")
					.Element(ModElements.Argentite_Solid, 1f)
					;
		}

		static void ElementToFeatureInjection()
		{
			///Feature Injections:
			///Features are smaller, partially premade patterns/shapes that are spawned into the world by the worldgen, for example  small empty rooms
			///The following injections add custom elements to spawn in the randomly chosen parts of patterns


			///Geodes Trait feature:
			FeatureElementInjections.Create("features/traits/Geode")
				.TargetGroup("RoomCenterElements")
					.Element(ModElements.Zinc_Solid, 50)
					.Element(ModElements.Silver_Solid, 50)
					.Element(ModElements.Ammonia_Solid, 20)
					.Element(ModElements.Borax_Solid, 20)
					.Element(ModElements.OilShale_Solid, 50)
					.Element(ModElements.AmmoniumSalt_Solid, 50)
					.Element(ModElements.MeteorOre_Solid, 40);

			///SO Metal Caves Trait feature:
			FeatureElementInjections.Create("expansion1::features/traits/MetalCavesSmall")
				.TargetGroup("RoomBorderChoices0")
					.Element(ModElements.Argentite_Solid, 0.15f)
					.Element(ModElements.Aurichalcite_Solid, 0.15f)
				.TargetGroup("RoomBorderChoices1")
					.Element(ModElements.Argentite_Solid, 0.15f)
					.Element(ModElements.Aurichalcite_Solid, 0.15f)
					;

			///Generic Injections
			//FeatureElementInjections.Create("features/generic/SandGeode")
			//	.TargetGroup("RoomCenterElements").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(64);

			///---[BARREN BIOME (SO)]---
			FeatureElementInjections.Create("expansion1::features/barren/GraniteCoalDeposit")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.OilShale_Solid, 1).MassOverride(540);

			///---[FOREST BIOME]---
			///Forest Features Base
			FeatureElementInjections.Create("features/forest/SmallFrozenLake")
				.TargetGroup("RoomBorderChoices1")
					.Element(SimHashes.PhosphateNodules, 1).MassOverride(224)
					.Element(ModElements.Argentite_Solid, 1);

			FeatureElementInjections.Create("features/forest/SmallLake")
				.TargetGroup("RoomBorderChoices1")
					.Element(SimHashes.PhosphateNodules, 1).MassOverride(224)
					.Element(ModElements.Argentite_Solid, 1);
			FeatureElementInjections.Create("features/forest/PhosphoriteLumb")
				.TargetGroup("RoomCenterElements")
					.Element(SimHashes.PhosphateNodules, 0.5f).MassOverride(214);

			///Forest Features SO
			//FeatureElementInjections.Create("expansion1::features/forest/OxyrockCave")
			//	.TargetGroup("RoomCenterElements").Element(ModElements.AmmoniumSalt_Solid, 0.4f)
			//	.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.2f);

			FeatureElementInjections.Create("expansion1::features/forest/SmallDirtyLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.5f);

			///---[FROZEN BIOME]---
			///Frozen Features Base
			FeatureElementInjections.Create("features/frozen/CO2Lake")
				.TargetGroup("RoomCenterElements").Element(ModElements.Ammonia_Gas, 1.5f).MassOverride(7);

			FeatureElementInjections.Create("features/frozen/ColdBubble")
				.TargetGroup("RoomCenterElements").Element(ModElements.Ammonia_Gas, 1.5f).MassOverride(7);

			//FeatureElementInjections.Create("features/frozen/SandGeode")
			//	.TargetGroup("RoomCenterElements").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(64);

			///Frozen Features SO
			FeatureElementInjections.Create("expansion1::features/frozen/MetalShelf")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.Argentite_Solid, 0.2f).MassOverride(1240)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Argentite_Solid, 0.2f).MassOverride(960);

			///---[HOTMARSH BIOME]---
			FeatureElementInjections.Create("features/hotmarsh/BlobRoom")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(64);
			FeatureElementInjections.Create("features/hotmarsh/BlobSlushRoom")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(64);
			FeatureElementInjections.Create("features/hotmarsh/GeyserHole")
				.TargetGroup("RoomCenterElements").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(64);
			FeatureElementInjections.Create("features/hotmarsh/LargeBlobRoomDry")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(64);
			FeatureElementInjections.Create("features/hotmarsh/SandGeode")
				.TargetGroup("RoomCenterElements").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(88)
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(72);
			FeatureElementInjections.Create("features/hotmarsh/TallRoom")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(92);
			FeatureElementInjections.Create("features/hotmarsh/TallSlushRoom")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(64);

			///---[Jungle BIOME]---
			FeatureElementInjections.Create("features/jungle/BleachRoom")
				.TargetGroup("RoomCenterElements").Element(ModElements.Chloroschist_Solid, 1f).MassOverride(1224)
				.TargetGroup("RoomBorderChoices0").Element(ModElements.Chloroschist_Solid, 1f).MassOverride(1422)
				.TargetGroup("RoomBorderChoices1")
					.Element(ModElements.Chloroschist_Solid, 4f).MassOverride(1322)
					.Element(SimHashes.PhosphateNodules, 1);

			FeatureElementInjections.Create("features/jungle/SandGeode")
				.TargetGroup("RoomCenterElements").Element(SimHashes.PhosphateNodules, 1f)
				.TargetGroup("RoomBorderChoices0").Element(SimHashes.PhosphateNodules, 1f)
				.TargetGroup("RoomBorderChoices1").Element(SimHashes.PhosphateNodules, 1f);

			FeatureElementInjections.Create("features/jungle/SmallRoom")
				.TargetGroup("RoomBorderChoices1").Element(SimHashes.Electrum, 1f); //why is that here ??

			///---[METALLIC BIOME (SO)]---
			FeatureElementInjections.Create("expansion1::features/metallic/DreckoHome")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.3f).MassOverride(240)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.3f).MassOverride(240);

			///---[MOO BIOME (SO)]---
			FeatureElementInjections.Create("expansion1::features/moo/MooBubbleSmall")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.3f).MassOverride(920);

			FeatureElementInjections.Create("expansion1::features/moo/MooCaveLarge")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.3f).MassOverride(880)
				.TargetGroup("RoomBorderChoices2").Element(ModElements.AmmoniumSalt_Solid, 0.3f).MassOverride(880);

			///---[OCEAN BIOME]---
			///Ocean Features Base
			FeatureElementInjections.Create("features/ocean/BleachLumb")
				.TargetGroup("RoomCenterElements").Element(ModElements.Chloroschist_Solid, 1f).MassOverride(426);

			FeatureElementInjections.Create("features/ocean/DeepPool")
				.TargetGroup("RoomBorderChoices0")
					.Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(126)
					.Element(ModElements.Chloroschist_Solid, 1f)
				.TargetGroup("RoomBorderChoices1")
					.Element(ModElements.AmmoniumSalt_Solid, 1f).MassOverride(124)
					.Element(ModElements.Chloroschist_Solid, 1f);

			FeatureElementInjections.Create("features/ocean/SaltCave")
				.TargetGroup("RoomBorderChoices0")
					.Element(ModElements.Borax_Solid, 1f).MassOverride(113)
				.TargetGroup("RoomBorderChoices1")
					.Element(ModElements.Borax_Solid, 1f).MassOverride(62);

			///Ocean Features SO
			FeatureElementInjections.Create("expansion1::features/ocean/SlushPool")
				.TargetGroup("RoomCenterElements").Element(ModElements.Borax_Solid, 0.3f).MassOverride(62);

			///---[OIL BIOME]---
			FeatureElementInjections.Create("features/oilpockets/Cavity")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.OilShale_Solid, 1f).MassOverride(320).Element(ModElements.Galena_Solid,1)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.OilShale_Solid, 1f).MassOverride(356);

			FeatureElementInjections.Create("features/oilpockets/CavityOilFloatersTall")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.OilShale_Solid, 1f).MassOverride(224).Element(ModElements.Galena_Solid, 1)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.OilShale_Solid, 1f).MassOverride(412);

			FeatureElementInjections.Create("features/oilpockets/CavityPond")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.OilShale_Solid, 1f).MassOverride(320).Element(ModElements.Galena_Solid, 1)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.OilShale_Solid, 1f).MassOverride(292);

			FeatureElementInjections.Create("features/oilpockets/CavityPondFrozen")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.OilShale_Solid, 1f).MassOverride(412)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.OilShale_Solid, 1f).MassOverride(340).Element(ModElements.Galena_Solid, 1);

			FeatureElementInjections.Create("features/oilpockets/DiamondClump")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.OilShale_Solid, 1f).MassOverride(212)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.OilShale_Solid, 1f).MassOverride(442);

			FeatureElementInjections.Create("features/oilpockets/OilWell")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.OilShale_Solid, 1f).MassOverride(524);


			///---[RUST BIOME]---
			FeatureElementInjections.Create("features/rust/EthanolLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1f);

			FeatureElementInjections.Create("features/rust/MiniBleachBall")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.Aurichalcite_Solid, 1f).MassOverride(822)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1f).MassOverride(920);

			FeatureElementInjections.Create("features/rust/SaltDeposit")
				.TargetGroup("RoomCenterElements").Element(ModElements.Borax_Solid, 1f).MassOverride(34);

			FeatureElementInjections.Create("features/rust/SulfurHole")
				.TargetGroup("RoomCenterElements").Element(ModElements.SulphuricAcid_Liquid, 4f)
				.RemoveExisting(SimHashes.SaltWater);

			FeatureElementInjections.Create("features/rust/TallRoom")
				.TargetGroup("RoomCenterElements").Element(ModElements.SulphuricAcid_Liquid, 3f).MassOverride(1)
				.RemoveExisting(SimHashes.Brine);

			///---[SEDIMENTARY BIOME]---
			///Sedimentary Features Base
			FeatureElementInjections.Create("features/sedimentary/DarkRoom")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(128);

			FeatureElementInjections.Create("features/sedimentary/FlatFrozenLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(154);

			FeatureElementInjections.Create("features/sedimentary/FlatLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(162);

			FeatureElementInjections.Create("features/sedimentary/MediumFrozenLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(188);

			FeatureElementInjections.Create("features/sedimentary/MediumLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(211);

			FeatureElementInjections.Create("features/sedimentary/MetalVacuumBlob")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1);

			FeatureElementInjections.Create("features/sedimentary/MetalVacuumBlobTall")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1);

			FeatureElementInjections.Create("features/sedimentary/SmallEmptyLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(128);

			FeatureElementInjections.Create("features/sedimentary/SmallEmptyLakeTall")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(128);

			FeatureElementInjections.Create("features/sedimentary/SmallFrozenLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(128);

			FeatureElementInjections.Create("features/sedimentary/SmallLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(128);

			FeatureElementInjections.Create("features/sedimentary/SmallLakeTall")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 1).MassOverride(224);

			///Sedimentary Features SO
			FeatureElementInjections.Create("expansion1::features/sedimentary/CoalDeposit")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.Aurichalcite_Solid, 1);

			FeatureElementInjections.Create("expansion1::features/sedimentary/CoalDepositDense")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.Aurichalcite_Solid, 1).MassOverride(1150);

			FeatureElementInjections.Create("expansion1::features/sedimentary/MetalVacuumBlobDense")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.Aurichalcite_Solid, 1).MassOverride(1100)
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1).MassOverride(1100);

			FeatureElementInjections.Create("expansion1::features/sedimentary/ParchedLake")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(98);

			FeatureElementInjections.Create("expansion1::features/sedimentary/SmallAlgaeBall")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(64);

			FeatureElementInjections.Create("expansion1::features/sedimentary/SmallMetalVacuumBlob")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1);

			FeatureElementInjections.Create("expansion1::features/sedimentary/TinyEmptyLake")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(92);

			FeatureElementInjections.Create("expansion1::features/sedimentary/TinyLake")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(78);

			///---[SWAMPY BIOME]---
			///Swamp Features SO
			FeatureElementInjections.Create("expansion1::features/swamp/AirPocket")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(92);

			FeatureElementInjections.Create("expansion1::features/swamp/BigAirPocket")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(98);

			FeatureElementInjections.Create("expansion1::features/swamp/DirtBall")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(82);

			FeatureElementInjections.Create("expansion1::features/swamp/DirtBallSmall")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.6f).MassOverride(94);

			FeatureElementInjections.Create("expansion1::features/swamp/DirtyPool")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.4f).MassOverride(64);

			FeatureElementInjections.Create("expansion1::features/swamp/MetalCavern")
				.TargetGroup("RoomBorderChoices2").Element(ModElements.AmmoniumSalt_Solid, 0.4f).MassOverride(78);

			FeatureElementInjections.Create("expansion1::features/swamp/MetalCavernSmall")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.4f).MassOverride(88);

			FeatureElementInjections.Create("expansion1::features/swamp/MetalSprinkles")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.Aurichalcite_Solid, 0.4f).MassOverride(2240);

			FeatureElementInjections.Create("expansion1::features/swamp/MudClump")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.2f).MassOverride(88);

			FeatureElementInjections.Create("expansion1::features/swamp/PollutedMudClump")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.2f).MassOverride(128);

			FeatureElementInjections.Create("expansion1::features/swamp/SandClump")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.2f).MassOverride(98);

			FeatureElementInjections.Create("expansion1::features/swamp/ShallowPool")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.2f).MassOverride(90);

			///---[WASTELAND BIOME]---
			///Wasteland (beetle) Features SO
			FeatureElementInjections.Create("expansion1::features/wasteland/BeetleCave")
				.TargetGroup("RoomBorderChoices0").Element(ModElements.AmmoniumSalt_Solid, 0.5f).MassOverride(56);

			FeatureElementInjections.Create("expansion1::features/wasteland/ShallowPool")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.AmmoniumSalt_Solid, 0.2f).MassOverride(90);


			///-[FROSTY PLANET BIOMES]
			FeatureElementInjections.Create("dlc2::features/icecaves/MetalBlob")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1f);
			FeatureElementInjections.Create("dlc2::features/carrotquarry/IceBellyCave")
				.TargetGroup("RoomBorderChoices1").Element(ModElements.Argentite_Solid, 0.3f);

			///-[PREHISTORIC PLANET BIOMES]
			FeatureElementInjections.Create("dlc4::features/garden/NickelOreBall")
			.TargetGroup("RoomBorderChoices1").Element(ModElements.Aurichalcite_Solid, 1f);

			FeatureElementInjections.Create("dlc4::features/raptor/RaptorHabitat")
			.TargetGroup("RoomBorderChoices1").Element(ModElements.Argentite_Solid, 0.3f).Element(ModElements.Chloroschist_Solid, 0.3f);

			FeatureElementInjections.Create("dlc4::features/wetlands/LargeCave")
			.TargetGroup("RoomBorderChoices1").Element(SimHashes.Electrum, 0.3f)
			.TargetGroup("RoomBorderChoices2").Element(SimHashes.Electrum, 0.15f);

			FeatureElementInjections.Create("dlc4::features/wetlands/MosquitoCave")
			.TargetGroup("RoomBorderChoices1").Element(SimHashes.Electrum, 0.4f)
			.TargetGroup("RoomBorderChoices2").Element(SimHashes.Electrum, 0.4f);

		}

		static void FeatureToSubworldInjection()
		{
			///Injecting custom templates into existing biomes

			//Barren
			SubWorldFeatureInjections.Inject("subworlds/barren/BarrenGranute", ["GranitePyriteDeposit", "GraniteSilverDeposit"]);

			SubWorldFeatureInjections.Inject_SO("subworlds/barren/BarrenCore", "ShaleBall");
			SubWorldFeatureInjections.Inject_SO("subworlds/barren/CoalyGranite", "SilverOreBall");
			
			//Forest
			SubWorldFeatureInjections.Inject("subworlds/forest/Forest", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestHot", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestMiniMetal", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestMiniMetalHot", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestMiniOxy", "PhosphateLump");
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestMiniOxyHot", "PhosphateLump");
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestMiniWater", "PhosphateGeode");
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestMiniWater", "PhosphateGeode");
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestMiniWaterHot", "PhosphateGeode");
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestStart", ["PhosphateGeode","SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestStartHot", ["PhosphateGeode","SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject("subworlds/forest/ForestFrozen", ["SilverFrozenLake", "SilverLump", "SilverOreVein"]);

			SubWorldFeatureInjections.Inject_SO("subworlds/forest/ForestCore", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/forest/ForestLandingSite", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/forest/ForestSurface", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/forest/med_Forest", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/forest/med_ForestHot", ["SilverLump", "SilverOreVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/forest/med_ForestSurface", ["SilverLump", "SilverOreVein"]);
			
			//Frozen
			SubWorldFeatureInjections.Inject("subworlds/frozen/CO2Lakes", "AmmoniaBubble");
			SubWorldFeatureInjections.Inject("subworlds/frozen/Frozen", "AmmoniaBubble");
			SubWorldFeatureInjections.Inject("subworlds/frozen/FrozenStrange", "AmmoniaBubble");

			//SubWorldFeatureInjections.Inject_SO("subworlds/frozen/FrozenCore", "FrozenOilBall");
			//SubWorldFeatureInjections.Inject_SO("subworlds/frozen/FrozenMedium", "FrozenOilBall");
			
			//Jungle
			SubWorldFeatureInjections.Inject("subworlds/jungle/Jungle", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject("subworlds/jungle/JungleFrozen", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject("subworlds/jungle/JungleSolid", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject("subworlds/jungle/JungleStrange", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);

			SubWorldFeatureInjections.Inject_SO("subworlds/jungle/JungleDreckless", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/jungle/JungleGassy", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/jungle/JungleInactive", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/jungle/JungleSteamy", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/jungle/med_JungleFrozen", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/jungle/med_JungleInactive", ["PhosphateGeode", "PhosphateLump", "ElectrumVein"]);
			
			//Marsh
			SubWorldFeatureInjections.Inject("subworlds/marsh/HotMarsh", "NitrateGeode");
			SubWorldFeatureInjections.Inject("subworlds/marsh/HotMarshSlush", "NitrateGeode");
			SubWorldFeatureInjections.Inject("subworlds/marsh/HotMarshStrange", "NitrateGeode");

			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/HotMarshLandingSite", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/HotMarshInactive", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/HotMarshSteamy", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/HotMarshSurface", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/med_HotMarshInactive", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/med_HotMarshLandingSite", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/med_HotMarshMushrooms", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/marsh/med_HotMarshStart", "NitrateGeode");
			
			//Metallic
			SubWorldFeatureInjections.Inject_SO("subworlds/metallic/Metallic", ["SilverOreMetallicDeposit", "PyriteMetallicDeposit"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/metallic/SwampyRenewableMetallicCold", ["SilverOreMetallicDeposit", "ZincOreMetallicDeposit"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/metallic/SwampyRenewableMetallic", ["SilverOreMetallicDeposit", "ZincOreMetallicDeposit"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/metallic/RenewableMetallic", ["SilverOreMetallicDeposit", "ZincOreMetallicDeposit"]);
			
			//Ocean
			SubWorldFeatureInjections.Inject("subworlds/ocean/Ocean", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject("subworlds/ocean/OceanDeep", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject("subworlds/ocean/OceanFrozen", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject("subworlds/ocean/OceanHot", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject("subworlds/ocean/OceanSlush", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject("subworlds/ocean/OceanStrange", ["BoraxLump", "DeepRichMetallicPool"]);

			SubWorldFeatureInjections.Inject_SO("subworlds/ocean/med_Ocean", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/ocean/med_OceanDeep", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/ocean/med_OceanSurface", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/ocean/OceanSlush", ["BoraxLump", "DeepRichMetallicPool"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/ocean/OceanSurface", ["BoraxLump", "DeepRichMetallicPool"]);

			//Oil
			SubWorldFeatureInjections.Inject("subworlds/oil/OilDry", "GalenaVein");
			SubWorldFeatureInjections.Inject("subworlds/oil/OilField", "GalenaVein");
			SubWorldFeatureInjections.Inject("subworlds/oil/OilPatch", "GalenaVein");
			SubWorldFeatureInjections.Inject("subworlds/oil/OilPatchDouble", "GalenaVein");
			SubWorldFeatureInjections.Inject("subworlds/oil/OilPockets", "GalenaVein");
			SubWorldFeatureInjections.Inject("subworlds/oil/OilPocketsCold", "GalenaVein");
			SubWorldFeatureInjections.Inject("subworlds/oil/OilPocketsStrange", "GalenaVein");

			SubWorldFeatureInjections.Inject_SO("subworlds/oil/OilSparse", "GalenaVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/oil/OilSurface", "GalenaVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/oil/OilWells", "GalenaVein");

			//Regolith
			SubWorldFeatureInjections.Inject_SO("subworlds/regolith/BarrenDust", "MeteorGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/regolith/DeadOasis", "MeteorGeode");

			//Rust
			SubWorldFeatureInjections.Inject("subworlds/rust/Rust", "ChloroschistDeposit");
			SubWorldFeatureInjections.Inject("subworlds/rust/RustFrozen", "ChloroschistDeposit");
			SubWorldFeatureInjections.Inject("subworlds/rust/RustWarm", ["ChloroschistDeposit", "SulfuricHole"]);

			SubWorldFeatureInjections.Inject_SO("subworlds/rust/med_Rust", ["ChloroschistDeposit", "SulfuricHole"]);
			SubWorldFeatureInjections.Inject_SO("subworlds/rust/med_RustFrozen", "ChloroschistDeposit");
			SubWorldFeatureInjections.Inject_SO("subworlds/rust/med_RustIceBorder", "ChloroschistDeposit");
			SubWorldFeatureInjections.Inject_SO("subworlds/rust/RustChillyLakes", ["ChloroschistDeposit", "SulfuricHole"]);

			//Sandstone
			SubWorldFeatureInjections.Inject("subworlds/sandstone/Desert", "PyriteVein");
			SubWorldFeatureInjections.Inject("subworlds/sandstone/Sandstone", "ZincOreVein");
			SubWorldFeatureInjections.Inject("subworlds/sandstone/SandstoneFrozen", "ZincOreVein");
			SubWorldFeatureInjections.Inject("subworlds/sandstone/SandstoneStrange", "ZincOreVein");

			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/med_SandstoneResourceful", "ZincOreVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/Sandstone", "ZincOreVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SandstoneMini", "ZincOreVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SandstoneMiniWater", "ZincOreVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SandstoneStart", "ZincOreVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SandstoneWarp", "ZincOreVein");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SandstoneWarpStart", "ZincOreVein");

			//Space
			SubWorldFeatureInjections.Inject("subworlds/space/Surface", "MeteorGeode");
			SubWorldFeatureInjections.Inject("subworlds/space/SurfaceCrags", "MeteorGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/space/HospitableClassicSurface", "MeteorGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/space/HospitableSurface", "MeteorGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/space/IcySurface", "MeteorGeode");

			//Swamp
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/med_SwampSurface", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/Swamp", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SwampMini", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SwampStart", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SwampStartDense", "NitrateGeode");
			SubWorldFeatureInjections.Inject_SO("subworlds/sandstone/SwampWarpStart", "NitrateGeode");

			//Wasteland
			SubWorldFeatureInjections.Inject_SO("subworlds/wasteland/WastelandBeetle", "ZincOreMetallicDeposit");
			SubWorldFeatureInjections.Inject_SO("subworlds/wasteland/WastelandWorm", "ZincOreMetallicDeposit");
		}

		static ChemProc_BiomeInjection()
		{
			//ElementToBiomeInjection();
			ElementToFeatureInjection();
			FeatureToSubworldInjection();
		}

		internal static void InjectIntoBiome(string biomePath)
		{
			string biome = Directory.GetParent(biomePath).Name;
			string subBiome = System.IO.Path.GetFileNameWithoutExtension(biomePath);
			//SgtLogger.l("Injecting Chemical Processing elements into biome: " + biome + " with sub-biome: " + subBiome);

			if (BiomeInjections.TryGetValue(biome, out var biomeInjection)
				&& biomeInjection.SubBiomeInjections.TryGetValue(subBiome, out var injections)
				&& ProcGen.SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.TryGetValue(biomePath, out ElementBandConfiguration elementBandConfiguration))
			{

				foreach (var injection in injections)
				{
					if (Mod.Instance.mod.IsDev)
						SgtLogger.l("Injecting element: " + injection.Element + " with band size: " + injection.bandSize);
					elementBandConfiguration.Add(new ElementGradient(injection.Element.CreateTag().ToString(), injection.bandSize, new()));
				}


				SgtLogger.l("Dumping ElementBandConfiguration for: " + biome + "/" + subBiome);
				foreach (var ban in elementBandConfiguration)
				{
					SgtLogger.l("Element band: " + ban.content + ", Band Size: " + ban.bandSize);
				}
			}
		}

		static HashSet<string> remainingFeatures = null;
		public static void InjectElementsIntoFeature(string featurePath)
		{
			if (remainingFeatures == null)
				remainingFeatures = FeatureInjections.Keys.ToHashSet();


			if (!ProcGen.SettingsCache.featureSettings.TryGetValue(featurePath, out var featureSettings)
				|| !FeatureInjections.TryGetValue(featurePath, out var featureInjections))
			{
				return;
			}
			//SgtLogger.l("Injecting into Feature: " + featurePath);

			if (featureSettings.ElementChoiceGroups != null)
			{
				if (featureInjections.ToRemove.Any())
				{
					foreach (var elementToRemove in featureInjections.ToRemove)
					{
						if (!featureSettings.ElementChoiceGroups.TryGetValue(elementToRemove.first, out var group))
						{
							SgtLogger.warning("[Feature Element Injection]: could not remove " + elementToRemove.second + " from " + elementToRemove.first + ": group does not exist on " + featurePath);
							continue;
						}
						group.choices.RemoveAll(wsh => wsh.element == elementToRemove.second.ToString());

						if (Mod.Instance.mod.IsDev)
							SgtLogger.l("[Feature Element Injection]: removed " + elementToRemove.second + " from group " + elementToRemove.first + " in " + featurePath);
					}
				}

				foreach (var injection in featureInjections.ElementInjections)
				{
					if (!featureSettings.ElementChoiceGroups.ContainsKey(injection.ChoiceGroupName))
					{
						SgtLogger.warning($"[Feature Element Injection]: ElementChoiceGroup {injection.ChoiceGroupName} not found in feature {featurePath}");
						continue;
					}
					var group = featureSettings.ElementChoiceGroups[injection.ChoiceGroupName];
					group.choices.Add(new ProcGen.WeightedSimHash(injection.Element.ToString(), injection.weight, injection.Override));
					if(Mod.Instance.mod.IsDev)
						SgtLogger.l($"[Feature Element Injection]: Added {injection.Element} to {injection.ChoiceGroupName} in {featurePath}");
				}
			}
			remainingFeatures.Remove(featurePath);
			//SgtLogger.l(remainingFeatures.Count + " injections remaining");
		}

		public static void InjectFeaturesIntoSubworld(SubWorld subworld, string subworldPath)
		{
			//SgtLogger.l("SUBWORLDPATH: " + subworldPath);
			if (!SubWorldFeatureInjections.FeatureToSubworldInjections.TryGetValue(subworldPath, out var featureInjections))
				return;

			if (subworld.features == null)
				subworld.features = [];
			subworld.features.AddRange(featureInjections.Select(feature => new Feature() { type = feature }));
		}
	}
}

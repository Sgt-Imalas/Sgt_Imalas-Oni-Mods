using Database;
using Dupes_Industrial_Overhaul.Chemical_Processing.Space;
using Klei.AI;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS;
using TUNING;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class MeteorShowerAdjustments
	{
		static void AddCometToShower(GameplayEvent shower, string cometID, float spawnWeight)
		{
			(shower as MeteorShowerEvent).AddMeteor(cometID, spawnWeight);
		}
		public static void AddModdedComets(GameplayEvents __instance)
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				///BaseGame Showers
				AddCometToShower(__instance.MeteorShowerIronEvent,HeavyCometConfig.ID, 2);
				AddCometToShower(__instance.MeteorShowerIronEvent, ZincCometConfig.ID, 1);

				AddCometToShower(__instance.MeteorShowerCopperEvent,HeavyCometConfig.ID, 2);
				AddCometToShower(__instance.MeteorShowerCopperEvent,SilverCometConfig.ID, 1);

				AddCometToShower(__instance.MeteorShowerGoldEvent, HeavyCometConfig.ID, 2);

				if (!DlcManager.IsExpansion1Active())
					return;

				//rockmeteor
				AddCometToShower(__instance.MeteorShowerDustEvent, HeavyCometConfig.ID, 3);

				AddCometToShower(__instance.ClusterRegolithShower, HeavyCometConfig.ID, 2);

				AddCometToShower(__instance.ClusterGoldShower, HeavyCometConfig.ID, 2);

				AddCometToShower(__instance.ClusterCopperShower, HeavyCometConfig.ID, 2);
				AddCometToShower(__instance.ClusterCopperShower, SilverCometConfig.ID, 1);
				//dustmeteors
				AddCometToShower(__instance.ClusterIronShower, HeavyCometConfig.ID, 1);
				AddCometToShower(__instance.ClusterIronShower, ZincCometConfig.ID, 1);

				AddCometToShower(__instance.ClusterLightRegolithShower, HeavyCometConfig.ID, 1);

				AddCometToShower(__instance.ClusterUraniumShower, HeavyCometConfig.ID, 1);

			}
		}

		//#region dummyEvents
		//public GameplayEvent HatchSpawnEvent;

		//public GameplayEvent PartyEvent;

		//public GameplayEvent EclipseEvent;

		//public GameplayEvent SatelliteCrashEvent;

		//public GameplayEvent FoodFightEvent;

		//public GameplayEvent PrickleFlowerBlightEvent;

		//public GameplayEvent MeteorShowerIronEvent;

		//public GameplayEvent MeteorShowerGoldEvent;

		//public GameplayEvent MeteorShowerCopperEvent;

		//public GameplayEvent MeteorShowerDustEvent;

		//public GameplayEvent MeteorShowerFullereneEvent;

		//public GameplayEvent GassyMooteorEvent;

		//public GameplayEvent ClusterSnowShower;

		//public GameplayEvent ClusterIceShower;

		//public GameplayEvent ClusterBiologicalShower;

		//public GameplayEvent ClusterLightRegolithShower;

		//public GameplayEvent ClusterRegolithShower;

		//public GameplayEvent ClusterGoldShower;

		//public GameplayEvent ClusterCopperShower;

		//public GameplayEvent ClusterIronShower;

		//public GameplayEvent ClusterUraniumShower;

		//public GameplayEvent ClusterOxyliteShower;

		//public GameplayEvent ClusterBleachStoneShower;

		//public GameplayEvent IridiumShowerEvent;

		//public GameplayEvent ClusterIceAndTreesShower;

		//static MeteorShowerEvent Add(MeteorShowerEvent e) => e;
		//	#endregion
		//public void BaseGameMeteorEvents()
		//{
		//	MeteorShowerGoldEvent = Add(new MeteorShowerEvent("MeteorShowerGoldEvent", 3000f, 0.4f, secondsBombardmentOn: new MathUtil.MinMax(50f, 100f), secondsBombardmentOff: new MathUtil.MinMax(800f, 1200f))
		//		.AddMeteor(GoldCometConfig.ID, 2f)
		//		.AddMeteor(RockCometConfig.ID, 0.5f)
		//		.AddMeteor(DustCometConfig.ID, 5f));
		//	MeteorShowerCopperEvent = Add(new MeteorShowerEvent("MeteorShowerCopperEvent", 4200f, 5.5f, secondsBombardmentOn: new MathUtil.MinMax(100f, 400f), secondsBombardmentOff: new MathUtil.MinMax(300f, 1200f))
		//		.AddMeteor(CopperCometConfig.ID, 1f)
		//		.AddMeteor(RockCometConfig.ID, 1f));
		//	MeteorShowerIronEvent = Add(new MeteorShowerEvent("MeteorShowerIronEvent", 6000f, 1.25f, secondsBombardmentOn: new MathUtil.MinMax(100f, 400f), secondsBombardmentOff: new MathUtil.MinMax(300f, 1200f)).AddMeteor(IronCometConfig.ID, 1f)
		//		.AddMeteor(RockCometConfig.ID, 2f)
		//		.AddMeteor(DustCometConfig.ID, 5f));
		//}

		//public void Expansion1MeteorEvents()
		//{
		//	MeteorShowerDustEvent = Add(new MeteorShowerEvent("MeteorShowerDustEvent", 9000f, 1.25f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Regolith"), secondsBombardmentOn: new MathUtil.MinMax(100f, 400f), secondsBombardmentOff: new MathUtil.MinMax(300f, 1200f))
		//		.AddMeteor(RockCometConfig.ID, 1f)
		//		.AddMeteor(DustCometConfig.ID, 6f));
		//	GassyMooteorEvent = Add(new MeteorShowerEvent("GassyMooteorEvent", 15f, 3.125f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Moo"), secondsBombardmentOn: new MathUtil.MinMax(15f, 15f), secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE, affectedByDifficulty: false)
		//		.AddMeteor(GassyMooCometConfig.ID, 1f));
		//	MeteorShowerFullereneEvent = Add(new MeteorShowerEvent("MeteorShowerFullereneEvent", 30f, 0.5f, secondsBombardmentOn: new MathUtil.MinMax(80f, 80f), secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE, clusterMapMeteorShowerID: null, affectedByDifficulty: false)
		//		.AddMeteor(FullereneCometConfig.ID, 6f)
		//		.AddMeteor(DustCometConfig.ID, 1f));
		//	ClusterSnowShower = Add(new MeteorShowerEvent("ClusterSnowShower", 600f, 3f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Snow"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(SnowballCometConfig.ID, 2f)
		//		.AddMeteor(LightDustCometConfig.ID, 1f));
		//	ClusterIceShower = Add(new MeteorShowerEvent("ClusterIceShower", 300f, 1.4f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Ice"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(SnowballCometConfig.ID, 14f)
		//		.AddMeteor(HardIceCometConfig.ID, 1f));
		//	ClusterOxyliteShower = Add(new MeteorShowerEvent("ClusterOxyliteShower", 300f, 4f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Oxylite"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(OxyliteCometConfig.ID, 4f)
		//		.AddMeteor(LightDustCometConfig.ID, 4f));
		//	ClusterBleachStoneShower = Add(new MeteorShowerEvent("ClusterBleachStoneShower", 300f, 3f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("BleachStone"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(BleachStoneCometConfig.ID, 13f)
		//		.AddMeteor(LightDustCometConfig.ID, 3f));
		//	ClusterBiologicalShower = Add(new MeteorShowerEvent("ClusterBiologicalShower", 300f, 3f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Biological"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(SlimeCometConfig.ID, 2f)
		//		.AddMeteor(AlgaeCometConfig.ID, 1f)
		//		.AddMeteor(PhosphoricCometConfig.ID, 1f));
		//	ClusterLightRegolithShower = Add(new MeteorShowerEvent("ClusterLightRegolithShower", 300f, 4f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("LightDust"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(DustCometConfig.ID, 1f)
		//		.AddMeteor(LightDustCometConfig.ID, 1f));
		//	ClusterRegolithShower = Add(new MeteorShowerEvent("ClusterRegolithShower", 300f, 3.5f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("HeavyDust"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(DustCometConfig.ID, 3f)
		//		.AddMeteor(RockCometConfig.ID, 2f)
		//		.AddMeteor(LightDustCometConfig.ID, 1f));
		//	ClusterGoldShower = Add(new MeteorShowerEvent("ClusterGoldShower", 75f, 1f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Gold"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(GoldCometConfig.ID, 4f)
		//		.AddMeteor(RockCometConfig.ID, 1f)
		//		.AddMeteor(LightDustCometConfig.ID, 2f));
		//	ClusterCopperShower = Add(new MeteorShowerEvent("ClusterCopperShower", 150f, 2.5f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Copper"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(CopperCometConfig.ID, 2f)
		//		.AddMeteor(RockCometConfig.ID, 1f));
		//	ClusterIronShower = Add(new MeteorShowerEvent("ClusterIronShower", 300f, 4.5f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Iron"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(IronCometConfig.ID, 4f)
		//		.AddMeteor(DustCometConfig.ID, 1f)
		//		.AddMeteor(LightDustCometConfig.ID, 2f));
		//	ClusterUraniumShower = Add(new MeteorShowerEvent("ClusterUraniumShower", 150f, 4.5f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("Uranium"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(UraniumCometConfig.ID, 2.5f)
		//		.AddMeteor(DustCometConfig.ID, 1f).
		//		AddMeteor(LightDustCometConfig.ID, 2f));
		//}

		//public void DLCMeteorEvents()
		//{
		//	ClusterIceAndTreesShower = Add(new MeteorShowerEvent("ClusterIceAndTreesShower", 300f, 1.4f, clusterMapMeteorShowerID: ClusterMapMeteorShowerConfig.GetFullID("IceAndTrees"), secondsBombardmentOn: METEORS.BOMBARDMENT_ON.UNLIMITED, secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(SpaceTreeSeedCometConfig.ID, 1f
		//		).AddMeteor(HardIceCometConfig.ID, 2f)
		//		.AddMeteor(SnowballCometConfig.ID, 22f));
		//	//LargeImpactor = Add(new LargeImpactorEvent("LargeImpactor", DlcManager.DLC4, null));
		//	//LargeImpactor.AddPrecondition(GameplayEventPreconditions.Instance.Or(GameplayEventPreconditions.Instance.Not(GameplayEventPreconditions.Instance.DifficultySetting(CustomGameSettingConfigs.DemoliorDifficulty, "Off")), GameplayEventPreconditions.Instance.ClusterHasTag("DemoliorImminentImpact")));
		//	IridiumShowerEvent = Add(new MeteorShowerEvent("IridiumShower", 30f, 0.5f, secondsBombardmentOn: new MathUtil.MinMax(80f, 80f), secondsBombardmentOff: METEORS.BOMBARDMENT_OFF.NONE)
		//		.AddMeteor(IridiumCometConfig.ID, 1f));
		//}
	}
}

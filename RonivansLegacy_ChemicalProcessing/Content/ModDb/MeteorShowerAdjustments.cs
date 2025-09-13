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
	}
}

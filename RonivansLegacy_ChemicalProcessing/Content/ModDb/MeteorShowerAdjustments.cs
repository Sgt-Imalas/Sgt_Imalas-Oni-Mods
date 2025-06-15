using Database;
using Dupes_Industrial_Overhaul.Chemical_Processing.Space;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class MeteorShowerAdjustments
	{
		public static void AddMeteorOreComet(GameplayEvents __instance)
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				(__instance.MeteorShowerDustEvent as MeteorShowerEvent).AddMeteor(HeavyCometConfig.ID, 3);
				(__instance.MeteorShowerIronEvent as MeteorShowerEvent).AddMeteor(HeavyCometConfig.ID, 2);
				(__instance.MeteorShowerCopperEvent as MeteorShowerEvent).AddMeteor(HeavyCometConfig.ID, 2);
				(__instance.MeteorShowerGoldEvent as MeteorShowerEvent).AddMeteor(HeavyCometConfig.ID, 2);
			}
		}
	}
}

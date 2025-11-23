using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Content.ModDb
{
	internal class Mod_Db
	{
		public static FloorLampPanes FloorLampPanes { get; set; }
		public static BigFossilVariants BigFossils { get; set; }

		public static Dictionary<SimHashes, List<IWeighted>> treasureHunterLoottable = new Dictionary<SimHashes, List<IWeighted>>()
		{

		};

		public static void PostDbInit(global::Db __instance)
		{
			FloorLampPanes = new FloorLampPanes();
			BigFossils = new BigFossilVariants();
			ModStatusItems.Register(__instance.BuildingStatusItems);
		}

		public static class BuildLocationRules
		{
			public static BuildLocationRule OnAnyWall = (BuildLocationRule)(-1569291063);
			public static BuildLocationRule GiantFossilRule = (BuildLocationRule)Hash.SDBMLower("DecorPackB_FloorOrHanging");
		}
	}
}

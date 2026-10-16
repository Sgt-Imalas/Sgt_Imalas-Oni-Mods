using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using static DistributionPlatform;

namespace CrittersFallThroughOpenDoors
{
	public class Mod : UserMod2
	{
		public static Harmony Harmony;
		public override void OnLoad(Harmony harmony)
		{
			Harmony = harmony;
			base.OnLoad(harmony);
			Debug.Log($"{this.mod.staticID} - Mod Version: {this.mod.packagedModInfo.version} ");
		}
	}
}

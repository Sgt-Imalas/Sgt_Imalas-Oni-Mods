using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace AkisSnowThings
{
	public class Mod : UserMod2
	{
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			HarmonyInstance = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}

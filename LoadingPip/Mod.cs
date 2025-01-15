using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace LoadingPip
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}

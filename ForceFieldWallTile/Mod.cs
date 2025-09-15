using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace ForceFieldWallTile
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);
			//DebugDevTool.Initialize();
			SgtLogger.LogVersion(this, harmony);
			ModAssets.LoadAssets();
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

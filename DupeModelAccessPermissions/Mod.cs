﻿using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using UtilLibs;

namespace DupeModelAccessPermissions
{
	public class Mod : UserMod2
	{
		public static HashSet<int> BionicMinionInstanceIds = new HashSet<int>();
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

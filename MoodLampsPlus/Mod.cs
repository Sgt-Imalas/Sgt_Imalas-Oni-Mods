using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace MoodLampsPlus
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			foreach(var mod in mods)
			{
				if (!mod.IsEnabledForActiveDlc())
					continue;

				if (mod.staticID == "AmogusMorb")
					ModAssets.AddAmogusLamps = false;
				else if (mod.staticID == "BathTub")
					ModAssets.AddBathtubLamp = false;
			}

			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

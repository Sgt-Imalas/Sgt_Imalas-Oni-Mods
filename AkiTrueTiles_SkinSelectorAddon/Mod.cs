using AkiTrueTiles_SkinSelectorAddon.Patches;
using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;

namespace AkiTrueTiles_SkinSelectorAddon
{
    public class Mod : UserMod2
    {
        public static bool TrueTilesEnabled = false;
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
        }       
        
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
			base.OnAllModsLoaded(harmony, mods);
			TrueTilesEnabled = mods.Any(mod=> mod.IsEnabledForActiveDlc() && mod.staticID.Contains("TrueTiles"));
            if (TrueTilesEnabled)
            {
                TrueTilesPatches.PatchAll(harmony);
                GamePatch.PatchAll(harmony);
			}

			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
        }
    }
}

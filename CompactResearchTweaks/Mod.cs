using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace CompactResearchTweaks
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
			SharedTweaks.ResearchNotificationMessageFix.ExecutePatch(harmony);
			SharedTweaks.ResearchScreenCollapseEntries.ExecutePatch(harmony);
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

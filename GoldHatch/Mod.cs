using GoldHatch.Creatures;
using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace GoldHatch
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
            HatchTuning.EGG_CHANCES_BASE.Add(new FertilityMonitor.BreedingChance() { egg = HatchGoldConfig.EGG_ID.ToTag(), weight = 0.02f });
            HatchTuning.EGG_CHANCES_HARD.Add(new FertilityMonitor.BreedingChance() { egg = HatchGoldConfig.EGG_ID.ToTag(), weight = 0.02f });
            HatchTuning.EGG_CHANCES_METAL.Add(new FertilityMonitor.BreedingChance() { egg = HatchGoldConfig.EGG_ID.ToTag(), weight = 0.22f });
        }
    }
}

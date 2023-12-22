using HarmonyLib;
using KMod;
using ShockWormMob.OreDeposits;
using System;
using UtilLibs;

namespace ShockWormMob
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony); OreDepositsConfig.GenerateAllDepositConfigs();
            SgtLogger.LogVersion(this, harmony);
        }
    }
}

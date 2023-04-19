using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace BetterCritterMorpher
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
        }
    }
}

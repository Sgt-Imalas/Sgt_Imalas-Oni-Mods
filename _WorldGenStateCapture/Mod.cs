using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace _WorldGenStateCapture
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            harmonyInstance = harmony;
            SgtLogger.LogVersion(this, harmony);
            THIS = this.mod;
        }
        public static Harmony harmonyInstance;
        public static KMod.Mod THIS;
    }
}

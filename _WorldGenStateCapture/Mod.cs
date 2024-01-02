using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using UtilLibs;

namespace _WorldGenStateCapture
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));
            base.OnLoad(harmony);
            harmonyInstance = harmony;
            SgtLogger.LogVersion(this, harmony);
            THIS = this.mod;
        }
        public static Harmony harmonyInstance;
        public static KMod.Mod THIS;
    }
}

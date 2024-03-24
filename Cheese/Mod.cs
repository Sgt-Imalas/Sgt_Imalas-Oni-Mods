using HarmonyLib;
using KMod;
using System;
using TUNING;
using UtilLibs;

namespace Cheese
{
    public class Mod : UserMod2
    {
        public static Harmony HarmonyInstance;
        public override void OnLoad(Harmony harmony)
        {
            HarmonyInstance = harmony;
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);

            ModAssets.LoadAll();
        }
    }
}

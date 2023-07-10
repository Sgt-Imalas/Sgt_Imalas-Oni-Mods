using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace DrywallPatternColours
{
    public class Mod : UserMod2
    {
        public static Harmony myHarmony;
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
            myHarmony = harmony;
        }
    }
}

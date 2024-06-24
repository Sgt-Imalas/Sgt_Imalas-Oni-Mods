using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace BathTub
{
    public class Mod : UserMod2
    {
        public static Harmony haromy;
        public override void OnLoad(Harmony harmony)
        {
            haromy = harmony;
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
        }
    }
}

using HarmonyLib;
using KMod;
using System;

namespace CrownMouldSkin
{
    public class Mod : UserMod2
    {
        public static Harmony HarmonyInstance;
        public override void OnLoad(Harmony harmony)
        {
            HarmonyInstance = harmony;
            base.OnLoad(harmony);
        }
    }
}

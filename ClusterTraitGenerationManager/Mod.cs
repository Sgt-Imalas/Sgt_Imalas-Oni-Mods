using HarmonyLib;
using KMod;
using System;

namespace ClusterTraitGenerationManager
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            ModAssets.LoadAssets();
        }
    }
}

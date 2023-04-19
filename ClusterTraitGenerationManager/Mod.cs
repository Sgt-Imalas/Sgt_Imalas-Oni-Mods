using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            ModAssets.LoadAssets();
            SgtLogger.LogVersion(this);
#if DEBUG
            Debug.LogError("Error THIS IS NOT RELEASE");
#endif
        }
    }
}

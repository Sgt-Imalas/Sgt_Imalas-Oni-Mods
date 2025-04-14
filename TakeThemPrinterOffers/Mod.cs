using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace TakeThemPrinterOffers
{
    public class Mod : UserMod2
    {
        public static Harmony HarmonyInstance;
        public override void OnLoad(Harmony harmony)
        {
			HarmonyInstance=harmony;

			base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
            ClusterManager.Instance.WorldContainers m_seasonIds

		}
    }
}

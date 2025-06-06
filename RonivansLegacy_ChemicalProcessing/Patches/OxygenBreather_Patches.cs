using Dupes_Industrial_Overhaul.Chemical_Processing.Disease;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class OxygenBreather_Patches
    {
		/// <summary>
		/// burn duplicants that step into acidic chemicals with chemical burns
		/// </summary>
		[HarmonyPatch(typeof(OxygenBreather), nameof(OxygenBreather.Sim200ms))]
        public class OxygenBreather_Sim200ms_Patch
        {
            static Dictionary<OxygenBreather,AcidBurnMonitor> acidBurnMonitors = new Dictionary<OxygenBreather, AcidBurnMonitor>();
			public static void Postfix(OxygenBreather __instance, float dt)
            {
                if(!acidBurnMonitors.TryGetValue(__instance,out var acidBurnMonitor))
                {
					acidBurnMonitor = __instance.gameObject.AddOrGet<AcidBurnMonitor>();
					acidBurnMonitors.Add(__instance, acidBurnMonitor);
				}
                acidBurnMonitor.CheckForAcidChemicals(dt);
            }
        }
    }
}

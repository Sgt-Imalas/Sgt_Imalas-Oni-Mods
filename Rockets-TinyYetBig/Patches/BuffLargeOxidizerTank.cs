using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    class BuffLargeOxidizerTank
    {
        [HarmonyPatch(typeof(OxidizerTankClusterConfig), "DoPostConfigureComplete")]
        public static class IncreaseCapacityto1350
        {
            public static void Postfix(GameObject go)
            {
                OxidizerTank oxidizerTank = go.AddOrGet<OxidizerTank>();
                oxidizerTank.targetFillMass = 1350f;
                oxidizerTank.maxFillMass = 1350f;
            }

        }
        
    }
}
//
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RebuildPreserve
{
    internal class CompatibilityPatches
    {
        public class Compatibility_FluidShipping
        {
            public static void ExecutePatch(Harmony harmony)
            {

                var m_TargetType = AccessTools.TypeByName("StormShark.OniFluidShipping.VesselInserter");

                var m_Prefix = AccessTools.Method(typeof(Compatibility_FluidShipping), "Prefix");
                if (m_TargetType != null)
                {
                    var m_TargetMethod = AccessTools.Method(m_TargetType, "OnCopySettings", new Type[] { typeof(object) });
                    if (m_TargetMethod == null)
                    {
                        SgtLogger.warning("StormShark.OniFluidShipping.VesselInserter.OnCopySettings not found");
                        return;
                    }
                    SgtLogger.l("adding condition check to StormShark.OniFluidShipping.VesselInserter.OnCopySettings");
                    harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix));
                }
                else
                {
                    SgtLogger.l("StormShark.OniFluidShipping mod target type VesselInserter not found.");
                }
            }
            public static bool Prefix(KMonoBehaviour __instance)
            {
                var m_TargetType = AccessTools.TypeByName("StormShark.OniFluidShipping.VesselInserter");
                return __instance.GetComponent(m_TargetType);
            }
        }
    }
}

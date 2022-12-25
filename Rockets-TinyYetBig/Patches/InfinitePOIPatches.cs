using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    internal class InfinitePOIPatches
    {
        [HarmonyPatch(typeof(HarvestablePOIStates.Instance))]
        [HarmonyPatch(nameof(HarvestablePOIStates.Instance.DeltaPOICapacity))]
        public static class InstaRecharge
        {
            public static void Postfix(HarvestablePOIStates.Instance __instance)
            {
                if (Config.Instance.InfinitePOI)
                {
                    __instance.poiCapacity = __instance.configuration.GetMaxCapacity();
                }
            }

        }

        [HarmonyPatch(typeof(SpacePOISimpleInfoPanel), "RefreshMassHeader")]
        public static class InstaRechargeStatusItem
        {
            public static void Postfix(HarvestablePOIStates.Instance harvestable, GameObject ___massHeader)
            {
                if (Config.Instance.InfinitePOI)
                {
                    HierarchyReferences component = ___massHeader.GetComponent<HierarchyReferences>();
                    component.GetReference<LocText>("ValueLabel").text = "<b>∞</b>";
                }
            }

        }
    }
}

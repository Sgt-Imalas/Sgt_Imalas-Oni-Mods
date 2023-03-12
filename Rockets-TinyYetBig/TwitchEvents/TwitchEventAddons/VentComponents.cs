using HarmonyLib;
using Rockets_TinyYetBig.Buildings.CargoBays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static Components;

namespace Rockets_TinyYetBig.TwitchEvents.TwitchEventAddons
{
    [HarmonyPatch(typeof(Vent), "OnSpawn")]
    public static class VentComponents
    {
        public static Cmps<Vent> Vents = new Cmps<Vent>();

        public static void Postfix(Vent __instance)
        {
            if(__instance.conduitType == ConduitType.Gas)
                Vents.Add(__instance);
        }
    }
    [HarmonyPatch(typeof(Building), "OnCleanUp")]
    public static class RemoveThem
    {
        public static void Prefix(Deconstructable __instance)
        {
            if (__instance.gameObject.TryGetComponent<Vent>(out var vent))
            {
                VentComponents.Vents.Remove(vent);
            }
        }
    }
}

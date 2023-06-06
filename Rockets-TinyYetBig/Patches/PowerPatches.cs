using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    internal class PowerPatches
    {
        //[HarmonyPatch(typeof(CircuitManager), "PowerFromBatteries")]
        //public static class PowerFromBatteries
        //{
        //    public static void Postfix(float joules_needed, List<Battery> batteries, IEnergyConsumer c)
        //    {
        //        SgtLogger.l(joules_needed + "", "JoulesNeeded");
        //        foreach(var battery in batteries)
        //        {
        //            SgtLogger.l(battery.Name + "", "Battery");
        //        }
        //        SgtLogger.l(c.Name + ", "+c.WattsUsed+", "+c.WattsNeededWhenActive, "watts");

        //    }
        //}
    }
}

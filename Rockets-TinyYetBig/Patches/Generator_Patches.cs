using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
	internal class Generator_Patches
	{
        ///// <summary>
        ///// Prevent engines from showing the "no power wire connected" statusitem
        ///// </summary>
        //[HarmonyPatch(typeof(Generator), nameof(Generator.SetStatusItem))]
        //public class Generator_SetStatusItem_Patch
        //{
        //    public static bool Prefix(Generator __instance)
        //    {
        //        return !__instance.TryGetComponent<RocketModuleCluster>(out _);
        //    }
        //}
        
	}
}

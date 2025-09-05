using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Tinkerable_Patches
	{

        [HarmonyPatch(typeof(Tinkerable), nameof(Tinkerable.OnPrefabInit))]
        public class Tinkerable_OnPrefabInit_Patch
		{
            public static void Postfix(Tinkerable __instance)
            {
               //SgtLogger.l("adding TinkerableCopySettingsHandler to "+__instance.GetProperName());
                __instance.gameObject.AddOrGet<TinkerableCopySettingsHandler>();
            }
        }
	}
}

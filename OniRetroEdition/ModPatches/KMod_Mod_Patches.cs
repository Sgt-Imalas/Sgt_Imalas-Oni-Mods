using HarmonyLib;
using Klei;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
	internal class KMod_Mod_Patches
	{

        [HarmonyPatch(typeof(KMod.Mod), nameof(KMod.Mod.LoadAnimation))]
        public class KMod_Mod_LoadAnimation_Patch
        {
            public static void Postfix(KMod.Mod __instance, ref bool __result)
			{
				if (__instance.staticID != Mod.Instance.mod.staticID)
                    return;

                SgtLogger.l("Loading Packed Kanims from Mod Folder");
				__result = InjectionMethods.LoadPackedKanims(System.IO.Path.Combine(IO_Utils.ModPath, "anim.zip"));
			}
        }
	}
}

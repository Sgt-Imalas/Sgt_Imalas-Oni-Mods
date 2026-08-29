using HarmonyLib;
using Klei;
using KMod;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class KMod_Mod_Patches
	{

        [HarmonyPatch(typeof(KMod.Mod), nameof(KMod.Mod.LoadAnimation))]
        public class KMod_Mod_LoadAnimation_Patch
        {
            public static void Postfix(KMod.Mod __instance, ref bool __result)
			{
				SgtLogger.l("KMod.Mod.LoadAnimation: "+ __instance.staticID);
				if (__instance.staticID != Mod.Instance.mod.staticID)
                    return;

                SgtLogger.l("Loading Packed Kanims from Mod Folder");
				__result = InjectionMethods.LoadAllPackedKanimsRecursively(FileSystem.Normalize(System.IO.Path.Combine(IO_Utils.ModPath, "anim_packed")));
			}
        }
	}
}

using AkisDecorPackB.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Patches
{
	internal class ModifierSet_Patches
	{

        [HarmonyPatch(typeof(ModifierSet), nameof(ModifierSet.Initialize))]
        public class ModifierSet_Initialize_Patch
		{
			public static void Postfix(ModifierSet __instance)
			{
				ModEffects.Register(__instance);
			}
		}
	}
}

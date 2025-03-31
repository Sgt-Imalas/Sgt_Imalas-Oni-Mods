using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class ValveBase_Patches
    {
		[HarmonyPatch(typeof(ValveBase), nameof(ValveBase.ConduitUpdate))]
		public class ValveBase_ConduitUpdate_Patch
		{
			public static void Prefix(ValveBase __instance, float dt)
			{
				if (ModAssets.TryGetCachedKbacs(__instance.gameObject, out var kbac, out var kbac2))
				{
					ModAssets.TryApplyConduitTint(__instance.conduitType, __instance.inputCell, kbac, kbac2);
				}
			}
		}
	}
}

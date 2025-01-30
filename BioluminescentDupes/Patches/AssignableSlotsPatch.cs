using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioluminescentDupes.Patches
{
	internal class AssignableSlotsPatch
	{
		[HarmonyPatch(typeof(AssignableSlots), MethodType.Constructor)]
		public class AssignableSlots_Ctor_Patch
		{
			public static void Postfix(AssignableSlots __instance)
			{
				ModAssets._AssignableSlots.Register(__instance);
			}
		}
	}
}

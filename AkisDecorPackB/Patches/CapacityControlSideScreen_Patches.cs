using AkisDecorPackB.Content.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Patches
{
	internal class CapacityControlSideScreen_Patches
	{

        [HarmonyPatch(typeof(CapacityControlSideScreen), nameof(CapacityControlSideScreen.IsValidForTarget))]
        public class CapacityControlSideScreen_IsValidForTarget_Patch
		{
			public static void Postfix(GameObject target, ref bool __result)
			{
				if (__result && target.TryGetComponent(out Pot pot))
					__result = pot.ShouldShowSettings;
			}
		}
	}
}

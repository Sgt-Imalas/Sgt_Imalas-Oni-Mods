using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemDropPrevention.Patches
{
	internal class RoverModifiers_Patches
	{

        [HarmonyPatch(typeof(RoverModifiers), nameof(RoverModifiers.OnBeginChore))]
        public class RoverModifiers_OnBeginChore_Patch
        {
            public static bool Prefix(RoverModifiers __instance)
            {
				return ModAssets.MarkForLaterDroppage(__instance.gameObject);
            }
        }
	}
}

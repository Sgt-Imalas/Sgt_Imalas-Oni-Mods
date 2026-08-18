using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BionicBoostersPlus.Patches
{
	internal class Electrobank_Patches
	{

        [HarmonyPatch(typeof(Electrobank), nameof(Electrobank.RemovePower))]
        public class Electrobank_RemovePower_Patch
        {
            public static bool Prefix(Electrobank __instance, ref float joules)
            {
                if (joules >= 0)
                    return true;

                //do not solar charge nonrechargeable electrobanks!
                if(!__instance.rechargeable)
                {
                    joules = 0;
                    return true;
                }
                
                __instance.AddPower(-joules);
                return false;
            }
        }
	}
}

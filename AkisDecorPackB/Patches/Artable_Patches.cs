using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Patches
{
	internal class Artable_Patches
	{

        [HarmonyPatch(typeof(Artable), nameof(Artable.SetDefault))]
        public class Artable_SetDefault_Patch
        {
            public static void Postfix(Artable __instance)
			{
				__instance.gameObject.Trigger(ModAssets.Hashes.FossilStageUnset);
			}
        }
	}
}

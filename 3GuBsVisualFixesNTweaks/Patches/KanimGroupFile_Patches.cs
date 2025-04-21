using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static MathUtil;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class KanimGroupFile_Patches
    {

        [HarmonyPatch(typeof(KAnimGroupFile), nameof(KAnimGroupFile.Load))]
        public class KAnimGroupFile_Load_Patch
		{
			private const string INTERACT_WARP_PORTAL = "anim_interacts_warp_portal_receiver_2_kanim";
			public static void Prefix(KAnimGroupFile __instance)
			{
				InjectionMethods.RegisterCustomInteractAnim(__instance, INTERACT_WARP_PORTAL);
			}
		}
		

		[HarmonyPatch(typeof(WarpReceiver), "ReceiveWarpedDuplicant")]
        public class WarpReceiverReceiveWarpedDuplicant_TargetMethod_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var index = codes.FindIndex(ci => ci.LoadsConstant("anim_interacts_warp_portal_receiver_kanim"));

                if (index == -1)
                {
                    return codes;
                }
                codes[index].operand = "anim_interacts_warp_portal_receiver_smoothed_kanim";


				return codes;
            }
        }
    }
}

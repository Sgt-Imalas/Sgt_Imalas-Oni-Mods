using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RoboRockets.Patches
{
	internal class NameFixPatch
	{
		[HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.TrySetRocketTitle))]
		[HarmonyPatch(new Type[] { typeof(ClustercraftExteriorDoor) })]
		public static class DetailsScreen_TrySetRocketTitle_Patch
		{
			public static bool GetTargetWorldNotNull(ClustercraftExteriorDoor clusterCraftDoor)
			{
				if (clusterCraftDoor != null && clusterCraftDoor.targetDoor == null)
					return false;

				return (clusterCraftDoor.GetTargetWorld() != null);
			}

			public static readonly MethodInfo TargetWorldCheck = AccessTools.Method(
			   typeof(DetailsScreen_TrySetRocketTitle_Patch),
			   nameof(GetTargetWorldNotNull));

			public static readonly MethodInfo Original = AccessTools.Method(
			   typeof(ClustercraftExteriorDoor),
			   nameof(ClustercraftExteriorDoor.HasTargetWorld));


			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();
				var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == Original);

				//foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
				if (insertionIndex != -1)
				{
					code[insertionIndex] = new CodeInstruction(OpCodes.Callvirt, TargetWorldCheck);
				}

				return code;
			}


			//public static bool Prefix(ClustercraftExteriorDoor clusterCraftDoor, DetailsScreen __instance, EditableTitleBar ___TabTitle, int ___setRocketTitleHandle)
			//{
			//    //Debug.Log(__instance.target.GetProperName() + ", target, world id: " +clusterCraftDoor.GetTargetWorld());

			//    if (clusterCraftDoor.GetTargetWorld() != null)
			//    {
			//        Debug.Log("target has World");
			//        WorldContainer targetWorld = clusterCraftDoor.GetTargetWorld();
			//        if ((UnityEngine.Object)targetWorld != (UnityEngine.Object)null)
			//        {
			//            ___TabTitle.SetTitle(targetWorld.GetComponent<ClusterGridEntity>().Name);
			//            ___TabTitle.SetUserEditable(true);
			//        }
			//        ___TabTitle.SetSubText(__instance.target.GetProperName());
			//        ___setRocketTitleHandle = -1;
			//    }
			//    else
			//    {
			//        if (___setRocketTitleHandle != -1)
			//            return false;
			//        ___setRocketTitleHandle = __instance.target.Subscribe(-71801987, (System.Action<object>)(clusterCraftDoor2 =>
			//        {
			//            __instance.OnRefreshData((object)null);
			//            __instance.target.Unsubscribe(___setRocketTitleHandle);
			//            ___setRocketTitleHandle = -1;
			//        }));
			//    }
			//    return false;
			//}
		}
	}
}

//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;

//namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
//{
//	class UtilityNetworkManager_Patches
//	{

//		[HarmonyPatch(typeof(UtilityNetworkManager<FlowUtilityNetwork, SolidConduit>), nameof(UtilityNetworkManager<FlowUtilityNetwork, SolidConduit>.RebuildNetworks))]
//		public class UtilityNetworkManager_RebuildNetworks_Patch
//		{
//			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
//			{

//				var checkBrokenState = AccessTools.Method(typeof(UtilityNetworkManager_RebuildNetworks_Patch), nameof(CheckIfSolidConduitIsBroken));
//				int obj1Index = 9; // if (this.items.TryGetValue(index2, out obj1))

//				foreach (var ci in orig)
//				{
//					if (ci.opcode == OpCodes.Isinst) //(obj1 is IDisconnectable
//					{
//						yield return new(OpCodes.Ldloc_S, obj1Index);
//						yield return new(OpCodes.Call, checkBrokenState);
//						yield return new(OpCodes.Brtrue, obj1Index,);
//						yield return ci;
//					}
//					else
//						yield return ci;
//				}
//			}

//			private static bool CheckIfSolidConduitIsBroken(object obj1)
//			{
				
//			}
//		}
//	}
//}

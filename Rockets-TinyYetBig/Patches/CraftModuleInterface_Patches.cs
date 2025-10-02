using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings.Engines;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class CraftModuleInterface_Patches
	{
		[HarmonyPatch(typeof(CraftModuleInterface), nameof(CraftModuleInterface.FuelRemaining), MethodType.Getter)]
		public static class FuelRemaining_Patch
		{
			/// <summary>
			/// for electric engines, check battery capacity and convert to fuel kg
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="__result"></param>
			public static void Postfix(CraftModuleInterface __instance, ref float __result)
			{
				if (__result == 0f)
				{
					return;
				}
				float totalBatteryJoules = 0f;
				ElectricEngineCluster targetEngine = null;
				foreach (Ref<RocketModuleCluster> clusterModule in __instance.clusterModules)
				{
					var md = clusterModule.Get();
					if (targetEngine == null && md.TryGetComponent<ElectricEngineCluster>(out var eng))
					{
						targetEngine = eng;
					}
					if (md.TryGetComponent<ModuleBattery>(out var battery))
					{
						totalBatteryJoules += battery.JoulesAvailable;
					}
				}
				if (targetEngine == null || !targetEngine.TryGetComponent<RocketModuleCluster>(out var module))
				{
					return;
				}

				float hexesRemaining_electricity = totalBatteryJoules / targetEngine.Joules_Per_Hex;
				//SgtLogger.l("TotalBatteryJoules: " + totalBatteryJoules);
				//SgtLogger.l("hexes : " + hexesRemaining_electricity);
				float fuelPerHex = module.performanceStats.fuelKilogramPerDistance;
				float remainingElectricity = hexesRemaining_electricity * fuelPerHex * 600f;
				//SgtLogger.l("remaining electricity : " + remainingElectricity);

				__result = Mathf.Min(__result, remainingElectricity);
			}
		}
		[HarmonyPatch(typeof(CraftModuleInterface), nameof(CraftModuleInterface.EvaluateConditionSet))]
		public class CraftModuleInterface_EvaluateConditionSet_Patch
		{
			/// <summary>
			/// Allow evaluation of the ribbon input of the advanced rocket platform
			/// </summary>
			public static ProcessCondition.Status ConvertWarningIfNeeded(ProcessCondition condition, CraftModuleInterface craftModuleInterface, ProcessCondition.ProcessConditionType conditionType)
			{

				var current = craftModuleInterface.CurrentPad;
				if (current != null && current.TryGetComponent(out AdvancedRocketStatusProvider evaluator))
				{
					///Only convert if it should launch or bit 3 is set (aka apply condition overrides permanently
					if (evaluator.GetBitMaskValAtIndex(0) || evaluator.GetBitMaskValAtIndex(3))
					{
						if (evaluator.GetBitMaskValAtIndex(1))
						{
							if (condition.GetType() == typeof(ConditionProperlyFueled) && condition.EvaluateCondition() != ProcessCondition.Status.Failure
							|| condition.GetType() == typeof(ConditionSufficientOxidizer) && condition.EvaluateCondition() != ProcessCondition.Status.Failure)
								return ProcessCondition.Status.Ready;
						}
						if (evaluator.GetBitMaskValAtIndex(2)
							&& (conditionType == ProcessCondition.ProcessConditionType.RocketStorage //regular storage conditions
							|| condition.GetType() == typeof(ConditionHasRadbolts)) && condition.EvaluateCondition() != ProcessCondition.Status.Failure) //radbolt storage
						{
							return ProcessCondition.Status.Ready;
						}
					}
				}
				return condition.EvaluateCondition();
			}

			private static readonly MethodInfo ConverterMethod = AccessTools.Method(
			  typeof(CraftModuleInterface_EvaluateConditionSet_Patch),
			  nameof(ConvertWarningIfNeeded)
		   );


			private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
					typeof(ProcessCondition),
					nameof(ProcessCondition.EvaluateCondition)
			   );

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();
				var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


				//InjectionMethods.PrintInstructions(code);
				if (insertionIndex != -1)
				{
					//int evaluatedConditionIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndex, false);
					code[insertionIndex] = new CodeInstruction(OpCodes.Call, ConverterMethod);
					code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_1));
					code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_0));
					//code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S,evaluatedConditionIndex));
					//code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_0));                    
				}
				else SgtLogger.error("TRANSPILER FAILED: AdvancedRocketPlatformAutolaunchPatch");

				return code;
			}

		}
	}
}

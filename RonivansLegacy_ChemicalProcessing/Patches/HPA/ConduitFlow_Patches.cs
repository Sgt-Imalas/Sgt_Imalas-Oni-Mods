using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	class ConduitFlow_Patches
	{
		private static readonly FieldInfo maxMass = AccessTools.Field(typeof(ConduitFlow), nameof(ConduitFlow.MaxMass));
		private static readonly MethodInfo conduitContentsAddMass = AccessTools.Method(typeof(ConduitFlow.ConduitContents), nameof(ConduitFlow.ConduitContents.AddMass));
		private static readonly MethodInfo replaceMaxMassAtCell = AccessTools.Method(typeof(ConduitFlow_Patches), nameof(ReplaceMaxMassAtCell));
		private static readonly MethodInfo doOverpressureDamageAtCell = AccessTools.Method(typeof(ConduitFlow_Patches), nameof(DoOverpressureDamageAtCell));

		///Modify MaxMass if needed for pressurized pipes when determining if the conduit is full.
		[HarmonyPatch(typeof(ConduitFlow), nameof(ConduitFlow.IsConduitFull))]
		public class ConduitFlow_IsConduitFull_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(ConduitFlow __instance, ref bool __result, int cell_idx)
			{
				if (__result && HighPressureConduitRegistration.TryGetHPACapacityAt(cell_idx, __instance.conduitType, out float increasedCap))
				{
					__result = increasedCap - __instance.grid[cell_idx].contents.mass <= 0;
				}
			}
		}

		///When Deserializing the contents inside of Conduits, the method will normally prevent the deserialized data from being higher than the built-in ConduitFlow MaxMass.
		///Instead, replace the max mass with infinity so the serialized mass will always be used.
		///Must be done this way because OnDeserialized is called before the Conduits are spawned, so no information is available as to what the max mass is supposed to be
		[HarmonyPatch(typeof(ConduitFlow), nameof(ConduitFlow.OnDeserialized))]
		public class ConduitFlow_OnDeserialized_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				// find injection point
				var index = codes.FindIndex(ci => ci.LoadsField(maxMass));

				if (index == -1)
				{
					SgtLogger.transpilerfail("ConduitFlow_OnDeserialized_Patch");
					return codes;
				}
				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(ConduitFlow_OnDeserialized_Patch), nameof(ReplaceMaxMass));
				// inject right after the found index
				codes.InsertRange(index + 1, [new CodeInstruction(OpCodes.Call, m_InjectedMethod)]);

				return codes;
			}
			internal static float ReplaceMaxMass(float original)
			{
				return float.PositiveInfinity;
			}
		}

		//Modify MaxMass if needed for pressurized pipes when update conduits. Also include overpressure integration
		[HarmonyPatch(typeof(ConduitFlow), nameof(ConduitFlow.UpdateConduit))]
		public class ConduitFlow_UpdateConduit_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				//variable: int cell2; index 13;
				///int cell2 = this.soaInfo.GetCell(conduitFromDirection.idx);
				///Debug.Assert(cell2 != -1);
				///ConduitFlow.ConduitContents contents1 = this.grid[cell2].contents;


				//This variable is used for the patch to determine the cell of the conduit being updated. The cell is then used in determining what its MaxMass (max capacity) should be
				CodeInstruction getCellInstruction = new CodeInstruction(OpCodes.Ldloc_S, 13);
				foreach (CodeInstruction code in orig)
				{
					foreach (CodeInstruction result in HandleMaxCapacityAndPressureDamage(code, getCellInstruction, true))
					{
						yield return result;
					}
				}
			}
		}

		[HarmonyPatch(typeof(ConduitFlow), nameof(ConduitFlow.AddElement))]
		public class ConduitFlow_AddElement_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				CodeInstruction getCellInstruction = new CodeInstruction(OpCodes.Ldarg_1); //int cell_idx : The first argument of the method being called (Ldarg_0 is the instance (this) reference)
				foreach (CodeInstruction code in orig)
				{
					foreach (CodeInstruction result in HandleMaxCapacityAndPressureDamage(code, getCellInstruction))
					{
						yield return result;
					}
				}
			}
		}

		///Replace references of ConduitFlow.MaxMass with a custom handler to determine if the MaxMass should be higher for pressurized pipes
		internal static IEnumerable<CodeInstruction> HandleMaxCapacityAndPressureDamage(CodeInstruction original, CodeInstruction getCellInstruction, bool isUpdateConduit = false)
		{
			///If the load field operand is being used to retrieve the maxMass field, override the maxMass value with our own max mass if necessary.
			///For example, if looking at a high pressure gas pipe, max mass will return 1000, even though we want the max mass of the high pressure pipe to be 3000.
			///The "toGetCell" code instruction will load the variable containing the cell index we need to look at
			if (original.LoadsField(maxMass))
			{
				yield return original; //old amount on stack
				yield return new CodeInstruction(OpCodes.Ldarg_0); //injecting conduitflow instance
				yield return getCellInstruction; //injecting cell
				yield return new CodeInstruction(OpCodes.Call, replaceMaxMassAtCell); //consume the three, returing a potentially changed max amount
			}

			///During the UpdateConduit method, the ConduitContents.AddMass method is called to move the contents from one pipe to the next.
			///In order to integrate an overpressure functionality, hook on just after the masses are added, and determine if overpressure damage needs to be dealt to the receiving conduit.
			else if (isUpdateConduit && original.Calls(conduitContentsAddMass))
			{
				yield return original;
				yield return new CodeInstruction(OpCodes.Ldarg_0); //instance
				yield return new CodeInstruction(OpCodes.Ldloc_2); //gridenode grid_node
				yield return getCellInstruction; //int cell2
				yield return new CodeInstruction(OpCodes.Call, doOverpressureDamageAtCell);
			}
			else
				yield return original;
		}

		///Replace max mass check if the conduit is HighPressure
		private static float ReplaceMaxMassAtCell(float standardMax, ConduitFlow conduitFlow, int cell_idx)
		{
			if(!HighPressureConduitRegistration.TryGetOutputHPACapacityAt(cell_idx, conduitFlow.conduitType, out float increasedCap))
				return standardMax;
			return increasedCap;
		}

		///Based on the passed variables, determine if overpressure damage should be dealt to the receiving conduit

		private static void DoOverpressureDamageAtCell(ConduitFlow conduitFlow, ConduitFlow.GridNode sender, int cell_idx)
		{
			///if the conduit is not high pressure, this check fails, therefore it should receive damage
			HighPressureConduitEventHandler.PressureDamageHandling(cell_idx,conduitFlow.conduitType, sender.contents.mass, HighPressureConduitRegistration.GetMaxConduitCapacityAt(cell_idx, conduitFlow.conduitType));
		}		
	}
}

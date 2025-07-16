using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class SolidConduitDispenser_Patches
	{

		[HarmonyPatch(typeof(SolidConduitDispenser), nameof(SolidConduitDispenser.ConduitUpdate))]
		public class SolidConduitDispenser_ConduitUpdate_Patch
		{
			public static ConfigurableSolidConduitDispenser configDispenserInstance;
			public static SolidConduitDispenser dispenserInstance;
			public static void Prefix(SolidConduitDispenser __instance)
			{
				if (__instance is ConfigurableSolidConduitDispenser dispenser)
				{
					configDispenserInstance = dispenser;
				}
				else
					configDispenserInstance = null;
				dispenserInstance = __instance;
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();
				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(SolidConduitDispenser_ConduitUpdate_Patch), "ReplaceCapacityConditionally");

				double nr = 20;

				for (int i = codes.Count - 1; i >= 0; i--)
				{
					var current = codes[i];
					if (current.LoadsConstant(nr))
					{
						codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, m_InjectedMethod));
					}
				}
				return codes;
			}

			private static double ReplaceCapacityConditionally(double input)
			{
				if (configDispenserInstance != null)
				{
					return configDispenserInstance.massDispensed;
				}
				else if (dispenserInstance != null && LogisticConduit.HasLogisticConduitAt(dispenserInstance.utilityCell))
				{
					return HighPressureConduit.SolidCap_Logistic;
				}
				return input;
			}
		}
	}
}

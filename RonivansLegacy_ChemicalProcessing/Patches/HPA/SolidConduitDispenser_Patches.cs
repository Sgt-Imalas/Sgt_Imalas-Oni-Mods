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
						codes.InsertRange(i + 1,
							[new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, m_InjectedMethod)]);
					}
				}
				return codes;
			}

			private static double ReplaceCapacityConditionally(double input, SolidConduitDispenser instance)
			{
				if (instance == null)
					return input;

				if (HighPressureConduitRegistration.IsDynamicSolidConduitDispenser(instance))
				{	
					//use whatever the attached rail supports
					return HighPressureConduitRegistration.GetMaxConduitCapacityAt(instance.utilityCell, ConduitType.Solid);
				}
				else if (LogisticConduit.HasLogisticConduitAt(instance.utilityCell))
				{
					//use logistic conduit mass
					return HighPressureConduitRegistration.SolidCap_Logistic;
				}
				else if (instance is ConfigurableSolidConduitDispenser configurable)
				{
					//use whats configured
					return configurable.massDispensed;
				}
				//use default (20)
				return input;
			}
		}
	}
}

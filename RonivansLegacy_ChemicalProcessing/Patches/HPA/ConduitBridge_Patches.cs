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
using static RonivansLegacy_ChemicalProcessing.Patches.HPA.ValveBase_Patches;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
    class ConduitBridge_Patches
    {
		//Integrate a max flow rate specifically for ConduitBridges (i.e. Gas Bridge)
		//Normally, a Gas Bridge can move any amount of gas from one pressurized pipe to another pressurized pipe, since inputs and outputs for buildings have no built in limiter to their flow rate.
		//For ConduitBridges, limit standard bridges to the same standard max flow rate as their respective conduits (1KG for gas bridge and 10KG for liquid bridge).
		[HarmonyPatch(typeof(ConduitBridge), nameof(ConduitBridge.ConduitUpdate))]
        public class ConduitBridge_ConduitUpdate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				MethodInfo ConduitFlow_GetContents = AccessTools.Method(typeof(ConduitFlow), nameof(ConduitFlow.GetContents));
				MethodInfo setMaxFlowForBridge = AccessTools.Method(typeof(ConduitBridge_ConduitUpdate_Patch), nameof(SetMaxFlowForBridge));

				foreach (CodeInstruction original in instructions)
				{
					if (original.Calls(ConduitFlow_GetContents))
					{
						yield return original; //flowManager.GetContents(inputCell)
						yield return new CodeInstruction(OpCodes.Ldarg_0); //this
						yield return new CodeInstruction(OpCodes.Ldloc_0); //ConduitFlow flowManager
						yield return new CodeInstruction(OpCodes.Call, setMaxFlowForBridge); //SetMaxFlow(flowManager.GetContents
					}
					else
						yield return original;
				}
			}
			static float targetConduitCapacity, targetBridgeCapacity, conduitMass;
			static bool isHPBridge, hasHPTargetConduit;
			static int sourceCell, targetCell;
			static ConduitType type;

			private static ConduitFlow.ConduitContents SetMaxFlowForBridge(ConduitFlow.ConduitContents contents, ConduitBridge bridge, ConduitFlow manager)
			{
				//If the bridge is broken, prevent the bridge from operating by limiting what it sees.
				conduitMass = contents.mass;

				if (bridge.GetComponent<BuildingHP>().IsBroken)
				{
					//does not actually remove mass from the conduit, just causes the bridge to assume there is no mass available to move.
					contents.RemoveMass(conduitMass);
					return contents;
				}
				type = bridge.type;
				targetCell = bridge.outputCell;

				isHPBridge = HighPressureConduitRegistration.HasHighPressureConduitAt(targetCell, type, true);
				hasHPTargetConduit = HighPressureConduitRegistration.HasHighPressureConduitAt(targetCell, type);

				//target conduit is high pressure, bridge is high pressure -> no damage case
				if (isHPBridge && hasHPTargetConduit)
					return contents;


				targetConduitCapacity = HighPressureConduitRegistration.GetMaxConduitCapacityAt(targetCell, type);

				//no pipe at output cell of bridge
				if (!HighPressureConduitRegistration.HasConduitAt(targetCell,type))
					return contents;

				targetBridgeCapacity = HighPressureConduitRegistration.GetMaxConduitCapacityAt(targetCell, type,true);

				//damage the bridge when the target pipe is a HPA bridge 
				if (conduitMass > targetBridgeCapacity * 1.1f)
					HighPressureConduitEventHandler.ScheduleForDamage(bridge.gameObject, (int)conduitMass, (int)targetBridgeCapacity);

				//If the ConduitBridge is not supposed to support the amount of fluid currently in the contents, only make the bridge's intended max visible
				//Also immediately deal damage if the current contents are higher than 110% of the intended max (110% is set because at 100%, a system with no pressurized pipes would seem to randomly deal damage as if the contents
				//  were barely over 100%
				if (contents.mass > targetBridgeCapacity)
				{
					float initial = contents.mass;
					float removed = contents.RemoveMass(initial - targetBridgeCapacity);
					float ratio = removed / initial;
					contents.diseaseCount = (int)((float)contents.diseaseCount * ratio);
				}
				///damage target conduit if it got too much mass transfered to it
				HighPressureConduitEventHandler.PressureDamageHandling(targetCell, type, contents.mass, targetConduitCapacity);
				return contents;
			}
		}
    }
}

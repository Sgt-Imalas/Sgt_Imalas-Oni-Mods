using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	class SolidConduitBridge_Patches
	{
		[HarmonyPatch(typeof(SolidConduitBridge), nameof(SolidConduitBridge.ConduitUpdate))]
		public class SolidConduitBridge_ConduitUpdate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HPA_Rails_Mod_Enabled;
			internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				MethodInfo SolidConduitFlow_RemovePickupable = AccessTools.Method(typeof(SolidConduitFlow), nameof(SolidConduitFlow.RemovePickupable));
				MethodInfo setMaxFlowForBridge = AccessTools.Method(typeof(SolidConduitBridge_ConduitUpdate_Patch), nameof(SetMaxFlowForBridge));

				foreach (CodeInstruction original in instructions)
				{
					if (original.Calls(SolidConduitFlow_RemovePickupable))
					{
						yield return original; // flowManager.RemovePickupable(this.inputCell);
						yield return new CodeInstruction(OpCodes.Ldarg_0); //this
						yield return new CodeInstruction(OpCodes.Call, setMaxFlowForBridge); //SetMaxFlow(flowManager.GetContents
					}
					else
						yield return original;
				}
			}

			private static Pickupable SetMaxFlowForBridge(Pickupable item, SolidConduitBridge bridge)
			{
				if (item == null)
					return item;

				int bridge_inputCell = bridge.inputCell;
				int bridge_outputCell = bridge.outputCell;

				bool SourceCellInsulated = HighPressureConduitRegistration.IsInsulatedRail(bridge_inputCell);
				bool TargetCellInsulated = HighPressureConduitRegistration.IsInsulatedRail(bridge_outputCell);
				if (TargetCellInsulated != SourceCellInsulated)
				{
					HighPressureConduitRegistration.SetInsulatedState(item, TargetCellInsulated);
				}

				///ignore items with custom unit mass
				if (item.PrimaryElement.MassPerUnit != 1)
				{
					//SgtLogger.l(pickupable.gameObject.GetProperName() + " had a special unit mass");
					return item;
				}

				float mass = item.TotalAmount;
				//If the bridge is broken, prevent the bridge from operating by limiting what it sees.
				//if (bridge.GetComponent<BuildingHP>().IsBroken)
				//{
				//	return DumpItem(item, mass, bridge.inputCell, bridge.gameObject);
				//}

				bool isHPBridge = HighPressureConduitRegistration.HasHighPressureConduitAt(bridge_outputCell, ConduitType.Solid, true);
				bool hasHPTargetConduit = HighPressureConduitRegistration.HasHighPressureConduitAt(bridge_outputCell, ConduitType.Solid);

				//target conduit is high pressure, bridge is high pressure -> no damage case
				if (isHPBridge && hasHPTargetConduit)
					return item;
								
				float targetConduitCapacity = HighPressureConduitRegistration.GetMaxConduitCapacityAt(bridge_outputCell, ConduitType.Solid);

				//no pipe at output cell of bridge
				if (!HighPressureConduitRegistration.HasConduitAt(bridge_outputCell, ConduitType.Solid))
					return item;

				float targetBridgeCapacity = HighPressureConduitRegistration.GetMaxConduitCapacityAt(bridge_outputCell, ConduitType.Solid, true);

				float targetMass = Mathf.Min(targetConduitCapacity, targetBridgeCapacity);


				///damage the bridge when the amount transferred is higher than the bridge can support
				//	if (mass > targetBridgeCapacity * 1.1f)
				//HighPressureConduit.ScheduleForDamage(bridge.gameObject);

				//If the ConduitBridge is not supposed to support the amount of fluid currently in the contents, only make the bridge's intended max visible
				//Also immediately deal damage if the current contents are higher than 110% of the intended max (110% is set because at 100%, a system with no pressurized pipes would seem to randomly deal damage as if the contents
				//  were barely over 100%
				if (mass > targetMass+0.001f)
				{
					return HighPressureConduitRegistration.DumpItem(item, mass, targetMass, bridge_inputCell, bridge.gameObject);
				}
				///damage target conduit if it got too much mass transfered to it
				//HighPressureConduit.PressureDamageHandling(targetConduit, mass, targetConduitCapacity);
				return item;
			}
		}
	}
}

using HarmonyLib;
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
			public static bool Prepare() => Config.Instance.HighPressureApplications;
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

				///ignore items with custom unit mass
				if (item.PrimaryElement.MassPerUnit != 1)
				{
					//SgtLogger.l(pickupable.gameObject.GetProperName() + " had a special unit mass");
					return item;
				}

				var mass = item.TotalAmount;
				//If the bridge is broken, prevent the bridge from operating by limiting what it sees.
				//if (bridge.GetComponent<BuildingHP>().IsBroken)
				//{
				//	return DumpItem(item, mass, bridge.inputCell, bridge.gameObject);
				//}
				var cell = bridge.outputCell;

				bool isHPBridge = HighPressureConduit.HasHighPressureConduitAt(cell, ConduitType.Solid, true);
				bool hasHPTargetConduit = HighPressureConduit.HasHighPressureConduitAt(cell, ConduitType.Solid);

				//target conduit is high pressure, bridge is high pressure -> no damage case
				if (isHPBridge && hasHPTargetConduit)
					return item;

				float targetConduitCapacity = HighPressureConduit.GetMaxConduitCapacityAt(bridge.outputCell, ConduitType.Solid, out var targetConduit);

				//no pipe at output cell of bridge
				if (targetConduit == null)
					return item;

				float targetBridgeCapacity = HighPressureConduit.GetMaxConduitCapacityAt(bridge.outputCell, ConduitType.Solid, out _, true);

				float targetMass = Mathf.Min(targetConduitCapacity, targetBridgeCapacity);


				///damage the bridge when the amount transferred is higher than the bridge can support
				//	if (mass > targetBridgeCapacity * 1.1f)
				//HighPressureConduit.ScheduleForDamage(bridge.gameObject);

				//If the ConduitBridge is not supposed to support the amount of fluid currently in the contents, only make the bridge's intended max visible
				//Also immediately deal damage if the current contents are higher than 110% of the intended max (110% is set because at 100%, a system with no pressurized pipes would seem to randomly deal damage as if the contents
				//  were barely over 100%
				if (mass > targetMass)
				{
					return DumpItem(item, mass, targetMass, bridge.inputCell, bridge.gameObject);
				}
				///damage target conduit if it got too much mass transfered to it
				//HighPressureConduit.PressureDamageHandling(targetConduit, mass, targetConduitCapacity);
				return item;
			}
			static Pickupable DumpItem(Pickupable pickupable, float mass, float targetMass, int dumpCell, GameObject target)
			{
				float amountToDump = (mass - targetMass);
				var droppedExcess = pickupable.Take(amountToDump);
				///drop excess mass
				droppedExcess.transform.SetPosition(Grid.CellToPosCCC(dumpCell, Grid.SceneLayer.Ore));
				SolidConduit.GetFlowManager().DumpPickupable(droppedExcess);
				HighPressureConduit.ScheduleDropNotification(target,(int) mass, (int)targetMass);
				return pickupable;
			}
		}
	}
}

using HarmonyLib;
using rail;
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
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	class SolidConduitFlow_Patches
	{
		public static SolidConduitFlow Instance;
		//Drop items off conveyor rail and damage the rail if it doesnt support the capacity
		[HarmonyPatch(typeof(SolidConduitFlow), nameof(SolidConduitFlow.UpdateConduit))]
		public class SolidConduitFlow_UpdateConduit_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications || Config.Instance.DupesLogistics;

			public static void Prefix(SolidConduitFlow __instance) => Instance = __instance;


			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig, MethodBase __originalMethod)
			{
				var codes = orig.ToList();
				MethodInfo dropExcessRailMaterialsAtCell = AccessTools.Method(typeof(SolidConduitFlow_Patches), nameof(DropExcessRailMaterialsAtCell));
				MethodInfo SolidConduitFlow_RemoveFromGrid = AccessTools.Method(typeof(SolidConduitFlow), nameof(SolidConduitFlow.RemoveFromGrid));
				var conduitIndex = __originalMethod.GetMethodBody().LocalVariables.Last(variable => variable.LocalType == typeof(SolidConduitFlow.Conduit)).LocalIndex;
				var targetCellIndex = __originalMethod.GetMethodBody().LocalVariables.Last(variable => variable.LocalType == typeof(int)).LocalIndex;

				SgtLogger.l("conduit index: " + conduitIndex + ", targetcell index: " + targetCellIndex);

				foreach (CodeInstruction ci in orig)
				{
					if (ci.Calls(SolidConduitFlow_RemoveFromGrid))
					{
						yield return ci;
						yield return new CodeInstruction(OpCodes.Ldloc_S, conduitIndex); //Conduit
						yield return new CodeInstruction(OpCodes.Ldloc_S, targetCellIndex); //cell
						yield return new CodeInstruction(OpCodes.Call, dropExcessRailMaterialsAtCell);
					}
					else
						yield return ci;
				}
			}
		}
		private static SolidConduitFlow.ConduitContents DropExcessRailMaterialsAtCell(SolidConduitFlow.ConduitContents contents, SolidConduitFlow.Conduit source, int targetcell)
		{
			int sourceCell = Instance.soaInfo.GetCell(source.idx);
			Pickupable pickupable = Instance.GetPickupable(contents.pickupableHandle);
			///ignore items that have a custom weight per unit
			if (pickupable.PrimaryElement.MassPerUnit != 1)
			{
				//SgtLogger.l(pickupable.gameObject.GetProperName() + " had a special unit mass");
				return contents;
			}
			float weight = pickupable.TotalAmount;
			float maxTargetRailCapacity = HighPressureConduit.SolidCap_Logistic;
			GameObject targetRail;

			if (!LogisticConduit.TryGetLogisticConduitAt(targetcell, false, out targetRail))
			{
				maxTargetRailCapacity = HighPressureConduit.GetMaxConduitCapacityAt(targetcell, ConduitType.Solid, out targetRail);
				SgtLogger.l("rail at targetcell " + targetcell + " is regular rail");

			}
			else
				SgtLogger.l("rail at targetcell " + targetcell + " is logistic rail");

				maxTargetRailCapacity += 0.01f; //adding 10 grams to avoid floating point errors dropping micrograms of items

			SgtLogger.l("Current Item Weight: "+weight+", target weight: "+maxTargetRailCapacity);

			if (weight <= maxTargetRailCapacity)
				return contents;
			
			if (weight > maxTargetRailCapacity)
			{
				float additionalWeightToRemove = (weight - maxTargetRailCapacity);
				var droppedExcess = pickupable.Take(additionalWeightToRemove);
				///drop excess mass
				Instance.DumpPickupable(droppedExcess);
				float ratio = additionalWeightToRemove / weight;

				SgtLogger.l($"Dropped {ratio * 100}% of mass on solid conduit");
			}
			return contents;
		}
	}
}

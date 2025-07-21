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
		public static SolidConduitFlow.Conduit Conduit;
		//Drop items off conveyor rail and damage the rail if it doesnt support the capacity
		[HarmonyPatch(typeof(SolidConduitFlow), nameof(SolidConduitFlow.UpdateConduit))]
		public class SolidConduitFlow_UpdateConduit_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;// || Config.Instance.DupesLogistics;

			public static void Prefix(SolidConduitFlow __instance, SolidConduitFlow.Conduit conduit)
			{
				Instance = __instance;
				Conduit = conduit;
			}


			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig, MethodBase __originalMethod)
			{
				var codes = orig.ToList();
				MethodInfo dropExcessRailMaterialsAtCell = AccessTools.Method(typeof(SolidConduitFlow_Patches), nameof(DropExcessRailMaterialsAtCell));
				MethodInfo SolidConduitFlow_RemoveFromGrid = AccessTools.Method(typeof(SolidConduitFlow), nameof(SolidConduitFlow.RemoveFromGrid));

				///this results in the wrong target, bc we need the one at index 4, but its nice to copy it later
				//var targetCellIndex = __originalMethod.GetMethodBody().LocalVariables.Last(variable => variable.LocalType == typeof(int)).LocalIndex;

				var targetCellIndex = 4; //int cell = this.soaInfo.GetCell(conduitFromDirection1.idx); // cell iterator


				foreach (CodeInstruction ci in orig)
				{
					if (ci.Calls(SolidConduitFlow_RemoveFromGrid))
					{
						yield return ci;
						yield return new CodeInstruction(OpCodes.Ldloc_S, targetCellIndex); //cell
						yield return new CodeInstruction(OpCodes.Call, dropExcessRailMaterialsAtCell);
					}
					else
						yield return ci;
				}
			}
		}
		private static SolidConduitFlow.ConduitContents DropExcessRailMaterialsAtCell(SolidConduitFlow.ConduitContents contents, int targetcell)
		{
			int sourceCell = Instance.soaInfo.GetCell(Conduit.idx);
			Pickupable pickupable = Instance.GetPickupable(contents.pickupableHandle);

			bool SourceCellInsulated = HighPressureConduitRegistration.IsInsulatedRail(sourceCell);
			bool TargetCellInsulated = HighPressureConduitRegistration.IsInsulatedRail(targetcell);
			if (TargetCellInsulated != SourceCellInsulated)
			{
				HighPressureConduitRegistration.SetInsulatedState(pickupable, TargetCellInsulated);
			}

			///ignore items that have a custom weight per unit
			if (pickupable.PrimaryElement.MassPerUnit != 1)
			{
				//SgtLogger.l(pickupable.gameObject.GetProperName() + " had a special unit mass");
				return contents;
			}
			float weight = pickupable.TotalAmount;
			float maxTargetRailCapacity = HighPressureConduitRegistration.SolidCap_Logistic;
			GameObject sourceRail, targetRail;

			if (!LogisticConduit.TryGetLogisticConduitAt(sourceCell, false, out sourceRail))
				maxTargetRailCapacity = HighPressureConduitRegistration.GetMaxConduitCapacityWithConduitGOAt(sourceCell, ConduitType.Solid, out sourceRail);

			HighPressureConduitRegistration.GetMaxConduitCapacityWithConduitGOAt(sourceCell, ConduitType.Solid, out targetRail);

			

			maxTargetRailCapacity += 0.0001f; //adding a tiny amount to avoid floating point errors dropping micrograms of items
			//SgtLogger.l("Current Item Weight: " + weight + ", target weight: " + maxTargetRailCapacity+" with source and target: "+sourceCell+","+targetcell);



			if (weight <= maxTargetRailCapacity)
				return contents;

			if (weight > maxTargetRailCapacity)
			{
				///alt variant: drop everything if weight too hight
				//Instance.DumpPickupable(pickupable.Take(weight));

				//float weightAboveThreshold = (weight - maxTargetRailCapacity);
				//float chanceToDamage = weightAboveThreshold / weight;
				//if(UnityEngine.Random.value < chanceToDamage) //the more overloaded the rail is, the higher the chance for damage
				//{
				//	HighPressureConduit.ScheduleForDamage(sourceRail);
				//}
				//return SolidConduitFlow.ConduitContents.EmptyContents();
				
				///using this variant for now because the rail system doesnt react to damage...
				///alt variant: drop only excess amount if weight too hight, keep target limit on the rail
				float additionalWeightToRemove = (weight - maxTargetRailCapacity);
				var droppedExcess = pickupable.Take(additionalWeightToRemove);
				///drop excess mass
				Instance.DumpPickupable(droppedExcess);
				float ratio = additionalWeightToRemove / weight;
				////SgtLogger.l($"Dropped {ratio * 100}% of mass on solid conduit");
			}
			return contents;
		}


		[HarmonyPatch(typeof(SolidConduitFlow), nameof(SolidConduitFlow.DumpPickupable))]
		public class SolidConduitFlow_DumpPickupable_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Prefix(SolidConduitFlow __instance, Pickupable pickupable)
			{
				HighPressureConduitRegistration.SetInsulatedState(pickupable, false);
			}
		}

		[HarmonyPatch(typeof(SolidConduitFlow), nameof(SolidConduitFlow.EmptyConduit))]
		public class SolidConduitFlow_EmptyConduit_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();
				MethodInfo restoreHeatTransfer = AccessTools.Method(typeof(SolidConduitFlow_EmptyConduit_Patch), nameof(RestoreHeatTransfer));
				MethodInfo SolidConduitFlow_RemovePickupable = AccessTools.Method(typeof(SolidConduitFlow), nameof(SolidConduitFlow.RemovePickupable));


				foreach (CodeInstruction ci in orig)
				{
					if (ci.Calls(SolidConduitFlow_RemovePickupable))
					{
						yield return ci;
						yield return new CodeInstruction(OpCodes.Call, restoreHeatTransfer);
					}
					else
						yield return ci;
				}
			}

			private static Pickupable RestoreHeatTransfer(Pickupable itemToRestore) => HighPressureConduitRegistration.SetInsulatedState(itemToRestore, false);
		}

		[HarmonyPatch(typeof(SolidConduitFlow), nameof(SolidConduitFlow.AddPickupable))]
		public class SolidConduitFlow_AddPickupable_Patch
		{
			public static void Postfix(SolidConduitFlow __instance, int cell_idx, Pickupable pickupable)
			{
				if(HighPressureConduitRegistration.IsInsulatedRail(cell_idx))
				{
					HighPressureConduitRegistration.SetInsulatedState(pickupable, true);
				}
				else
				{
					HighPressureConduitRegistration.SetInsulatedState(pickupable, false);
				}
			}
		}

	}
}

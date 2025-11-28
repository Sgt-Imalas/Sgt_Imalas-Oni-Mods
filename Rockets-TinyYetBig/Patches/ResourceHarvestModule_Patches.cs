using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class ResourceHarvestModule_Patches
	{
		[HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), MethodType.Constructor, [typeof(IStateMachineTarget), typeof(ResourceHarvestModule.Def)])]
		public class ResourceHarvestModule_Constructor_Patch
		{
			public static void Postfix(ResourceHarvestModule.StatesInstance __instance, IStateMachineTarget master, ResourceHarvestModule.Def def)
			{
				if (master.gameObject.TryGetComponent(out ResourceHarvestModuleHEPInjector hepHarvester)|| master.gameObject.TryGetComponent(out hepHarvester))
				{
					hepHarvester.Inject_Constructor(__instance, master, def);
				}
			}
		}

		[HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.UpdateMeter))]
		public class ResourceHarvestModule_StatesInstance_UpdateMeter_Patch
		{
			public static bool Prefix(ResourceHarvestModule.StatesInstance __instance)
			{
				if (__instance.master.gameObject.TryGetComponent(out ResourceHarvestModuleHEPInjector hepHarvester))
				{
					hepHarvester.Inject_UpdateMeter();
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.ConsumeDiamond))]
		public class ResourceHarvestModule_StatesInstance_ConsumeDiamond_Patch
		{
			public static bool Prefix(ResourceHarvestModule.StatesInstance __instance, float amount)
			{
				if (__instance.master.gameObject.TryGetComponent(out ResourceHarvestModuleHEPInjector hepHarvester))
				{
					hepHarvester.Inject_ConsumeDiamond(amount);
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.HasAnyAmountOfDiamond))]
		public class ResourceHarvestModule_StatesInstance_HasAnyAmountOfDiamond_Patch
		{
			public static bool Prefix(ResourceHarvestModule.StatesInstance __instance, ref bool __result)
			{
				if (__instance.master.gameObject.TryGetComponent(out ResourceHarvestModuleHEPInjector hepHarvester))
				{
					__result = hepHarvester.Inject_HasAnyAmountOfDiamond();
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.GetMaxExtractKGFromDiamondAvailable))]
		public class ResourceHarvestModule_StatesInstance_GetMaxExtractKGFromDiamondAvailable_Patch
		{
			public static bool Prefix(ResourceHarvestModule.StatesInstance __instance, ref float __result)
			{
				if (__instance.master.gameObject.TryGetComponent(out ResourceHarvestModuleHEPInjector hepHarvester))
				{
					__result = hepHarvester.Inject_GetMaxExtractKGFromDiamondAvailable();
					return false;
				}
				return true;
			}
		}
		//[HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.CheckIfCanDrill))]
		//public class ResourceHarvestModule_StatesInstance_CheckIfCanDrill_Patch
		//{
		//	public static bool Prefix(ResourceHarvestModule.StatesInstance __instance, ref bool __result)
		//	{
		//		if (__instance.master.gameObject.TryGetComponent(out ResourceHarvestModuleHEPInjector hepHarvester))
		//		{
		//			__result = hepHarvester.Inject_CheckIfCanDrill();
		//			return false;
		//		}
		//		return true;
		//	}
		//}



		/// <summary>
		/// Triggers reevaluation of pilot digging skills
		/// </summary>
		[HarmonyPatch(typeof(ResourceHarvestModule), nameof(ResourceHarvestModule.InitializeStates))]
		public static class TriggerStartTaskForEvaluation
		{
			public static void Postfix(ResourceHarvestModule __instance)
			{
				__instance.not_grounded.EventHandler(GameHashes.OnParticleStorageChanged, (smi)=>
				{
					smi.CheckIfCanDrill();
				});
				__instance.not_grounded.drilling.Enter(smi =>
				{
					if (!smi.master.gameObject.TryGetComponent<RocketModuleCluster>(out var rmc))
						return;
					var clusterCraft = rmc.CraftInterface.m_clustercraft;

					if (!clusterCraft.TryGetComponent<WorldContainer>(out var worldContainer))
						return;

					try
					{
						var controlStations = Components.RocketControlStations.GetWorldItems(worldContainer.id);
						if (controlStations != null && controlStations.Count > 0)
						{
							var station = controlStations.First();
							if (station != null)
							{
								station.smi.sm.CreateLaunchChore(station.smi);
							}
						}
					}
					catch (Exception ex)
					{
						SgtLogger.warning("Regular drillcone has encountered an error trying to reevaluate piloting skills:");
						SgtLogger.error(ex.Message);
					}
				});
			}
		}

		/// <summary>
		/// Adjusts the drilling speed of drillcone SMI to include support module speed boosts
		/// </summary>
		[HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.HarvestFromPOI))]
		public static class AddDrillconeHarvestSpeedBuff
		{
			
			//since these arent running in parallel, caching that for the transpiler below. initialisation value = NoseconeHarvestConfig.solidCapacity/NoseconeHarvestConfig.timeToFill
			static float actualMiningSpeed = ROCKETRY.SOLID_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE / 3600f;
			public static void Prefix(ResourceHarvestModule.StatesInstance __instance)
			{
				if (__instance.gameObject.TryGetComponent<RocketModuleCluster>(out var Module))
				{
					float SupportModuleCount = 0;
					foreach (var otherModule in Module.CraftInterface.ClusterModules)
					{
						if (otherModule.Get().TryGetComponent<DrillConeAssistentModule>(out _))
						{
							++SupportModuleCount;
						}
					}
					//SgtLogger.debuglog(__instance + ", BooserCount: " + SupportModuleCount);
					// __instance.def.harvestSpeed;
					actualMiningSpeed = (1f + SupportModuleCount * Config.Instance.DrillconeSupportSpeedBoost / 100f) * ModAssets.DefaultDrillconeHarvestSpeed * ModAssets.GetMiningPilotSkillMultiplier(Module.CraftInterface.m_clustercraft);
				}
			}
			/// <summary>
			/// replace the base mining speed value with the calculated cached one from above
			/// </summary>
			/// <param name="instructions"></param>
			/// <param name="il"></param>
			/// <returns></returns>
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();

				for (int i = code.Count - 1; i >= 0; --i)
				{
					var ci = code[i];
					if (ci.LoadsField(ResourceHarvestModuleDef_harvestSpeed))
					{
						//code.Insert(i+1, new CodeInstruction(OpCodes.Ldarg_0));
						code.Insert(i + 1, new CodeInstruction(OpCodes.Call, OverrideDefMiningSpeedMethod));
					}
				}

				return code;
			}
			private static readonly FieldInfo ResourceHarvestModuleDef_harvestSpeed = AccessTools.Field(
				typeof(ResourceHarvestModule.Def),
				nameof(ResourceHarvestModule.Def.harvestSpeed)
				);

			private static readonly MethodInfo OverrideDefMiningSpeedMethod = AccessTools.Method(
					typeof(AddDrillconeHarvestSpeedBuff),
					nameof(OverrideDefMiningSpeed)
			   );
			public static float OverrideDefMiningSpeed(float originalDefSpeed
				//, ResourceHarvestModule.StatesInstance __instance
				)
			{
				//SgtLogger.l($"original: {originalDefSpeed}, new: {actualMiningSpeed}");
				return actualMiningSpeed;
			}
		}
	}
}

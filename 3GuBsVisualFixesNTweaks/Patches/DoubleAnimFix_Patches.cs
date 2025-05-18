using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ComplexRecipe;
using static STRINGS.MISC.STATUSITEMS;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class DoubleAnimFix_Patches
	{
		public static void CleanAnimTransitions(List<StateMachine.Action> enterActions)
		{

			enterActions.RemoveAll(action => action.name.Contains("PlayAnim("));
			enterActions.RemoveAll(action => action.name.Contains("CheckIfAnimQueueIsEmpty"));
		}

		[HarmonyPatch]
		public static class RemovePoweredActiveStoppableController
		{
			[HarmonyPostfix]
			public static void Postfix(GameObject go)
			{
				StateMachineController component = go.GetComponent<StateMachineController>();
				if (component != null)
				{
					SgtLogger.l("removing PoweredActiveStoppableController from " + go.GetProperName());
					component.cmpdef?.defs?.RemoveAll(item => item.GetStateMachineType() == typeof(PoweredActiveStoppableController));
				}
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(ApothecaryConfig).GetMethod(name);
				yield return typeof(GlassForgeConfig).GetMethod(name);
				yield return typeof(GourmetCookingStationConfig).GetMethod(name);
				yield return typeof(MetalRefineryConfig).GetMethod(name);
				yield return typeof(SuitFabricatorConfig).GetMethod(name);
			}
		}

		[HarmonyPatch(typeof(DesalinatorConfig), nameof(DesalinatorConfig.DoPostConfigureComplete))]
		public class DesalinatorConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();
				SgtLogger.l("removing PoweredActiveController from Desalinator");
				stateMachineController.cmpdef.defs.RemoveAll(def => def.GetStateMachineType() == typeof(PoweredActiveController));
			}
		}

		[HarmonyPatch(typeof(Desalinator.States), nameof(Desalinator.States.InitializeStates))]
		public class Desalinator_States_InitializeStates_Patch
		{
			public static void Postfix(Desalinator.States __instance)
			{
				__instance.on.working
					.Enter(smi => smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.Working))
					.Exit(smi => smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.Working));
			}
		}
		[HarmonyPatch(typeof(WaterPurifierConfig), nameof(WaterPurifierConfig.DoPostConfigureComplete))]
		public class WaterPurifierConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();
				SgtLogger.l("removing PoweredActiveController from WaterPurifier");
				stateMachineController.cmpdef.defs.RemoveAll(def => def.GetStateMachineType() == typeof(PoweredActiveController));
			}
		}
		[HarmonyPatch(typeof(FertilizerMakerConfig), nameof(FertilizerMakerConfig.DoPostConfigureComplete))]
		public class FertilizerMakerConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();
				SgtLogger.l("removing PoweredActiveController from FertilizerMaker");
				stateMachineController.cmpdef.defs.RemoveAll(def => def.GetStateMachineType() == typeof(PoweredActiveController));
			}
		}

		[HarmonyPatch(typeof(LiquidCooledRefinery), nameof(LiquidCooledRefinery.SpawnOrderProduct))]
		public class LiquidCooledRefinery_SpawnOrderProduct_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix(LiquidCooledRefinery __instance, List<GameObject> __result)
			{
				if (!__result.Any() || !__instance.TryGetComponent<MetalRefineryTint>(out var handler))
					return;

				foreach (var obj in __result)
				{
					handler.ProductStorage.Store(obj, true, true);
				}
			}
		}
		[HarmonyPatch(typeof(ComplexFabricatorSM.States), nameof(ComplexFabricatorSM.States.InitializeStates))]
		public class ComplexFabricatorSM_States_InitializeStates_Patch
		{
			public static void Postfix(ComplexFabricatorSM.States __instance)
			{
				CleanAnimTransitions(__instance.operating.working_pst.enterActions);
				//premake some override:
				__instance.operating.working_loop.Enter(smi =>
				{
					var soc = smi.GetComponent<SymbolOverrideController>();
					if (soc == null)
						return;
					var fab = smi.GetComponent<ComplexFabricator>();
					if (fab == null) return;

					var currentRecipe = fab.CurrentWorkingOrder;
					if (currentRecipe == null) return;
					var products = currentRecipe.results;
					if (products == null || !products.Any()) return;

					var targetProduct = products[0].material;
					var prefab = Assets.GetPrefab(targetProduct);
					if (prefab == null)
					{
						SgtLogger.warning("prefab of " + targetProduct.name + " was null");
						return;
					}
					ModAssets.RefreshOutputTracker(soc, prefab);

				});
				__instance.operating.working_pst.transitions?.Clear();
				__instance.operating.working_pst
					.Enter(smi =>
					{
						var complexFabricatorWorkable = smi.GetComponent<ComplexFabricatorWorkable>();
						if (complexFabricatorWorkable == null || !complexFabricatorWorkable.synchronizeAnims || !smi.GetComponent<ComplexFabricator>().duplicantOperated)
						{
							smi.GetComponent<KBatchedAnimController>().Play("working_pst");
						}
						else
							smi.GoTo(__instance.operating.working_pst_complete);
					})
					.EventTransition(GameHashes.AnimQueueComplete, __instance.operating.working_pst_complete);
				__instance.operating.working_pst_complete.transitions?.Clear();
				__instance.operating.working_pst_complete
					.WorkableCompleteTransition((smi) => smi.master.fabricator.Workable, __instance.idle)
						.OnAnimQueueComplete(__instance.idle)
						.ScheduleAction("drop metal refinery products in sync with anim", 2.55f, (smi) => smi.Trigger(ModAssets.OnRefineryAnimPlayed))
						;
			}
		}

	}
}

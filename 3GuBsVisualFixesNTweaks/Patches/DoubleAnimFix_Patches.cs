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

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class DoubleAnimFix_Patches
	{
		static void CleanAnimTransitions(List<StateMachine.Action> enterActions)
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
					SgtLogger.l("removing PoweredActiveStoppableController from "+go.GetProperName());
					component.cmpdef?.defs?.RemoveAll(item => item.GetStateMachineType() == typeof(PoweredActiveStoppableController));
				}
				var workable = go.GetComponent<ComplexFabricatorWorkable>();
				workable.synchronizeAnims = false;
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
		[HarmonyPatch(typeof(WaterPurifierConfig), nameof(WaterPurifierConfig.DoPostConfigureComplete))]
		public class WaterPurifierConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{

				StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();
				
				SgtLogger.l("removing PoweredActiveController from WaterPurifier" );
				stateMachineController.cmpdef.defs.RemoveAll(def => def.GetStateMachineType() == typeof(PoweredActiveController));
			}

		}
	}
}

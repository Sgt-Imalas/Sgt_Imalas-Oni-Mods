using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class TintableContents_Patches
	{
		static Dictionary<SimHashes, Color> CachedColors = new Dictionary<SimHashes, Color>();

		public static Color GetElementColor(SimHashes simhash)
		{
			if (!CachedColors.TryGetValue(simhash, out var color))
			{
				var element = ElementLoader.GetElement(simhash.CreateTag());
				color = element.substance.conduitColour;
				CachedColors[simhash] = color;
			}
			return color;

		}

		[HarmonyPatch]
		public static class AddTintableToBuildings
		{
			[HarmonyPrefix]
			public static void Prefix(GameObject go)
			{
				go.AddOrGet<ContentTintable>();
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(LiquidConditionerConfig).GetMethod(name);
				yield return typeof(LiquidPumpConfig).GetMethod(name);
				yield return typeof(LiquidMiniPumpConfig).GetMethod(name);
			}
		}

		[HarmonyPatch(typeof(LimitValve), nameof(LimitValve.OnMassTransfer))]
		public class LimitValve_ConduitUpdate_Patch
		{
			public static void Prefix(ValveBase __instance, SimHashes element, float transferredMass)
			{
				if (__instance.TryGetComponent<KBatchedAnimController>(out var kbac))
				{

					if (transferredMass <= 0)
					{
						kbac.SetSymbolTint("tint", Color.clear);
						return;
					}


					kbac.SetSymbolTint("tint", GetElementColor(element));

				}
			}
		}

		[HarmonyPatch(typeof(ValveBase), nameof(ValveBase.ConduitUpdate))]
		public class ValveBase_ConduitUpdate_Patch
		{
			public static void Prefix(ValveBase __instance, float dt)
			{
				if (__instance.TryGetComponent<KBatchedAnimController>(out var kbac))
				{
					ConduitFlow flowManager = Conduit.GetFlowManager(__instance.conduitType);
					ConduitFlow.Conduit conduit = flowManager.GetConduit(__instance.inputCell);
					if (!flowManager.HasConduit(__instance.inputCell))
					{
						kbac.SetSymbolTint("tint", Color.clear);
						return;
					}
					ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);

					if (contents.mass > 0f)
					{
						kbac.SetSymbolTint("tint", GetElementColor(contents.element));
					}
					else
						kbac.SetSymbolTint("tint", Color.clear);
				}
			}
		}


		[HarmonyPatch(typeof(SpaceHeater), nameof(SpaceHeater.AddSelfHeat))]
		public class SpaceHeater_AddSelfHeat_Patch
		{
			public static void Postfix(SpaceHeater __instance)
			{
				if (!__instance.heatLiquid)
					return;
				if (!__instance.monitorCells.Any())
					return;
				int inputCell = __instance.monitorCells[0];
				
				if (!__instance.TryGetComponent<KBatchedAnimController>(out var kbac))
					return;

				var element = Grid.Element[inputCell];
				if(!element.IsLiquid)
				{
					kbac.SetSymbolTint("tint",Color.clear);
				}
				else
				{
					kbac.SetSymbolTint("tint", GetElementColor(element.id));
				}
			}
		}
	}

}

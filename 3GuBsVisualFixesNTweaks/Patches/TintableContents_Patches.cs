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

					if (!CachedColors.TryGetValue(element, out var color))
					{
						var elementSubstance = ElementLoader.GetElement(element.CreateTag());
						color = elementSubstance.substance.conduitColour;
						CachedColors[element] = color;
					}

					kbac.SetSymbolTint("tint", color);

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
						if (!CachedColors.TryGetValue(contents.element, out var color))
						{
							var element = ElementLoader.GetElement(contents.element.CreateTag());
							color = element.substance.conduitColour;
							CachedColors[contents.element] = color;
						}

						kbac.SetSymbolTint("tint", color);
					}
					else
						kbac.SetSymbolTint("tint", Color.clear);
				}
			}
		}
	}

}

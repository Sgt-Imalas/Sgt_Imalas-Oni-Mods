using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	class OverlayModes_Patches
	{
		/// <summary>
		/// this patch overrides the tinting color the high pressure conduits receive in their respective conduit overlays
		/// </summary>
		[HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Update))]
		public class OverlayModes_ConduitMode_Update_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			static HashedString ViewMode;
			public static void Prefix(OverlayModes.ConduitMode __instance)
			{
				ViewMode = __instance.ViewMode();
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var Colorset_GetColorByName = AccessTools.Method(typeof(ColorSet), nameof(ColorSet.GetColorByName));
				var ReplaceConduitColor = AccessTools.Method(typeof(OverlayModes_ConduitMode_Update_Patch), nameof(ReplaceHPConduitColor));

				var GetColorByName = AccessTools.Method(typeof(ColorSet), nameof(ColorSet.GetColorByName));
				//var set_TintColour = AccessTools.PropertySetter(typeof(KAnimControllerBase), nameof(KAnimControllerBase.TintColour));

				var codes = orig.ToList();

				// SaveLoadRoot layerTarget;
				int layerTargetIdx = 12;
				foreach (CodeInstruction original in orig)
				{
					if (original.Calls(GetColorByName))
					{
						yield return original; //puts the color on the stack
						yield return new CodeInstruction(OpCodes.Ldloc_S, layerTargetIdx); //current layer target 
						yield return new CodeInstruction(OpCodes.Call, ReplaceConduitColor); //ReplaceHPConduitColor
					}
					else
						yield return original;
				}
			}

			private static Color32 ReplaceHPConduitColor(Color32 oldColor, SaveLoadRoot currentItem)
			{
				if (HighPressureConduitRegistration.IsHighPressureConduit(currentItem.gameObject.GetInstanceID()))
				{
					if (ViewMode == OverlayModes.LiquidConduits.ID)
					{
						return HighPressureConduitRegistration.GetColorForConduitType(ConduitType.Liquid, true);
					}
					else if (ViewMode == OverlayModes.GasConduits.ID)
					{
						return HighPressureConduitRegistration.GetColorForConduitType(ConduitType.Gas, true);
					}
				}
				return oldColor;
			}

		}

		[HarmonyPatch(typeof(OverlayModes.SolidConveyor), nameof(OverlayModes.SolidConveyor.Update))]
		public class OverlayModes_SolidConveyor_Update_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HPA_Rails_Mod_Enabled || Config.Instance.DupesLogistics_Enabled;
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var SolidConveyor_tint_color = AccessTools.Field(typeof(OverlayModes.SolidConveyor), nameof(OverlayModes.SolidConveyor.tint_color));
				var ReplaceConduitColor = AccessTools.Method(typeof(OverlayModes_SolidConveyor_Update_Patch), nameof(ReplaceSolidConduitColor));
				var codes = orig.ToList();

				// SaveLoadRoot layerTarget;
				int layerTargetIdx = 11;
				foreach (CodeInstruction original in orig)
				{
					if (original.LoadsField(SolidConveyor_tint_color))
					{
						yield return original; //puts the color on the stack
						yield return new CodeInstruction(OpCodes.Ldloc_S, layerTargetIdx); //current layer target 
						yield return new CodeInstruction(OpCodes.Call, ReplaceConduitColor); //ReplaceHPConduitColor
					}
					else
						yield return original;
				}
			}

			private static Color32 ReplaceSolidConduitColor(Color32 oldColor, SaveLoadRoot currentItem)
			{
				if (LogisticConduit.IsLogisticConduit(currentItem.gameObject))
				{
					return HighPressureConduitRegistration.GetColorForConduitType(ConduitType.Solid, true, true);
				}
				else if (HighPressureConduitRegistration.IsHighPressureConduit(currentItem.gameObject.GetInstanceID()))
				{
					return HighPressureConduitRegistration.GetColorForConduitType(ConduitType.Solid, true);
				}
				return oldColor;
			}
		}
	}
}

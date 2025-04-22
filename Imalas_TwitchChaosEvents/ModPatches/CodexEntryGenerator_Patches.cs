using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static CodexEntryGenerator_Elements;
using static CodexTemperatureTransitionPanel;

namespace Imalas_TwitchChaosEvents.ModPatches
{
	class CodexEntryGenerator_Patches
	{


		[HarmonyPatch(typeof(CodexTemperatureTransitionPanel), MethodType.Constructor, [typeof(Element), typeof(CodexTemperatureTransitionPanel.TransitionType)])]
		public class CodexTemperatureTransitionPanel_Constructor_Patch
		{
			public static void Prefix(ref Element source, CodexTemperatureTransitionPanel.TransitionType type)
			{
				if (source.id == ModElements.InverseWaterFlakingCrashPrevention.SimHash)
				{
					source = ElementLoader.FindElementByHash(ModElements.InverseIce.SimHash);
				}
			}
		}

		[HarmonyPatch(typeof(CodexTemperatureTransitionPanel), nameof(CodexTemperatureTransitionPanel.ConfigureResults))]
		public class CodexTemperatureTransitionPanel_ConfigureResults_Patch
		{
			public static bool Prefix(CodexTemperatureTransitionPanel __instance, GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
			{
				if (__instance.sourceElement.id != ModElements.InverseIce.SimHash)
				{
					return true;
				}
				__instance.sourceElement.lowTempTransition = ElementLoader.FindElementByHash(ModElements.InverseWater.SimHash);
				if (__instance.transitionType == TransitionType.COOL && __instance.sourceElement.lowTempTransition == null)
				{
					var inverseWater = ElementLoader.FindElementByHash(ModElements.InverseWater.SimHash);

					HierarchyReferences component = Util.KInstantiateUI(__instance.materialPrefab, __instance.resultsContainer, force_active: true).GetComponent<HierarchyReferences>();
					Tuple<Sprite, Color> uISprite = Def.GetUISprite(inverseWater);
					component.GetReference<Image>("Icon").sprite = uISprite.first;
					component.GetReference<Image>("Icon").color = uISprite.second;
					string text = $"{GameUtil.GetFormattedMass(1f)}";

					component.GetReference<LocText>("Title").text = text;
					component.GetReference<LocText>("Title").color = Color.black;
					component.GetReference<ToolTip>("ToolTip").toolTip = inverseWater.name;
					component.GetReference<KButton>("Button").onClick += delegate
					{
						ManagementMenu.Instance.codexScreen.ChangeArticle(global::STRINGS.UI.ExtractLinkID(inverseWater.tag.ProperName()));
					};
					return false;
				}
				return true;
			}


			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();
				var hightTemptTransitionField = AccessTools.Field(typeof(Element), nameof(Element.highTempTransition));
				var lowTemptTransitionField = AccessTools.Field(typeof(Element), nameof(Element.lowTempTransition));

				// find injection point
				var index = codes.FindIndex(ci => ci.LoadsField(hightTemptTransitionField));
				var index2 = codes.FindIndex(ci => ci.LoadsField(lowTemptTransitionField));

				if (index == -1)
				{
					SgtLogger.error("TRANSPILER ERROR: CodexTemperatureTransitionPanel_ConfigureResults_Patch: Cannot find highTempTransition field");
					return codes;
				}
				if (index2 == -1)
				{
					SgtLogger.error("TRANSPILER ERROR: CodexTemperatureTransitionPanel_ConfigureResults_Patch: Cannot find lowTempTransition field");
					return codes;
				}

				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(CodexTemperatureTransitionPanel_ConfigureResults_Patch), "InjectedMethod");

				// inject right after the found index
				codes.InsertRange(index + 1, new[]
				{
					new CodeInstruction(OpCodes.Call, m_InjectedMethod)
				});
				codes.InsertRange(index2 + 1, new[]
			   {
					new CodeInstruction(OpCodes.Call, m_InjectedMethod)
				});
				return codes;
			}

			private static Element InjectedMethod(Element highTempTransitionTarget)
			{
				if (highTempTransitionTarget.id == ModElements.InverseWaterFlakingCrashPrevention.SimHash)
				{
					return ElementLoader.FindElementByHash(ModElements.InverseIce.SimHash);
				}
				else
				{
					return highTempTransitionTarget;
				}
			}
		}
	}
}

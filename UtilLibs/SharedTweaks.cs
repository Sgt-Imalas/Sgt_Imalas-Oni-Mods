using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ElementConverter;
using static STRINGS.BUILDINGS.PREFABS;

namespace UtilLibs
{
	public static class SharedTweaks
	{
		/// <summary>
		/// Stagger research entries instead of having them in a single line
		/// </summary>
		public static class ResearchNotificationMessageFix
		{
			static readonly string RegistryKey = "RegistryKey_ResearchNotificationMessageFix";
			public static void ExecutePatch(Harmony harmony)
			{
				if (PRegistry.GetData<bool>(RegistryKey))
					return;
				try
				{
					var targetMethod = AccessTools.Method(typeof(ResearchCompleteMessage), nameof(ResearchCompleteMessage.GetMessageBody));
					var transpiler = AccessTools.Method(typeof(ResearchNotificationMessageFix), nameof(LinebreakTranspiler));
					harmony.Patch(targetMethod, transpiler: new(transpiler));
					PRegistry.PutData(RegistryKey, true);
				}
				catch { }
			}
			private static IEnumerable<CodeInstruction> LinebreakTranspiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				// find injection point
				var index = codes.FindIndex(ci => ci.LoadsConstant(", "));

				if (index == -1)
				{
					Console.WriteLine("TRANSPILER FAILED: ResearchCompleteMessage");
					return codes;
				}
				codes[index].operand = "\n  • ";
				return codes;
			}
		}

		/// <summary>
		/// if >8 entries, collapse them into a "+X more" item when not hovered
		/// </summary>
		public static class ResearchScreenCollapseEntries
		{
			static Dictionary<ResearchEntry, List<GameObject>> CollectedIcons = new();
			static Dictionary<ResearchEntry, GameObject> CollapsedIndicators = new();

			static readonly string RegistryKey = "RegistryKey_ResearchScreenCollapseEntries";
			public static void ExecutePatch(Harmony harmony)
			{
				if (PRegistry.GetData<bool>(RegistryKey))
					return;
				try
				{
					var targetMethodOn = AccessTools.Method(typeof(ResearchEntry), nameof(ResearchEntry.OnHover));
					var targetMethodOff = AccessTools.Method(typeof(ResearchEntry), nameof(ResearchEntry.SetEverythingOff));
					var targetMethodEntryCollection = AccessTools.Method(typeof(ResearchEntry), nameof(ResearchEntry.GetFreeIcon));
					var targetMethodSetTech = AccessTools.Method(typeof(ResearchEntry), nameof(ResearchEntry.SetTech));

					var turnOffPostfix = AccessTools.Method(typeof(ResearchScreenCollapseEntries), nameof(TurnOffPostfix));
					var turnOnPrefix = AccessTools.Method(typeof(ResearchScreenCollapseEntries), nameof(TurnOnPrefix));
					var collectEntriesPostfix = AccessTools.Method(typeof(ResearchScreenCollapseEntries), nameof(CollectEntriesPostfix));

					///show all entries on select (onhover)
					harmony.Patch(targetMethodOn, prefix: new(turnOnPrefix, Priority.LowerThanNormal));

					///hide all entries >8 on deselect
					harmony.Patch(targetMethodOff, postfix: new(turnOffPostfix, Priority.LowerThanNormal));

					///cache icons to hide later
					harmony.Patch(targetMethodEntryCollection, postfix: new(collectEntriesPostfix));

					///start off collapsed
					harmony.Patch(targetMethodSetTech, postfix: new(turnOffPostfix));

					PRegistry.PutData(RegistryKey, true);
				}
				catch { }
			}

			public static void CollectEntriesPostfix(ResearchEntry __instance, ref GameObject __result)
			{
				if (__instance.targetTech?.unlockedItems.Count() <= 8)
					return;
				if (!CollectedIcons.ContainsKey(__instance))
					CollectedIcons[__instance] = new(8);
				CollectedIcons[__instance].Add(__result);
			}
			public static void TurnOffPostfix(ResearchEntry __instance) => CollapseExcessEntries(__instance, true);
			public static void TurnOnPrefix(ResearchEntry __instance, bool entered, Tech hoverSource)
			{
				if (!entered) return;

				if (hoverSource != __instance.targetTech)
					return;

				CollapseExcessEntries(__instance, false);
			}
			static void CollapseExcessEntries(ResearchEntry entry, bool setCollapsed)
			{
				///do not adress techs with 8 or less items
				if (entry.targetTech?.unlockedItems.Count() <= 8)
					return;

				if (!CollapsedIndicators.TryGetValue(entry, out var collapsedIcon))
					collapsedIcon = CreateCollapseIcon(entry);

				collapsedIcon.SetActive(setCollapsed);

				if (!CollectedIcons.TryGetValue(entry, out var icons))
					return;

				for (int i = icons.Count - 1; i >= 7; i--)
				{
					icons[i].SetActive(!setCollapsed);
				}

			}

			/// <summary>
			/// runs once per tech item
			/// </summary>
			/// <param name="entry"></param>
			/// <returns></returns>
			private static GameObject CreateCollapseIcon(ResearchEntry entry)
			{
				//set that once to avoid endless wide tooltips
				entry.researchName.GetComponent<ToolTip>().SizingSetting = ToolTip.ToolTipSizeSetting.MaxWidthWrapContent;

				//avoid GetFreeIcon method to not collect this
				GameObject freeIcon = Util.KInstantiateUI(entry.iconPrefab, entry.iconPanel);
				freeIcon.SetActive(true);
				var hr = freeIcon.GetComponent<HierarchyReferences>();

				var bgImage = hr.GetReference<KImage>("Background");

				hr.GetReference<KImage>("Icon").gameObject.SetActive(false);
				bgImage.color = PUITuning.Colors.ButtonPinkStyle.inactiveColor;

				hr.GetReference<KImage>("DLCOverlay").gameObject.SetActive(false);

				var infoText = Util.KInstantiateUI<LocText>(entry.researchName.gameObject, freeIcon, true);
				int extraItems = entry.targetTech?.unlockedItems.Count - 7 ?? -1;
				infoText.SetText("+" + extraItems);
				infoText.enableAutoSizing = false;
				infoText.fontSize = 32;
				infoText.alignment = TextAlignmentOptions.Center;

				CollapsedIndicators[entry] = freeIcon;
				return freeIcon;

			}
		}
		/// <summary>
		/// Improve elementconverter description strings
		/// </summary>
		public static class ElementConverterDescriptionImprovement
		{
			static readonly string RegistryKey = "RegistryKey_ElementConverterDescriptionImprovement";
			public static void ExecutePatch(Harmony harmony)
			{
				if (PRegistry.GetData<bool>(RegistryKey))
					return;
				try
				{

					var targetMethod = AccessTools.Method(typeof(ElementConverter), nameof(ElementConverter.GetDescriptors));
					var replaceElementConverterPrefix = AccessTools.Method(typeof(ElementConverterDescriptionImprovement), nameof(ReplaceElementConverterDescriptorsPrefix));
					harmony.Patch(targetMethod, prefix: new(replaceElementConverterPrefix));
					PRegistry.PutData(RegistryKey, true);
				}
				catch { }
			}
			public static bool ReplaceElementConverterDescriptorsPrefix(ElementConverter __instance, GameObject go, ref List<Descriptor> __result)
			{
				int visibleConverterCount = 0;
				var converters = go.GetComponents<ElementConverter>();
				foreach (var converter in converters)
				{
					if (converter.showDescriptors && converter.consumedElements != null)
						++visibleConverterCount;
				}

				//only go into effect when there are multiple elementconverters on the GO
				if (!__instance.showDescriptors || visibleConverterCount < 2 || __instance.consumedElements == null || !__instance.consumedElements.Any())
				{
					return true;
				}
				string consumptionString = string.Empty;
				string productionString = string.Empty;
				string tooltip = string.Empty;

				string elementFormatted = "{0} ({1})";

				if (__instance.consumedElements != null)
				{
					foreach (var consumedElement in __instance.consumedElements)
					{
						if (!consumptionString.IsNullOrWhiteSpace())
							consumptionString += ", ";
						if (!tooltip.IsNullOrWhiteSpace())
							tooltip += "\n";

						consumptionString += string.Format(elementFormatted, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"));
						tooltip += string.Format(STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"));

					}
					tooltip += "\n\n";
				}
				if (__instance.outputElements != null)
				{
					foreach (var outputElement in __instance.outputElements)
					{
						if (!productionString.IsNullOrWhiteSpace())
							productionString += ", ";

						if (outputElement.IsActive)
						{
							LocString productionEntryTooltip = STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_INPUTTEMP;
							if (outputElement.useEntityTemperature)
							{
								productionEntryTooltip = STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP;
							}
							else if (outputElement.minOutputTemperature > 0f)
							{
								productionEntryTooltip = STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINTEMP;
							}
							productionString += string.Format(elementFormatted, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature));
							tooltip += string.Format(productionEntryTooltip.Replace("\n\n", "\n"), outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature));

							tooltip += "\n";
						}
					}
				}

				//turn x to y
				string text = string.Format(CRAFTINGTABLE.RECIPE_DESCRIPTION, consumptionString, productionString);
				Descriptor item = default(Descriptor);
				item.SetupDescriptor(text, tooltip, Descriptor.DescriptorType.Effect);
				__result = [item];

				return false;
			}
		}
	}
}

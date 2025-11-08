using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using ProcGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using static ElementConverter;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.UI;

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
			static readonly string RegistryKeyVersion = RegistryKey + "_Version";
			static readonly int Version = 1;

			public static void ExecutePatch()
			{
				var harmony = new Harmony(RegistryKey);

				if (PRegistry.GetData<bool>(RegistryKey))
				{
					if (PRegistry.GetData<int>(RegistryKeyVersion) < Version)
						harmony.UnpatchAll(RegistryKey); //remove old patches and then apply newer ones
					else
						return;//if latest/newer version is already active; skip
				}
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
			static readonly string RegistryKeyVersion = RegistryKey + "_Version";
			static readonly int Version = 1;

			public static void ExecutePatch()
			{
				var harmony = new Harmony(RegistryKey);

				if (PRegistry.GetData<bool>(RegistryKey))
				{
					if (PRegistry.GetData<int>(RegistryKeyVersion) < Version)
						harmony.UnpatchAll(RegistryKey); //remove old patches and then apply newer ones
					else
						return;//if latest/newer version is already active; skip
				}
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
			public static void TurnOffPostfix(ResearchEntry __instance)
			{
				CollapseExcessEntries(__instance, true);
			}
			public static void TurnOnPrefix(ResearchEntry __instance, bool entered, Tech hoverSource)
			{
				if (!entered) return;

				if (hoverSource != __instance.targetTech)
					return;

				CollapseExcessEntries(__instance, false);
			}
			static void HandleFastTrack(ResearchEntry entry, bool enableOldLayout)
			{

				if (entry.TryGetComponent<LayoutElement>(out var frozenLayout)
					&& entry.TryGetComponent<LayoutGroup>(out var realLayout)
					&& entry.transform.parent.TryGetComponent<KChildFitter>(out var cf))
				{
					if (frozenLayout.enabled == realLayout.enabled)
					{
						return;
					}

					if (enableOldLayout)
					{
						SetFreezeState(frozenLayout, realLayout, false);
						cf.FitSize();
					}
					else
					{
						SetFreezeState(frozenLayout, realLayout, true);
						cf.FitSize();
					}
				}
			}
			static void SetFreezeState(LayoutElement frozenLayout, LayoutGroup realLayout, bool freeze)
			{
				///forced rebuild is required or the entry distorts
				LayoutRebuilder.ForceRebuildLayoutImmediate(realLayout.rectTransform());
				if (freeze)
				{
					realLayout.enabled = false;
					frozenLayout.enabled = true;
				}
				else
				{
					frozenLayout.enabled = false;
					realLayout.enabled = true;
				}
			}

			static void CollapseExcessEntries(ResearchEntry entry, bool collapseEntries)
			{
				///do not adress techs with 8 or less items
				if (entry.targetTech?.unlockedItems.Count() <= 8)
					return;

				if (!CollapsedIndicators.TryGetValue(entry, out var collapsedIcon))
					collapsedIcon = CreateCollapseIcon(entry);

				if (!CollectedIcons.TryGetValue(entry, out var icons))
					return;


				collapsedIcon.SetActive(collapseEntries);

				for (int i = icons.Count - 1; i >= 7; i--)
				{
					icons[i].SetActive(!collapseEntries);
				}
				HandleFastTrack(entry, !collapseEntries);
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
				infoText.enableWordWrapping = false;
				infoText.overflowMode = TextOverflowModes.Overflow;

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
			static readonly string RegistryKeyVersion = RegistryKey + "_Version";
			static readonly int Version = 1;

			public static void ExecutePatch()
			{
				var harmony = new Harmony(RegistryKey);

				if (PRegistry.GetData<bool>(RegistryKey))
				{
					if (PRegistry.GetData<int>(RegistryKeyVersion) < Version)
						harmony.UnpatchAll(RegistryKey); //remove old patches and then apply newer ones
					else
						return;//if latest/newer version is already active; skip
				}
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


		/// <summary>
		/// space the connection lines horizontally to avoid them overlapping
		/// </summary>
		public static class ResearchScreenBetterConnectionLines
		{

			static readonly string RegistryKey = "RegistryKey_ResearchScreenBetterConnectionLines";
			static readonly string RegistryKeyVersion = RegistryKey + "_Version";
			static readonly int Version = 1;

			public static void ExecutePatch()
			{
				var harmony = new Harmony(RegistryKey);

				if (PRegistry.GetData<bool>(RegistryKey))
				{
					if (PRegistry.GetData<int>(RegistryKeyVersion) < Version)
						harmony.UnpatchAll(RegistryKey); //remove old patches and then apply newer ones
					else
						return;//if latest/newer version is already active; skip
				}
				try
				{
					var targetMethodOnSpawn = AccessTools.Method(typeof(ResearchEntry), nameof(ResearchEntry.OnSpawn));

					var onSpawnPostfix = AccessTools.Method(typeof(ResearchScreenBetterConnectionLines), nameof(OnSpawnPostfix));

					///change the line renderers to be more spaced out
					harmony.Patch(targetMethodOnSpawn, postfix: new(onSpawnPostfix, Priority.HigherThanNormal));
					SgtLogger.l(RegistryKey + " successfully patched");
					PRegistry.PutData(RegistryKey, true);
				}
				catch { }
			}

			/// <summary>
			/// horizontal distance between 2 directly horizontally adjacent tech nodes
			/// </summary>
			const float X_Step = 350;
			/// <summary>
			/// vertical distance between 2 directly vertically adjacent tech nodes
			/// </summary>
			const float Y_Step = 250;

			static Dictionary<Tech, int> techConnectionPoints = null;

			public static void OnSpawnPostfix(ResearchEntry __instance)
			{
				if (techConnectionPoints == null)
				{
					techConnectionPoints = new Dictionary<Tech, int>();
					foreach (var tech in Db.Get().Techs.resources)
					{
						foreach (var requiredTech in tech.requiredTech)
						{
							if (!techConnectionPoints.ContainsKey(requiredTech))
							{
								techConnectionPoints.Add(requiredTech, 0);
							}
							techConnectionPoints[requiredTech]++;
						}
					}
				}


				foreach (var lineRenderer in __instance.techLineMap)
				{
					UnityEngine.Object.Destroy(lineRenderer.Value.gameObject);
				}
				__instance.techLineMap.Clear();

				List<Tech> sourceBelow = [], sourceAbove = [], sourceEven = [];

				var currentTech = __instance.targetTech;

				foreach (var sourceTech in currentTech.requiredTech)
				{
					float verticalOffset = (sourceTech.center.y - currentTech.center.y);
					//SgtLogger.l(" ->> verticalOffset: " + verticalOffset + " for " + currentTech.Id + " to " + sourceTech.Id);
					if (verticalOffset < -1)
						sourceBelow.Add(sourceTech);
					else if (verticalOffset > 1)
						sourceAbove.Add(sourceTech);
					else
						sourceEven.Add(sourceTech);
				}
				sourceAbove.Sort((a, b) => a.center.y.CompareTo(b.center.y));
				sourceBelow.Sort((a, b) => -a.center.y.CompareTo(b.center.y));
				foreach (var evenTech in sourceEven)
				{
					CreateTechConnection(__instance, currentTech, evenTech);
				}
				float connectionOffset = sourceEven.Any() ? 1 : sourceBelow.Any() && sourceAbove.Any() ? 0.625f : 0;
				for (int i = 0; i < sourceBelow.Count; i++)
				{
					CreateTechConnection(__instance, currentTech, sourceBelow[i], -connectionOffset - i);
				}
				for (int i = 0; i < sourceAbove.Count; i++)
				{
					CreateTechConnection(__instance, currentTech, sourceAbove[i], connectionOffset + i);
				}

				__instance.QueueStateChanged(isSelected: false);
				if (__instance.targetTech == null)
				{
					return;
				}

				foreach (TechInstance item2 in Research.Instance.GetResearchQueue())
				{
					if (item2.tech == __instance.targetTech)
					{
						__instance.QueueStateChanged(isSelected: true);
					}
				}
			}

			static void CreateTechConnection(ResearchEntry __instance, Tech currentTech, Tech requisite, float connectionPointNr = 0)
			{
				float techBorderX = currentTech.width / 2f + 25f;
				float requisiteTechRightBorderX = (currentTech.center.x - techBorderX - (requisite.center.x + techBorderX)) +2;


				Vector2 relativeStartPoint = Vector2.zero;
				Vector2 relativeEndPoint = Vector2.zero;

				float verticalOffset = (requisite.center.y - currentTech.center.y);
				float totalConnections = Mathf.Max(0,techConnectionPoints[requisite]);

				float YClamp = Mathf.Min(totalConnections, 3);

				float verticalStepDiff = Mathf.Clamp(verticalOffset / Y_Step, -YClamp, YClamp);
				verticalStepDiff = (float)Math.Round(verticalStepDiff * 4, MidpointRounding.ToEven) / 4f; //round to 0.25 steps, so we can use the same offset for all techs
				
				//if (Mathf.Abs(verticalStepDiff) == 0.5f)
				//	verticalStepDiff *= 1.25f;

				if (Mathf.Abs(verticalStepDiff) == 1.25f)
					verticalStepDiff /= 1.25f;


				float stepOffset = 12f;

				float maxHeightOffsetTarget = currentTech.height / 2f;
				//default in game is 20px offset for at least one above/below)
				float relativeYDiffTarget = Mathf.Clamp(verticalStepDiff * stepOffset, -maxHeightOffsetTarget, maxHeightOffsetTarget);
				float relativeYDiffTargetConnectionPoint = Mathf.Clamp(connectionPointNr * stepOffset, -maxHeightOffsetTarget, maxHeightOffsetTarget);

				float maxHeightOffsetSource = (requisite.height / 2f);
				//default in game is 20px offset for at least one above/below)
				float relativeYDiffSource = Mathf.Clamp(-verticalStepDiff * stepOffset, -maxHeightOffsetSource, maxHeightOffsetSource);

				relativeEndPoint = new(0, relativeYDiffTargetConnectionPoint);
				relativeStartPoint = new(0, relativeYDiffSource);

				float midPointConnectionCalc = Mathf.CeilToInt(Mathf.Max(0, totalConnections-3)/ 2f);

				float midpoint = 32 - stepOffset - (midPointConnectionCalc * stepOffset);
				if (midpoint < 0)
					midpoint = 0;

				//SgtLogger.l(" - verticalStepDiff: " + verticalStepDiff + " for " + currentTech.Id + " to " + requisite.Id + "; Midpoint: "+midpoint+"Total Cons: "+totalConnections+", relativeYDiffTarget " + relativeYDiffTarget + " , halfTechHeightTarget: " + maxHeightOffsetTarget + ", relativeYDiffSource: " + relativeYDiffSource);

				float horizontalOffsetTarget = midpoint + Mathf.Abs(relativeYDiffTarget);
				float horizontalOffsetSource = midpoint + Mathf.Abs(relativeYDiffSource);

				UILineRenderer component = Util.KInstantiateUI(__instance.linePrefab, __instance.lineContainer.gameObject, true).GetComponent<UILineRenderer>();
				component.Points = [
					new Vector2(0, 0) + relativeEndPoint,
						new Vector2(-horizontalOffsetTarget, 0) + relativeEndPoint,
						new Vector2(-horizontalOffsetSource, verticalOffset) + relativeStartPoint,
						new Vector2(-requisiteTechRightBorderX, verticalOffset) + relativeStartPoint,
						];

				//foreach (var point in component.Points)
				//{
				//	SgtLogger.l(currentTech.Id + "->" + requisite.Id + " - Point: " + point);
				//}

				component.LineThickness = __instance.lineThickness_inactive;
				component.color = __instance.inactiveLineColor;
				__instance.techLineMap.Add(requisite, component);
			}
		}
		/// <summary>
		/// Dynamically adjust the height of the MaterialSelector header based on the text length
		/// </summary>
		public static class DynamicMaterialSelectorHeaderHeight
		{
			static readonly string RegistryKey = "RegistryKey_DynamicMaterialSelectorHeaderHeight";
			static readonly string RegistryKeyVersion = RegistryKey + "_Version";
			static readonly int Version = 3;

			public static void ExecutePatch()
			{
				var harmony = new Harmony(RegistryKey);
				
				if (PRegistry.GetData<bool>(RegistryKey))
				{
					if (PRegistry.GetData<int>(RegistryKeyVersion) < Version)
						harmony.UnpatchAll(RegistryKey); //remove old patches and then apply newer ones
					else
						return;//if latest/newer version is already active; skip
				}
				try
				{
					var targetMethod = AccessTools.Method(typeof(MaterialSelector), nameof(MaterialSelector.UpdateHeader));
					var postfix = AccessTools.Method(typeof(DynamicMaterialSelectorHeaderHeight), nameof(DynamicHeightPostfix));
					harmony.Patch(targetMethod, postfix: new(postfix));
					PRegistry.PutData(RegistryKey, true);
				}
				catch { }
			}
			public static void DynamicHeightPostfix(MaterialSelector __instance)
			{
				LocText headerText = __instance.Headerbar.GetComponentInChildren<LocText>();
				int linecount = headerText.textInfo.lineCount;
				int height = linecount * 24;
				__instance.Headerbar.GetComponent<LayoutElement>().minHeight = height;
			}
		}
	}
}

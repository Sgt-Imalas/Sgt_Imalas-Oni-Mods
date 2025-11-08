using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// if >8 entries, collapse them into a "+X more" item when not hovered
	/// </summary>
	public sealed class ResearchScreenCollapseEntries : PForwardedComponent
	{
		static Dictionary<ResearchEntry, List<GameObject>> CollectedIcons = new();
		static Dictionary<ResearchEntry, GameObject> CollapsedIndicators = new();
		public static void Register()
		{
			new ResearchScreenCollapseEntries().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
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
				plibInstance.Patch(targetMethodOn, prefix: new(turnOnPrefix, Priority.LowerThanNormal));

				///hide all entries >8 on deselect
				plibInstance.Patch(targetMethodOff, postfix: new(turnOffPostfix, Priority.LowerThanNormal));

				///cache icons to hide later
				plibInstance.Patch(targetMethodEntryCollection, postfix: new(collectEntriesPostfix));

				///start off collapsed
				plibInstance.Patch(targetMethodSetTech, postfix: new(turnOffPostfix));

				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
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
}

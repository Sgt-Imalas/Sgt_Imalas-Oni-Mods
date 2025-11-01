using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DontBlameMods.cs
{
	internal class Patches
	{
		/// <summary>
		/// Since the Mod patches the load method, it will recieve blame on ANY load crash.
		/// This Patch keeps it enabled on a crash, so you dont need to reenable it for syncing,
		/// </summary>
		[HarmonyPatch(typeof(KMod.Mod))]
		[HarmonyPatch(nameof(KMod.Mod.SetCrashed))]
		public static class DontDisableModOnCrash
		{
			public static bool Prefix(KMod.Mod __instance)
			{
				__instance.SetCrashCount(__instance.crash_count + 1);
				return false;
			}
		}
		//[HarmonyPatch(typeof(Manager))]
		//[HarmonyPatch(nameof(Manager.EnableMod))]
		//public static class DontDisableModOnCrash2
		//{
		//    public static bool Prefix(Manager __instance, Label id, bool enabled, object caller)
		//    {
		//        SgtLogger.l($"Prevented disabling the mod {id.title} as crashing mod.");
		//        if(caller.GetType() == typeof(ReportErrorDialog) && enabled==false)
		//        {

		//            return false;
		//        }
		//        return true;
		//    }
		//}

		[HarmonyPatch(typeof(ReportErrorDialog))]
		[HarmonyPatch(nameof(ReportErrorDialog.BuildModsList))]
		public static class DontDisableModOnCrash3
		{
			public static bool Prefix(ReportErrorDialog __instance)
			{

				DebugUtil.Assert((UnityEngine.Object)Global.Instance != (UnityEngine.Object)null && Global.Instance.modManager != null);
				Manager mod_mgr = Global.Instance.modManager;
				List<KMod.Mod> allCrashableMods = mod_mgr.GetAllCrashableMods();
				allCrashableMods.Sort((Comparison<KMod.Mod>)((x, y) => y.foundInStackTrace.CompareTo(x.foundInStackTrace)));
				foreach (KMod.Mod mod in allCrashableMods)
				{
					HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(__instance.modEntryPrefab, __instance.modEntryParent.gameObject);
					LocText reference = hierarchyReferences.GetReference<LocText>("Title");
					reference.text = mod.title;
					reference.color = mod.foundInStackTrace ? Color.red : Color.white;
					MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
					toggle.ChangeState(mod.IsEnabledForActiveDlc() ? 1 : 0);
					KMod.Label mod_label = mod.label;
					toggle.onClick += (System.Action)(() =>
					{
						bool enabled = !mod_mgr.IsModEnabled(mod_label);
						toggle.ChangeState(enabled ? 1 : 0);
						mod_mgr.EnableMod(mod_label, enabled, __instance);
					});
					toggle.GetComponent<ToolTip>().OnToolTip = (Func<string>)(() => (string)(mod_mgr.IsModEnabled(mod_label) ? STRINGS.UI.FRONTEND.MODS.TOOLTIPS.ENABLED : STRINGS.UI.FRONTEND.MODS.TOOLTIPS.DISABLED));
					hierarchyReferences.gameObject.SetActive(true);
				}
				return false;
			}
		}

	}
}

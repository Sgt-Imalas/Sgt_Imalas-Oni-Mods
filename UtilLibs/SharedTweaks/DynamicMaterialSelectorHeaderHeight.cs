using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// Dynamically adjust the height of the MaterialSelector header based on the text length
	/// </summary>
	public sealed class DynamicMaterialSelectorHeaderHeight : PForwardedComponent
	{
		public static void Register()
		{
			new DynamicMaterialSelectorHeaderHeight().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod = AccessTools.Method(typeof(MaterialSelector), nameof(MaterialSelector.UpdateHeader));
				var postfix = AccessTools.Method(typeof(DynamicMaterialSelectorHeaderHeight), nameof(DynamicHeightPostfix));
				plibInstance.Patch(targetMethod, postfix: new(postfix));
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
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

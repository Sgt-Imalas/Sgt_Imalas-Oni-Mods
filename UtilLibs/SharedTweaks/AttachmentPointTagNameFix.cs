using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UtilLibs.SharedTweaks
{
	public class AttachmentPointTagNameFix : PForwardedComponent
	{
		public static void Register()
		{
			new AttachmentPointTagNameFix().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod =  AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.IsAreaClear), [typeof(GameObject), typeof(int), typeof(Orientation), typeof(ObjectLayer), typeof(ObjectLayer), typeof(bool), typeof(bool), typeof(string).MakeByRefType(), typeof(bool)]);
				var targetMethod2 = AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.IsValidBuildLocation), [typeof(GameObject), typeof(int), typeof(Orientation), typeof(bool),typeof(string).MakeByRefType()]);
				var postfix = AccessTools.Method(typeof(AttachmentPointTagNameFix), nameof(AttachmentPointTagNameFix.Postfix));
				plibInstance.Patch(targetMethod, postfix: new(postfix));
				plibInstance.Patch(targetMethod2, postfix: new(postfix));
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		/// <summary>
		/// fixes attachment buildings using the tag.tostring instead of the actual tag string
		/// only takes effect if the tag string exists
		/// </summary>
		public static void Postfix(BuildingDef __instance, ref string fail_reason, ref bool __result)
		{
			if (__result || __instance.BuildLocationRule != BuildLocationRule.BuildingAttachPoint)
				return;

			string properAttachmentPointTagString = Strings.Get("STRINGS.MISC.TAGS." + __instance.AttachmentSlotTag.Name.ToUpperInvariant());

			if (!properAttachmentPointTagString.Contains("MISSING.STRINGS"))
			{
				fail_reason = string.Format(global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, properAttachmentPointTagString);
				//SgtLogger.l("fail reason: "+fail_reason);
			}
		}
	}
}

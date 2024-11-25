using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace AkisSnowThings.Patches
{
	internal class BuildingDefPatch
    {
        [HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.IsAreaClear),
            [typeof(GameObject),typeof(int), typeof(Orientation), typeof(ObjectLayer), typeof(ObjectLayer), typeof(bool), typeof(bool), typeof(string)],
            [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out])]
        public class BuildingDef_IsAreaClear_Patch
		{
            public static void Postfix(BuildingDef __instance, ref string fail_reason, ref bool __result)
            {
                if (!__result && __instance.BuildLocationRule == BuildLocationRule.BuildingAttachPoint)
				{                    
					fail_reason = string.Format(global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, Strings.Get("STRINGS.MISC.TAGS."+__instance.AttachmentSlotTag.Name.ToUpperInvariant()));
                    SgtLogger.l("fail reason: "+fail_reason);
				}
            }

		}
	}
}

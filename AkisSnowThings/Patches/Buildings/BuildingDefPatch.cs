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
        /// <summary>
        /// fixes attachment buildings using the tag.tostring instead of the actual tag string
        /// only takes effect if the tag string exists
        /// </summary>
        [HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.IsAreaClear),
            [typeof(GameObject),typeof(int), typeof(Orientation), typeof(ObjectLayer), typeof(ObjectLayer), typeof(bool), typeof(bool), typeof(string), typeof(bool)],
            [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal])]
        public class BuildingDef_IsAreaClear_Patch
		{
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
}

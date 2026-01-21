using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.WORLD_TRAITS;
using static UIMinionOrMannequin;
using static UnityEngine.GraphicsBuffer;

namespace BlueprintsV2.BlueprintsV2.Patches
{
	internal class FilterSideScreen_Patches
	{
        [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.IsValidForTarget))]
        public class FilterSideScreen_IsValidForTarget_Patch
        {
            public static void Postfix(FilterSideScreen __instance, GameObject target, ref bool __result)
            {
                if (__result || __instance.isLogicFilter)
                    return;

                if(target == null)
                    return;

                if(target.TryGetComponent<ElementNote>(out _) && target.TryGetComponent<Filterable>(out _))
                    __result = true;
            }
        }

        [HarmonyPatch(typeof(Filterable), nameof(Filterable.GetTagOptions))]
        public class Filterable_GetTagOptions_Patch
        {
            public static bool Prefix(Filterable __instance, ref Dictionary<Tag, HashSet<Tag>> __result)
            {
                if(__instance is not ElementOnlyFilterable)
                    return true;
                __result = ElementOnlyFilterable.GetElementFilters();
                return false;
			}
        }

        [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.SetTarget))]
        public class FilterSideScreen_SetTarget_Patch
        {
            public static void Postfix(FilterSideScreen __instance, GameObject target)
			{
				if (__instance.isLogicFilter)
					return;
				bool tragetingElementIndicator = (target.TryGetComponent<ElementNote>(out _));
				void SetActive(string name)
                {
                    __instance.transform.Find(name)?.gameObject?.SetActive(!tragetingElementIndicator);
                }
                SetActive("OutputElementHeader");
                SetActive("EverthingElse");
                SetActive("SelectElementHeader");
                SetActive("SelectedElement/Image");
			}
        }
	}
}

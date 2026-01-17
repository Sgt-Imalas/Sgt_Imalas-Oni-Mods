using BlueprintsV2.BlueprintsV2.BlueprintData.LiquidInfo;
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

                if(target.TryGetComponent<ElementPlanInfo>(out _) && target.TryGetComponent<Filterable>(out _))
                    __result = true;
            }
        }


        [HarmonyPatch(typeof(SingleItemSelectionSideScreenBase), nameof(SingleItemSelectionSideScreenBase.SetData))]
        public class FilterSideScreen_SetData_Patch
        {
            public static void Prefix(SingleItemSelectionSideScreenBase __instance, Dictionary<Tag, HashSet<Tag>> data)
            {
                if (__instance is not FilterSideScreen fs)
                    return;

                if (fs.targetFilterable is not ElementOnlyFilterable)
                    return;
                data = DiscoveredResources.Instance.GetDiscoveredResourcesFromTagSet(ElementOnlyFilterable.elementFilterableCategories);
				data.Add(GameTags.Void,[GameTags.Void]);
			}
        }

        [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.SetTarget))]
        public class FilterSideScreen_SetTarget_Patch
        {
            public static void Postfix(FilterSideScreen __instance, GameObject target)
            {
                bool targetingLiquidIndicator = (target.TryGetComponent<Filterable>(out _) && target.TryGetComponent<ElementPlanInfo>(out _));

                if (__instance.isLogicFilter)
                    return;

                void SetActive(string name)
                {
                    __instance.transform.Find(name)?.gameObject?.SetActive(!targetingLiquidIndicator);
                }
                SetActive("OutputElementHeader");
                SetActive("EverthingElse");
                SetActive("SelectElementHeader");
                SetActive("SelectedElement/Image");
			}
        }
	}
}

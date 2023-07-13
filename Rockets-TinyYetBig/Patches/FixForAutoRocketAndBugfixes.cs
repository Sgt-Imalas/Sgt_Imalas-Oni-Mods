using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class FixForAutoRocketAndBugfixes
    {
        /// <summary>
        /// Only affects debug create rocket command, prevents crash when it tries to load element with combustibleliquid tag by converting it to petroleum
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader), "GetElement")]
        public static class FixAutoRocket
        {
            public static void Postfix(Tag tag, ref Element __result)
            {
                if(tag== GameTags.CombustibleLiquid && __result == default(Element))
                {
                    ElementLoader.elementTagTable.TryGetValue(SimHashes.Petroleum.CreateTag(), out __result);
                }
            }

        }
        [HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.OnCopySettings))]
        public static class Fix_OxyliteFallingOutOnReload
        {
            public static void Postfix(object data, OxidizerTank __instance)
            {
                if (DlcManager.IsExpansion1Active()
                    && ((GameObject)data).TryGetComponent<OxidizerTank>(out var sourceTank)
                    && __instance.supportsMultipleOxidizers
                    && sourceTank.supportsMultipleOxidizers)
                {
                    if (__instance.TryGetComponent<FlatTagFilterable>(out var flatTagDestination) && sourceTank.TryGetComponent<FlatTagFilterable>(out var flatTagSource))
                    {
                        flatTagDestination.selectedTags = new List<Tag>(flatTagSource.selectedTags);

                        if (__instance.TryGetComponent<TreeFilterable>(out var TreeFilter))
                            TreeFilter.UpdateFilters(new HashSet<Tag>(flatTagDestination.selectedTags));
                    }
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UtilLibs;
using System.Reflection;
using PeterHan.PLib.Core;
using System.Reflection.Emit;
using static ResearchTypes;

namespace Rockets_TinyYetBig.Patches
{
    class BugfixPatches
    {
        /// <summary>
        /// Fixes freshly built rocket interior space exposure not working
        /// </summary>
        [HarmonyPatch(typeof(WorldContainer), "PlaceInteriorTemplate")]
        public class WorldContainer_PlaceInteriorTemplate_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();
                MethodInfo SimMsgModifyCellWorldZone = AccessTools.Method(
                    typeof(SimMessages),
                    nameof(SimMessages.ModifyCellWorldZone));


                for (var i = 1; i < codes.Count; ++i)
                {
                    if (codes[i].Calls(SimMsgModifyCellWorldZone) && codes[i - 1].LoadsConstant(7))
                    {
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, 255);
                        return codes;
                    }
                }

                SgtLogger.warning("WorldContainer Transpiler failed!");
                return codes;
            }

        }


        /// <summary>
        /// Only affects debug create rocket command, prevents crash when it tries to load element with combustibleliquid tag by converting it to petroleum
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader), "GetElement")]
        public static class FixAutoRocket
        {
            public static void Postfix(Tag tag, ref Element __result)
            {
                if (tag == GameTags.CombustibleLiquid && __result == default(Element))
                {
                    ElementLoader.elementTagTable.TryGetValue(SimHashes.Petroleum.CreateTag(), out __result);
                }
            }

        }

        public static void AttemptPatch(Harmony harmony)
        {
            if (!AppDomain.CurrentDomain.GetAssemblies().ToList().Any(ass => ass.FullName.Contains("StockBugFix")))
            {
                SgtLogger.l("applying oxidizer fix patch as stock bug fix is not installed");
                var postfixMethod = AccessTools.Method(
               typeof(OxidizerTank_Set_UserMaxCapacity_Patch_IncorporatedFromStockBugFix),
               nameof(OxidizerTank_Set_UserMaxCapacity_Patch_IncorporatedFromStockBugFix.Postfix),
               new Type[] { typeof(OxidizerTank) });
                Debug.Log(postfixMethod);

                harmony.Patch(OxidizerTank_Set_UserMaxCapacity_Patch_IncorporatedFromStockBugFix.TargetMethod(), postfix: new HarmonyMethod(postfixMethod));
            }
            else
                SgtLogger.l("Stockbugfix is installed, skipping oxidizer patch");
        }
        public static class OxidizerTank_Set_UserMaxCapacity_Patch_IncorporatedFromStockBugFix
        {
            /// <summary>
            /// Determines the target method to patch.
            /// </summary>
            /// <returns>The method which should be affected by this patch.</returns>
            internal static MethodBase TargetMethod()
            {
                return GetPropertySetter(typeof(OxidizerTank), nameof(OxidizerTank.UserMaxCapacity));
            }
            internal static MethodBase GetPropertySetter(Type baseType, string name)
            {
                var method = baseType.GetPropertySafe<float>(name, false)?.GetSetMethod();
                if (method == null)
                    SgtLogger.error("Unable to find target method for {0}.{1}!".F(baseType.Name,
                        name));
                return method;
            }

            /// <summary>
            /// Applied after the setter runs.
            /// </summary>
            public static void Postfix(OxidizerTank __instance)
            {
                var obj = __instance.gameObject;
                if (obj != null && obj.TryGetComponent(out Storage storage))
                    storage.Trigger((int)GameHashes.OnStorageChange, obj);
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

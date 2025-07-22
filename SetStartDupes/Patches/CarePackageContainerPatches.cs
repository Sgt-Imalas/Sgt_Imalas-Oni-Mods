using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace SetStartDupes.Patches
{
    /// <summary>
    /// Credit: Akis Bio Inks, https://github.com/aki-art/ONI-Mods/blob/master/PrintingPodRecharge/Patches/CarePackageContainerPatch.cs
    /// </summary>

    public class CarePackageContainerPatch
    {
        // Fixes incorrect KCAl formatting for some items that should display themselves so
        [HarmonyPatch(typeof(CarePackageContainer), "GetSpawnableQuantityOnly")]
        public class CarePackageContainer_GetSpawnableQuantityOnly_Patch
        {
            public static void Postfix(ref string __result, CarePackageInfo ___info)
            {
                var info = EdiblesManager.GetFoodInfo(___info.id);

                if (info != null && info.CaloriesPerUnit <= 0)
                    __result = string.Format(global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, ___info.quantity.ToString());
            }
        }

        [HarmonyPatch(typeof(CarePackageContainer), "GetCurrentQuantity")]
        public class CarePackageContainer_GetCurrentQuantity_Patch
        {
            public static void Postfix(WorldInventory inventory, ref string __result, CarePackageInfo ___info)
            {
                var info = EdiblesManager.GetFoodInfo(___info.id);

                if (info != null && info.CaloriesPerUnit <= 0)
                {
                    var amount = inventory.GetAmount(___info.id.ToTag(), false);
                    __result = string.Format(global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_CURRENT_AMOUNT, amount.ToString());
                }
            }
        }

        // Foods with 0 calorie (such as raw egg or some seeds) would appear glitchy and weird, with no animation
        [HarmonyPatch(typeof(CarePackageContainer), "SetAnimator")]
        public static class CarePackageContainer_SetAnimator_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
            {
                var f_CaloriesPerUnit = AccessTools.Field(typeof(EdiblesManager.FoodInfo), "CaloriesPerUnit");

                var codes = orig.ToList();

                var lnLoadCaloriesPerUnit = codes.FindIndex(ci => ci.LoadsField(f_CaloriesPerUnit));

                if (lnLoadCaloriesPerUnit == -1)
                    return codes;

                var lnStoreNum = codes.FindIndex(lnLoadCaloriesPerUnit, ci => ci.opcode == OpCodes.Stloc_1);

                if (lnStoreNum == -1)
                    return codes;

                var f_info = AccessTools.Field(typeof(CarePackageContainer), "info");
                var m_FilterFoodInfo = AccessTools.Method(typeof(CarePackageContainer_SetAnimator_Patch), "FilterFoodInfo", new[] { typeof(int), typeof(CarePackageInfo) });

                codes.InsertRange(lnStoreNum, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, f_info),
                    new CodeInstruction(OpCodes.Call, m_FilterFoodInfo)
                });

                //Log.PrintInstructions(codes);

                return codes;
            }

            private static int FilterFoodInfo(int existingValue, CarePackageInfo info)
            {
                return existingValue <= 0 ? (int)info.quantity : existingValue;
            }
        }


        [HarmonyPatch(typeof(CarePackageContainer), nameof(CarePackageContainer.OnSpawn))]
        public class CarePackageContainer_Add_SelectPackageButton
        {
            public static void Postfix(CarePackageContainer __instance)
            {
                List<CarePackageInfo> carePackageInfos = null;

                var BioInks_ModApi_Type = Type.GetType("PrintingPodRecharge.ModAPI, PrintingPodRecharge", false, false);
                if (BioInks_ModApi_Type != null)
                {
                    var currentPool = Traverse.Create(BioInks_ModApi_Type).Method("GetCurrentPool").GetValue() as List<CarePackageInfo>;
                    carePackageInfos = currentPool;
                }

                if (carePackageInfos != null)
                    SgtLogger.l("Bio Inks Pool loaded");



                if (__instance.reshuffleButton == null || !Config.Instance.RerollDuringGame_CarePackage)
                    return;

                var selectButton = Util.KInstantiateUI<KButton>(__instance.reshuffleButton.gameObject, __instance.reshuffleButton.transform.parent.gameObject, true);
                selectButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 20, 33f);
                UIUtils.FindAndDestroy(selectButton.transform, "Text");
                if (selectButton.transform.Find("FG").TryGetComponent<Image>(out var image))
                {
                    image.sprite = Assets.GetSprite("icon_gear");
                }

                selectButton.onClick += () =>
                {
                    UnityCarePackageSelectorScreen.ShowWindow(__instance, () => { }, carePackageInfos);
                };
            }
        }
    }
}

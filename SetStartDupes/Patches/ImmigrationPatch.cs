using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using UnityEngine;
using SetStartDupes.CarePackageEditor;

namespace SetStartDupes.Patches
{
    public class ImmigrationPatch
    {
        public static bool GeneratingFrontendList = false;


        [HarmonyPatch(typeof(Immigration), nameof(Immigration.ConfigureCarePackages))]
        public class AdditionalCarePackages
        {
            public static void Postfix(Immigration __instance)
            {
                if (!GeneratingFrontendList)
                {
                    CarePackageOutlineManager.RemoveDisabledVanillaPackages(__instance.carePackages);

					if (Config.Instance.AddAdditionalCarePackages)
						__instance.carePackages.AddRange(CarePackageOutlineManager.GetAllAdditionalCarePackages());
				}
            }
        }
        [HarmonyPatch(typeof(Immigration), nameof(Immigration.OnPrefabInit))]
        public class AdjustTimeOfReprint_Initial
        {
            public static bool Prefix(Immigration __instance)
            {
                if (GeneratingFrontendList)
                    return false;

                if (__instance.spawnInterval.Length >= 2)
                {
                    __instance.spawnInterval[0] = Mathf.RoundToInt(Config.Instance.PrintingPodRechargeTimeFirst * 600f);
                    __instance.spawnInterval[1] = Mathf.RoundToInt(Config.Instance.PrintingPodRechargeTime * 600f);

                }
                //for (int i = 0; i < __instance.spawnInterval.Length; i++)
                //{
                //    __instance.spawnInterval[i] = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
                //}
                //__instance.timeBeforeSpawn = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
                //for(int i = 0; i < __instance.spawnInterval.Length; i++)
                //{
                //    SgtLogger.l(__instance.spawnInterval[i].ToString(), i.ToString());
                //}
                return true;
            }
        }
    }
}

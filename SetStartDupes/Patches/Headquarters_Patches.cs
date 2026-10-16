using HarmonyLib;
using UnityEngine;

namespace SetStartDupes.Patches
{
	internal class Headquarters_Patches
	{

        [HarmonyPatch(typeof(HeadquartersConfig), nameof(HeadquartersConfig.ConfigureBuildingTemplate))]
        public class HeadquartersConfig_ConfigureBuildingTemplate_Patch
        {
            static bool Prepare() => Config.Instance.CarePackagesOnlyMode == Config.CarePackageLimiterMode.PrinterCheckbox; 

            public static void Postfix(GameObject go)
            {
                go.AddOrGet<PrintingPodCheckboxToggle>();
			}
        }

        [HarmonyPatch(typeof(ExobaseHeadquartersConfig), nameof(ExobaseHeadquartersConfig.ConfigureBuildingTemplate))]
        public class ExobaseHeadquartersConfig_ConfigureBuildingTemplate_Patch
		{
			static bool Prepare() => Config.Instance.CarePackagesOnlyMode == Config.CarePackageLimiterMode.PrinterCheckbox;
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<PrintingPodCheckboxToggle>();
			}
		}
	}
}

using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SkillsInfoScreen
{
	internal class Patches
	{
        static ManagementMenu.ManagementMenuToggleInfo AttributesInfo;
		static SkillsInfoScreen AttributesScreen;

		[HarmonyPatch(typeof(ManagementMenu), nameof(ManagementMenu.OnPrefabInit))]
        public class ManagementMenu_OnPrefabInit_Patch
        {
            public static void Prefix(ManagementMenu __instance)
            {
				///make screen
				var vitalsScreenGO = __instance.vitalsScreen.gameObject;
				var screenGO = Util.KInstantiateUI(vitalsScreenGO, vitalsScreenGO.transform.parent.gameObject);
				screenGO.transform.name = nameof(VitalsTableScreen);
				UnityEngine.Object.DestroyImmediate(screenGO.GetComponent<VitalsTableScreen>());
				AttributesScreen = screenGO.AddOrGet<SkillsInfoScreen>();
				AttributesScreen.GetPrefabRefs(__instance.vitalsScreen);
				screenGO.SetActive(true);

				SgtLogger.l("SkillsInfoScreen gameobject:");
				UIUtils.ListAllChildrenWithComponents(screenGO.transform);
				UIUtils.ListAllChildrenPath(screenGO.transform);

				///make menu info entry
				AttributesInfo = new ManagementMenu.ManagementMenuToggleInfo(
					STRINGS.UI.CHARACTERCONTAINER_SKILLS_TITLE, 
					"OverviewUI_codex", 
					hotkey: Action.NumActions, 
					tooltip: STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_VITALS);

				//__instance.AddToggleTooltipForResearch(AttributesInfo, "disabled tooltip");
				__instance.AddToggleTooltip(AttributesInfo);		
				AttributesInfo.prefabOverride = UnityEngine.Object.Instantiate(__instance.researchButtonPrefab);
				AttributesInfo.prefabOverride.transform.Find("TextContainer/Text").GetComponent<LocText>().text = STRINGS.UI.CHARACTERCONTAINER_SKILLS_TITLE;

				__instance.ScreenInfoMatch.Add(AttributesInfo, new()
				{
					screen = AttributesScreen,
					toggleInfo = AttributesInfo,
					cancelHandler = null
				});
			}
        }

		[HarmonyPatch(typeof(KIconToggleMenu), nameof(KIconToggleMenu.Setup), [typeof(IList<KIconToggleMenu.ToggleInfo>)])]
		public class KIconToggleMenu_Setup_Patch
		{
			public static void Prefix(KIconToggleMenu __instance, IList<KIconToggleMenu.ToggleInfo> toggleInfo)
			{
				if (__instance is not ManagementMenu)
					return;

				if (toggleInfo.Any(info => info.icon == "OverviewUI_codex"))
					return;

				toggleInfo.Insert(0, AttributesInfo);
			}
		}


		//static GameObject SecondaryHeaderRow;
		//[HarmonyPatch(typeof(JobsTableScreen), nameof(JobsTableScreen.OnActivate))]
		//public class JobsTableScreen_OnActivate_Patch
		//{
		//	public static void Postfix(JobsTableScreen __instance)
		//	{
		//		var header = __instance.header_row.gameObject;
		//		SecondaryHeaderRow = Util.KInstantiateUI(header, header.transform.parent.gameObject, true);
		//	}
		//}
	}
}

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
        static ManagementMenu.ManagementMenuToggleInfo SkillsOverviewInfo;
		static AttributeInfoScreen AttributesScreen;
		static SkillsOverviewInfoScreen SkillsOverviewScreen;

		public static string SkillsOverviewName = "TMP";

		const string AttributeIcon = "OverviewUI_codex";
		const string SkillsIcon = "OverviewUI_roles_icon";

		[HarmonyPatch(typeof(ManagementMenu), nameof(ManagementMenu.OnPrefabInit))]
        public class ManagementMenu_OnPrefabInit_Patch
        {
			[HarmonyPrefix]
            public static void CreateAttributeListScreen(ManagementMenu __instance)
            {
				///make attribute screen
				var vitalsScreenGO = __instance.vitalsScreen.gameObject;
				var screenGO = Util.KInstantiateUI(vitalsScreenGO, vitalsScreenGO.transform.parent.gameObject);
				screenGO.transform.name = nameof(AttributeInfoScreen);
				UnityEngine.Object.DestroyImmediate(screenGO.GetComponent<VitalsTableScreen>());
				AttributesScreen = screenGO.AddOrGet<AttributeInfoScreen>();
				AttributesScreen.GetPrefabRefs(__instance.vitalsScreen);
				screenGO.SetActive(true);

				//SgtLogger.l("SkillsInfoScreen gameobject:");
				//UIUtils.ListAllChildrenWithComponents(screenGO.transform);
				//UIUtils.ListAllChildrenPath(screenGO.transform);

				///make menu info entry
				AttributesInfo = new ManagementMenu.ManagementMenuToggleInfo(
					STRINGS.UI.CHARACTERCONTAINER_SKILLS_TITLE,
					AttributeIcon, 
					hotkey: Action.NumActions, 
					tooltip: "");

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

			[HarmonyPrefix]
			public static void CreateSkillsListScreen(ManagementMenu __instance)
			{
				///make attribute screen
				var sourceScreenGO = __instance.vitalsScreen.gameObject;
				var screenGO = Util.KInstantiateUI(sourceScreenGO, sourceScreenGO.transform.parent.gameObject);
				screenGO.transform.name = nameof(SkillsOverviewInfoScreen);
				UnityEngine.Object.DestroyImmediate(screenGO.GetComponent<VitalsTableScreen>());
				SkillsOverviewScreen = screenGO.AddOrGet<SkillsOverviewInfoScreen>();
				SkillsOverviewScreen.GetPrefabRefs(__instance.vitalsScreen);
				screenGO.SetActive(true);

				SgtLogger.l("SkillsOverviewInfoScreen gameobject:");
				UIUtils.ListAllChildrenWithComponents(screenGO.transform);
				UIUtils.ListAllChildrenPath(screenGO.transform);

				///make menu info entry
				SkillsOverviewInfo = new ManagementMenu.ManagementMenuToggleInfo(
					SkillsOverviewName,
					SkillsIcon,
					hotkey: Action.NumActions,
					tooltip: "");

				//__instance.AddToggleTooltipForResearch(AttributesInfo, "disabled tooltip");
				__instance.AddToggleTooltip(AttributesInfo);
				AttributesInfo.prefabOverride = UnityEngine.Object.Instantiate(__instance.researchButtonPrefab);
				AttributesInfo.prefabOverride.transform.Find("TextContainer/Text").GetComponent<LocText>().text = SkillsOverviewName;

				__instance.ScreenInfoMatch.Add(SkillsOverviewInfo, new()
				{
					screen = SkillsOverviewScreen,
					toggleInfo = SkillsOverviewInfo,
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

				if (!toggleInfo.Any(info => info.icon == AttributeIcon))
					toggleInfo.Insert(0, AttributesInfo);

				if (!toggleInfo.Any(info => info.icon == SkillsIcon))
					toggleInfo.Insert(1, SkillsOverviewInfo);

			}
		}


		[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				SkillsOverviewName = STRINGS.UI.DETAILTABS.STATS.NAME + " " + STRINGS.UI.DETAILTABS.NEEDS.OVERVIEW;

				//Strings.Add(SkillOverviewKey, skillsoverview);
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

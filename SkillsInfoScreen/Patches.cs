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
				AttributesScreen.FetchOptionsPanel(__instance.jobsScreen);
				screenGO.SetActive(true);

				//SgtLogger.l("SkillsInfoScreen gameobject:");
				//UIUtils.ListAllChildrenWithComponents(screenGO.transform);
				//UIUtils.ListAllChildrenPath(screenGO.transform);

				///make menu info entry
				AttributesInfo = new ManagementMenu.ManagementMenuToggleInfo(
					 STRINGS.UI.CHARACTERCONTAINER_SKILLS_TITLE,
					AttributeIcon, 
					hotkey: Action.NumActions, 
					tooltip: STRINGS.UI.CHARACTERCONTAINER_SKILLS_TITLE);

				//__instance.AddToggleTooltipForResearch(AttributesInfo, "disabled tooltip");
				__instance.AddToggleTooltip(AttributesInfo);

				ModIntegration_CleanHud.TryGetConfigValue("UseSmallButtons", out bool useSmallButtons);
				AttributesInfo.prefabOverride =  Util.KInstantiateUI<KToggle>((useSmallButtons ? __instance.smallPrefab : __instance.prefab).gameObject);

				//prevent word wrapping due to shrunken buttons
				__instance.prefab.transform.Find("TextContainer/Text").GetComponent<LocText>().enableWordWrapping = false;

				__instance.ScreenInfoMatch.Add(AttributesInfo, new()
				{
					screen = AttributesScreen,
					toggleInfo = AttributesInfo,
					cancelHandler = null
				});
			}

			//[HarmonyPrefix] ///not working properly yet.
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
				ModIntegration_CleanHud.TryGetConfigValue("UseSmallButtons", out bool useSmallButtons);
				AttributesInfo.prefabOverride = Util.KInstantiateUI<KToggle>((useSmallButtons ? __instance.smallPrefab : __instance.prefab).gameObject);

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

				var skillsToggle = toggleInfo.FirstOrDefault(toggleInfo => toggleInfo.icon == "OverviewUI_jobs_icon");
				int insertAt = 0;
				if(skillsToggle != null)
					insertAt = toggleInfo.IndexOf(skillsToggle);

				if (!toggleInfo.Any(info => info.icon == AttributeIcon))
					toggleInfo.Insert(insertAt, AttributesInfo);


				//if (!toggleInfo.Any(info => info.icon == SkillsIcon))
				//	toggleInfo.Insert(1, SkillsOverviewInfo);

			}
		}

		[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				SkillsOverviewName = STRINGS.UI.DETAILTABS.STATS.NAME + " " + STRINGS.UI.DETAILTABS.NEEDS.OVERVIEW;
				LocalisationUtil.Translate(typeof(MOD_STRINGS));

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

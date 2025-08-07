using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace DupePrioPresetManager
{
	internal class ResearchScreenPatches
	{
		internal static void ExecuteAll(Harmony harmony)
		{
			var researchscreen_onspawn = AccessTools.Method(typeof(ResearchScreen), nameof(ResearchScreen.OnSpawn));
			var researchscreen_onspawn_postfix = AccessTools.Method(typeof(ResearchScreenPatches), nameof(ResearchScreenPatches.ResearchScreen_OnSpawn_Postfix));

			var researchscreen_onshow = AccessTools.Method(typeof(ResearchScreen), nameof(ResearchScreen.OnShow));
			var researchscreen_onshow_postfix = AccessTools.Method(typeof(ResearchScreenPatches), nameof(ResearchScreenPatches.ResearcScreen_OnShow_Postfix));

			harmony.Patch(researchscreen_onspawn, postfix: new HarmonyMethod(researchscreen_onspawn_postfix));
			harmony.Patch(researchscreen_onshow, postfix: new HarmonyMethod(researchscreen_onshow_postfix));

		}

		static void ResearchScreen_OnSpawn_Postfix(ResearchScreen __instance)
		{
			var button = Util.KInstantiateUI(__instance.sideBar.clearSearchButton.gameObject, __instance.sideBar.allFilter.transform.parent.gameObject, true);
			button.name = "presetButton";
			button.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconPaste");

			if(button.TryGetComponent<LayoutElement>(out var LE))
			{
				LE.minHeight = 36;
				LE.minWidth = 36;
			}
			//__instance.sideBar.transform.Find("Content/Queue").gameObject.SetActive(true);
			//__instance.sideBar.transform.Find("Content/Queue/Scroll/Rect/QueueContent/TechWidgetPrefab").gameObject.SetActive(true);
			//__instance.sideBar.transform.Find("Content/Queue/Scroll/Rect/QueueContent/TechWidgetPrefabAlt").gameObject.SetActive(true);
			//__instance.sideBar.transform.Find("Content/Queue/Scroll/Rect/QueueContent/TechItemWidgetPrefab").gameObject.SetActive(true);

			var bt = button.GetComponent<KButton>();
			bt.ClearOnClick();
			bt.onClick += () => UnityPresetScreen_ResearchQueue.ShowWindow(() => { });
		}
		static void ResearcScreen_OnShow_Postfix(ResearchScreen __instance, bool show)
		{
			if(show) 
				ModAssets.ParentScreen = __instance.transform.gameObject;
		}
	}
}

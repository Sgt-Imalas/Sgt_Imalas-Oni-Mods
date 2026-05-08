using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Content.Scripts.StarmapEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static STRINGS.UI.UISIDESCREENS;

namespace Rockets_TinyYetBig.Patches
{
	internal class SimpleInfoScreen_Patches
	{
		public static void ClearAll()
		{
			StationPartPanel = null;
			//	StationPartDrawer = null;
			LastParent = null;
			RocketModulesPanel = null;
		}
		public static CollapsibleDetailContentPanel StationPartPanel = null;
		public static CollapsibleDetailContentPanel RocketModulesPanel = null;
		//public static DetailsPanelDrawer StationPartDrawer = null;
		public static GameObject LastParent = null;

		///[HarmonyPatch(typeof(SimpleInfoScreen), nameof(SimpleInfoScreen.OnSelectTarget))]
		public class SimpleInfoScreen_OnSelectTarget_Patch
		{

			private static void InitStationPartPanel(SimpleInfoScreen instance)
			{
				if (StationPartPanel == null || LastParent != instance.gameObject)
				{
					if (StationPartPanel != null)
					{
						UnityEngine.Object.Destroy(StationPartPanel.gameObject);
					}
					StationPartPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, instance.gameObject);
					//StationPartDrawer = new DetailsPanelDrawer(instance.attributesLabelTemplate, StationPartPanel.Content.gameObject);
					LastParent = instance.gameObject;
					StationPartPanel.HeaderLabel.text = "Station Parts";
				}
			}
			private static void InitRocketModulesPanel(SimpleInfoScreen instance)
			{
				if (RocketModulesPanel == null || LastParent != instance.gameObject)
				{
					if (RocketModulesPanel != null)
					{
						UnityEngine.Object.Destroy(RocketModulesPanel.gameObject);
					}
					RocketModulesPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, instance.gameObject);
					//StationPartDrawer = new DetailsPanelDrawer(instance.attributesLabelTemplate, StationPartPanel.Content.gameObject);
					LastParent = instance.gameObject;
					RocketModulesPanel.HeaderLabel.text = STRINGS.UI.KLEI_INVENTORY_SCREEN.SUBCATEGORIES.RTB_MODULE_SKINS.ToString().ToUpperInvariant();
				}
			}
			public static void Prefix(SimpleInfoScreen __instance, GameObject target)
			{
				if (target != null && target.TryGetComponent<StationDeconstructable>(out var decon))
				{
					InitStationPartPanel(__instance);
					bool shouldShow = false;
					StationPartPanel.gameObject.SetActive(true);
					// TraitsDrawer.BeginDrawing();
					int count = 0;

					foreach (var entry in StationPartPanel.labels)
					{
						entry.Value.used = false;
					}

					foreach (var res in decon.Resources)
					{
						StationPartPanel.SetLabel("traitLabel_" + count++, res.Item + " x" + res.Amount, "");
						++count;
						shouldShow = true;
					}
					if (!shouldShow)
					{
						StationPartPanel.gameObject.SetActive(false);
					}
					else
						StationPartPanel.Commit();
				}
				else
				{
					StationPartPanel?.gameObject.SetActive(false);
				}

				//if (target != null && target.TryGetComponent<Clustercraft>(out var rocket) && rocket.ModuleInterface.ClusterModules.Any())
				//{
				//	InitRocketModulesPanel(__instance);
				//	RocketModulesPanel.gameObject.SetActive(true);
				//	// TraitsDrawer.BeginDrawing();
				//	int count = 0;

				//	foreach (var entry in RocketModulesPanel.labels)
				//	{
				//		entry.Value.used = false;
				//	}
				//	var modules = rocket.ModuleInterface.ClusterModules;
				//	for (int i = modules.Count() - 1; i >= 0; --i)
				//	{
				//		var module = modules[i].Get();
				//		RocketModulesPanel.SetLabelWithButton("moduleLabel_" + count++, module.GetProperName(), SELECTMODULESIDESCREEN.TITLE, () => __instance.SetTarget(module.gameObject));
				//	}
				//	RocketModulesPanel.Commit();
				//}
				//else
				//{
				//	RocketModulesPanel?.gameObject.SetActive(false);
				//}
			}
		}
	}
}

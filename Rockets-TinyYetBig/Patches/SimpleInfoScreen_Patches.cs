using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Content.Scripts.StarmapEntities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class SimpleInfoScreen_Patches
	{
		public static void ClearAll()
		{
			SimpleInfoScreen_OnSelectTarget_Patch.StationPartPanel = null;
			SimpleInfoScreen_OnSelectTarget_Patch.StationPartDrawer = null;
			SimpleInfoScreen_OnSelectTarget_Patch.LastParent = null;
		}

		//[HarmonyPatch(typeof(SimpleInfoScreen), nameof(SimpleInfoScreen.OnSelectTarget))]
		public class SimpleInfoScreen_OnSelectTarget_Patch
		{
			public static CollapsibleDetailContentPanel StationPartPanel = null;
			public static DetailsPanelDrawer StationPartDrawer = null;
			public static GameObject LastParent = null;

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
						StationPartPanel.SetLabel("traitLabel_" + count, res.Item + " x"+res.Amount , "");
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
			}
		}
	}
}

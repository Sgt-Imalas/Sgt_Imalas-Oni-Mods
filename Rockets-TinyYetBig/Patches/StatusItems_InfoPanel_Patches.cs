using Database;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Rockets_TinyYetBig.STRINGS;
using static Rockets_TinyYetBig.STRINGS.UI_MOD.CLUSTERMAPROCKETSIDESCREEN;

namespace Rockets_TinyYetBig.Patches
{
	class StatusItems_InfoPanel_Patches
	{
		class ExtendSolarNotification
		{
			[HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
			public static class SolarNoseconeStatusItems
			{
				public static void Postfix(BuildingStatusItems __instance)
				{
					__instance.ModuleSolarPanelWattage.resolveStringCallback = (Func<string, object, string>)((str, data) =>
					{
						if (data is ModuleSolarPanel)
						{
							ModuleSolarPanel moduleSolarPanel = (ModuleSolarPanel)data;
							str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(moduleSolarPanel.CurrentWattage));
						}
						else if (data is ModuleSolarPanelAdjustable)
						{
							ModuleSolarPanelAdjustable moduleSolarPanel = (ModuleSolarPanelAdjustable)data;
							str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(moduleSolarPanel.CurrentWattage));
						}

						return str;
					});
				}

			}

		}

	}
}

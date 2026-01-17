using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration.EnumMirrors;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration
{
	public static class PlanningTool_EnumMapping
	{
		static Dictionary<int,Color> ColorMap;
		public static Color AsColor(PlanColor planColor)
		{
			if (ColorMap == null)
			{
				if (!PlanningTool_Integration.ModActive)
					return Color.gray;

				///Initialize color dictionary
				try
				{
					AccessTools.Method(PlanningTool_Integration.t_PlanColorExtension, "AsColor").Invoke(null, [0]);
				}
				catch { }
				var map = (IDictionary)AccessTools.Field(PlanningTool_Integration.t_PlanColorExtension, "ColorMap").GetValue(null);
				if (map == null)
					return Color.gray;
				ColorMap = map.CastDict().ToDictionary(entry => (int)entry.Key, entry => (Color)entry.Value);
				SgtLogger.l("PlanningTool Color map copy initialized with " + ColorMap.Count+" entries");
			}

			if (ColorMap.ContainsKey((int)planColor))
			{
				return ColorMap[(int)planColor];
			}

			Debug.LogWarning("[BlueprintsV2/PlanningTool] Color with enum value " + planColor + " not recognized, returning default color");
			return Color.gray;
		}
		private static IEnumerable<DictionaryEntry> CastDict(this IDictionary dictionary)
		{
			foreach (DictionaryEntry entry in dictionary)
			{
				yield return entry;
			}
		}
	} 
}

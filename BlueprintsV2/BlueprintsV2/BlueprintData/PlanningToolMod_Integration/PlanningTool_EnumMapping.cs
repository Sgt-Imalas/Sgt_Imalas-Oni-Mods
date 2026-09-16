using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration.EnumMirrors;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration
{
	public static class PlanningTool_EnumMapping
	{
		static Dictionary<int,Color> ColorMap;

		public static Color FallbackColor(PlanColor color)
		{
			return color switch
			{
				PlanColor.Violet => Color.violet,
				PlanColor.Gray => Color.gray,
				PlanColor.Blue => Color.blue,
				PlanColor.Green => Color.green,
				PlanColor.Red => Color.red,
				PlanColor.Yellow => Color.yellow,
				PlanColor.Cyan => Color.cyan,
				PlanColor.White => Color.white,
				PlanColor.Magenta => Color.magenta,
				PlanColor.Orange => Color.orange,
				PlanColor.Black => Color.black,
				_ => Color.darkGray			
			};
		}

		public static Color AsColor(PlanColor planColor)
		{
			if (ColorMap == null)
			{
				if (!PlanningTool_Integration.ModActive)
					return FallbackColor(planColor);

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

using HarmonyLib;
using LogicSatellites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace LogicSatellites.Patches
{
	class GameUtil_Patches
	{

		static bool IsCheckingGameObject = false;

		[HarmonyPatch(typeof(GameUtil), nameof(GameUtil.GetUnitFormattedName), [typeof(string), typeof(float), typeof(bool)])]
		public class GameUtil_GetUnitFormattedName_Patch
		{
			public static void Postfix(string name, float count, ref string __result)
			{
				if (name != STRINGS.ITEMS.LS_CLUSTERSATELLITEPART.TITLE || IsCheckingGameObject)
					return;

				__result = StringFormatter.Replace(global::STRINGS.UI.NAME_WITH_UNITS, "{0}", name).Replace("{1}", $"{(count / SatelliteComponentConfig.MASS):0.##}");
			}
		}
		[HarmonyPatch(typeof(GameUtil), nameof(GameUtil.GetUnitFormattedName), [typeof(GameObject), typeof(bool)])]
		public class GameUtil_GetUnitFormattedName_Patch2
		{
			public static void Prefix() => IsCheckingGameObject = true;
			public static void Postfix() => IsCheckingGameObject = false;
		}
	}
}

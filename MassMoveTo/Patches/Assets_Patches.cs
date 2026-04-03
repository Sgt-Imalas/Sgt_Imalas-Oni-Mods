using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace MassMoveTo.Patches
{
	internal class Assets_Patches
	{

		/// <summary>
		/// Add tool icon
		/// </summary>
		[HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public class Assets_OnPrefabInit_Patch
		{
			public static string MassMoveToolIconId = "MassMoveToolIcon";
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{
				ModAssets.MassMoveToolIcon = InjectionMethods.AddSpriteToAssets(__instance, MassMoveToolIconId);
			}
		}
	}
}

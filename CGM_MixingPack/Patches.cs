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
using static CGM_MixingPack.ModAssets;

namespace CGM_MixingPack
{
	internal class Patches
	{

		[HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.OnPrefabInit))]
		public class CustomGameSettings_OnPrefabInit_Patch
		{
			public static void Postfix(CustomGameSettings __instance)
			{
				ModAssets.RegisterMixings(__instance);
			}
		}
	}
}

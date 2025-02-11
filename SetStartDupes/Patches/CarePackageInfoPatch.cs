using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SetStartDupes.Patches
{
	internal class CarePackageInfoPatch
	{

		[HarmonyPatch(typeof(CarePackageInfo), MethodType.Constructor, [typeof(string), typeof(float), typeof(Func<bool>), typeof(string)])]
		public class CarePackageInfo_Constructor_1_Patch
		{
			public static void Prefix(CarePackageInfo __instance, ref float amount)
			{
				amount = GetTotalCarePackageAmount(amount); 
			}
		}
		[HarmonyPatch(typeof(CarePackageInfo), MethodType.Constructor, [typeof(string), typeof(float), typeof(Func<bool>)])]
		public class CarePackageInfo_Constructor_2_Patch
		{
			public static void Prefix(CarePackageInfo __instance, ref float amount)
			{
				amount = GetTotalCarePackageAmount(amount);
			}
		}
		static float GetTotalCarePackageAmount(float baseAmount)
		{
			float multiplier = Config.Instance.CarePackageMultiplier;
			if (Mathf.Approximately(multiplier, 1f))
			{
				return baseAmount;
			}
			var multiplied = Mathf.Max(1f, baseAmount * multiplier);
			return Mathf.RoundToInt(multiplied);
		}
	}
}

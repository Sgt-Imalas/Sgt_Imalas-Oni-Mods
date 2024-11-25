using AkisSnowThings.Content.Scripts.MultiTools;
using HarmonyLib;
using UnityEngine;

namespace AkisSnowThings.Patches
{
	public class MinionConfigPatch
	{

		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.BaseMinion))]
		public class MinionConfig_CreatePrefab_Patch
		{
			public static void Postfix(GameObject __result)
			{
				SnowBeam.AddSnapOn(__result);
			}
		}


		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.SetupLaserEffects))]
		public class MinionConfig_SetupLaserEffects_Patch
		{
			public static void Postfix(GameObject prefab)
			{
				SnowBeam.AddLaserEffect(prefab);
			}
		}
	}
}

using HarmonyLib;
using Imalas_TwitchChaosEvents.Creeper;
using Imalas_TwitchChaosEvents.Fire;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Attachments
{
	internal class AttachmentPatches
	{
		[HarmonyPatch(typeof(Health), nameof(Health.OnSpawn))]
		public class HealthDamageHandler_Addon_Patch
		{
			public static void Postfix(Health __instance)
			{
				__instance.gameObject.AddOrGet<Health_DamageHander>();
			}

		}

		[HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.OnSpawn))]
		public class Minion_AddFlipper_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<Minion_Flipper>();
			}

		}
	}
}

using HarmonyLib;
using Imalas_TwitchChaosEvents.Creeper;
using Imalas_TwitchChaosEvents.Elements;
using Imalas_TwitchChaosEvents.Fire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.ModPatches
{
	internal class SaveGame_Patches
	{

		[HarmonyPatch(typeof(SaveGame), nameof(SaveGame.OnPrefabInit))]
		public class SaveGame_OnPrefabInit_Patch
		{
			public static void Postfix(SaveGame __instance)
			{
				__instance.gameObject.AddOrGet<ChaosTwitch_SaveGameStorage>();
				CreeperController.instance = __instance.gameObject.AddOrGet<CreeperController>();
				FireManager.Instance = __instance.gameObject.AddOrGet<FireManager>();
				RandomTickManager.Instance = __instance.gameObject.AddOrGet<RandomTickManager>();
			}
		}
	}
}

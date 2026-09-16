using HarmonyLib;
using Imalas_TwitchChaosEvents.BeeGeyser;

namespace Imalas_TwitchChaosEvents.ModPatches
{
    class Game_Patches
	{
		[HarmonyPatch(typeof(Game), nameof(Game.DestroyInstances))]
		public class Clear_ForbiddenList
		{
			public static void Prefix()
			{
				BeeCoat.Coats.Clear();
			}
		}
	}
}

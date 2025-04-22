using HarmonyLib;
using Imalas_TwitchChaosEvents.BeeGeyser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

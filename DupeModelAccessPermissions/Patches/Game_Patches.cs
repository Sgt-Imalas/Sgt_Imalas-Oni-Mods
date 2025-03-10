using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DupeModelAccessPermissions.Patches
{
	internal class Game_Patches
	{     
		/// <summary>
		/// Clear up bionics list on exit
		/// </summary>
		[HarmonyPatch(typeof(Game), nameof(Game.OnDestroy))]
		public static class GameOnDestroy
		{
			public static void Postfix() => Mod.BionicMinionInstanceIds.Clear();
		}
	}
}

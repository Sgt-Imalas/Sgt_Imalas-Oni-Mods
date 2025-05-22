using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
	/// <summary>
	/// Clean up cached tile components when game is destroyed
	/// </summary>
	public class GamePatch
	{
		public static void PatchAll(Harmony harmony)
		{
			Game_DestroyInstances_Patch.Patch(harmony);
		}

		// needs to be manually patched, otherwise it loads Game type too early, which in turns CustomGameSettings, and breaks some translations on it		
		public class Game_DestroyInstances_Patch
		{
			public static void Patch(Harmony harmony)
			{
				var m_OnSpawn = AccessTools.Method("Game, Assembly-CSharp:DestroyInstances");
				var m_Prefix = AccessTools.Method(typeof(Game_DestroyInstances_Patch), "Prefix");
				harmony.Patch(m_OnSpawn, new HarmonyMethod(m_Prefix));
			}
			public static void Prefix()
			{
				TrueTiles_OverrideStorage.Cmps.Clear();
			}
		}
	}
}

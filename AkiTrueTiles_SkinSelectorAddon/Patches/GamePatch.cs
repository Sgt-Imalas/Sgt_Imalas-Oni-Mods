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
	/// Mirrored from true tiles, used to initialize the parallel grid structure
	/// </summary>
	public class GamePatch
	{
		public static void PatchAll(Harmony harmony)
		{
			Game_OnSpawn_Patch.Patch(harmony);
			Game_DestroyInstances_Patch.Patch(harmony);
		}

		// needs to be manually patched, otherwise it loads Game type too early, which in turns CustomGameSettings, and breaks some translations on it
		public class Game_OnSpawn_Patch
		{
			public static void Patch(Harmony harmony)
			{
				var m_OnSpawn = AccessTools.Method("Game, Assembly-CSharp:OnSpawn");
				var m_Postfix = AccessTools.Method(typeof(Game_OnSpawn_Patch), "Postfix");
				harmony.Patch(m_OnSpawn, null, new HarmonyMethod(m_Postfix));
			}

			public static void Postfix()
			{
				SgtLogger.l("Game_OnSpawn_Patch.Postfix");
				ModAssets.TT_Initialize();
			}
		}
		public class Game_DestroyInstances_Patch
		{
			public static void Patch(Harmony harmony)
			{
				var m_OnSpawn = AccessTools.Method("Game, Assembly-CSharp:DestroyInstances");
				var m_Postfix = AccessTools.Method(typeof(Game_OnSpawn_Patch), "Postfix");
				harmony.Patch(m_OnSpawn, null, new HarmonyMethod(m_Postfix));
			}
			public static void Prefix()
			{
				TrueTiles_OverrideStorage.Cmps.Clear();
			}
		}
	}
}

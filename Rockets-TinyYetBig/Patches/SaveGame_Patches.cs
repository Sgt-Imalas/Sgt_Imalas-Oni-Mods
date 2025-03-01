using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
	internal class SaveGame_Patches
	{
		/// <summary>
		/// big TY to Aki for this implementation
		/// Adds several global components to the savegame object to be serialized
		/// </summary>
		/// 
		[HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
		public class SaveGame_OnPrefabInit_Patch
		{
			public static void Postfix(SaveGame __instance)
			{
				__instance.gameObject.AddOrGet<RainbowSpec>();
				RTB_SavegameStoredSettings.Instance = __instance.gameObject.AddOrGet<RTB_SavegameStoredSettings>();
				DockingManagerSingleton.Instance = __instance.gameObject.AddOrGet<DockingManagerSingleton>();
			}
		}
	}
}

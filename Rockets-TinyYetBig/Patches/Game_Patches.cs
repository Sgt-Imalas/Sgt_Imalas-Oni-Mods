using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class Game_Patches
	{

        [HarmonyPatch(typeof(Game), nameof(Game.OnLoadLevel))]
        public class Game_OnLoadLevel_Patch
        {
            public static void Postfix()
            {
                SpaceStationManager.ClearAll();
                RTB_SavegameStoredSettings.ClearAll();
				SimpleInfoScreen_Patches.ClearAll();
                WorldSelector_Patches.ClearAll();
			}
        }
	}
}

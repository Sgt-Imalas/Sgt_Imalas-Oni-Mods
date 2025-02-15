using HarmonyLib;
using SettingsSyncGroups.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SettingsSyncGroups.Patches
{
	internal class GamePatch
	{

		[HarmonyPatch(typeof(Game), nameof(Game.DestroyInstances))]
		public class Clear_ForbiddenList
		{
			public static void Prefix()
			{
				SgtLogger.l("Clearing GroupCarrier Cache");
				SyncGroupCarrier.AllCarriersByTag.Clear();
			}
		}

	}
}

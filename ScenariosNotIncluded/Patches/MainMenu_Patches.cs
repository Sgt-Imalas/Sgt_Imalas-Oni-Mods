using HarmonyLib;
using Klei;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static StateMachine;

namespace ScenariosNotIncluded.Patches
{
    class MainMenu_Patches
	{
		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnPrefabInit))]
		public static class MainMenu_OnPrefabInit_Patch
		{
			public static void Prefix(MainMenu __instance)
			{
				var openMenu =  () =>
				{
					var ModUploader = Util.KInstantiateUI(ScreenPrefabs.Instance.RailModUploadMenu.gameObject, Global.Instance.globalCanvas, true);
				};
				
				__instance.MakeButton(new MainMenu.ButtonInfo("Open Mod Menu", openMenu, 22, __instance.normalButtonStyle));

			}
		}
	}
}

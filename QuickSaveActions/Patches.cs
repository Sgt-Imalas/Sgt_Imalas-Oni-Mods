using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static QuickSaveActions.ModAssets;

namespace QuickSaveActions
{
	internal class Patches
	{

		[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.OnKeyDown))]
		public class PlayerController_OnKeyDown_Patch
		{
			public static void Prefix(KButtonEvent e)
			{
				if(e.IsAction(ModAssets.QuickSave.GetKAction()) && ModAssets.TryQuickSave()
				|| e.IsAction(ModAssets.QuickLoad.GetKAction()) && ModAssets.TryQuickLoad()
				)
					e.Consumed = true;
			}
		}
	}
}

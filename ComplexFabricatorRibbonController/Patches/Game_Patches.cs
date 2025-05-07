using ComplexFabricatorRibbonController.Content.Scripts.Buildings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ComplexFabricatorRibbonController.Patches
{
    class Game_Patches
    {
		[HarmonyPatch(typeof(Game), nameof(Game.DestroyInstances))]
		public class Clear_ForbiddenList
		{
			public static void Prefix()
			{
				SgtLogger.l("Clearing ComplexFabricatorRibbonControllerAttachment Cache");
				ComplexFabricatorRecipeControlAttachment.ClearCache();
			}
		}
	}
}

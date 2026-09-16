using ComplexFabricatorRibbonController.Content.Scripts.Buildings;
using HarmonyLib;
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

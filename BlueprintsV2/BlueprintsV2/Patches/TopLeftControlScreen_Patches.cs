using BlueprintsV2.BlueprintsV2.UnityUI;
using HarmonyLib;

namespace BlueprintsV2.BlueprintsV2.Patches
{
	internal class TopLeftControlScreen_Patches
	{

		[HarmonyPatch(typeof(TopLeftControlScreen), nameof(TopLeftControlScreen.OnActivate))]
		public static class Add_Colorable_Button
		{
			public static void Postfix(TopLeftControlScreen __instance)
			{
				var buttonMenu = Util.KInstantiateUI(ModAssets.NoteOptionScreenGO, __instance.secondaryRow.gameObject, true).transform;
				buttonMenu.name = "BPV2_NoteMenu";
				var logic = buttonMenu.gameObject.AddOrGet<NoteVisualizerSettings>();
				var debugTimeButton = Util.KInstantiateUI(__instance.kleiItemDropButton.gameObject, buttonMenu.gameObject, true).transform;
				debugTimeButton.name = "VisibilityToggle";
				logic.Init(debugTimeButton.GetComponent<MultiToggle>());
				logic.transform.SetSiblingIndex(__instance.kleiItemDropButton.transform.GetSiblingIndex());
			}
		}
	}
}

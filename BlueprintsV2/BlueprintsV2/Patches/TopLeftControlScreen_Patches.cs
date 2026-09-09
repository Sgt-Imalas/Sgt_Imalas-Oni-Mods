using BlueprintsV2.BlueprintData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.Patches
{
	internal class TopLeftControlScreen_Patches
	{

		[HarmonyPatch(typeof(TopLeftControlScreen), nameof(TopLeftControlScreen.OnActivate))]
		public static class Add_Colorable_Button
		{
			static MultiToggle ToggleColorOverlayButton = null;
			static ToolTip ToggleColorOverlayButtonTooltip = null;
			static Image image = null;
			public static void ToggleNoteVisibility()
			{
				BlueprintState.ToggleNoteVisibility();
				UpdateToggleState();
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
			}
			public static void UpdateToggleState()
			{
				ToggleColorOverlayButton.ChangeState(BlueprintState.NoteVisibility? 2 : 1);
			}

			public static void Postfix(TopLeftControlScreen __instance)
			{

				var debugTimeButton = Util.KInstantiateUI(__instance.kleiItemDropButton.gameObject, __instance.sandboxToggle.transform.parent.gameObject, true).transform;
				debugTimeButton.name = "toggleNoteVisibility";
				//UIUtils.ListAllChildrenWithComponents(debugButton);
				debugTimeButton.transform.FindComponent<Image>()?.sprite = ModAssets.NoteToolIcon_Sprite;
				debugTimeButton.Find("FG").TryGetComponent<Image>(out image);
				image.sprite = ModAssets.NoteToolIcon_Sprite;
				image.overrideSprite = ModAssets.NoteToolIcon_Sprite;
				debugTimeButton.TryGetComponent<MultiToggle>(out ToggleColorOverlayButton);
				var buttonColor = UIUtils.rgb(31, 161, 255);
				ToggleColorOverlayButton.states[2].color = buttonColor;
				ToggleColorOverlayButton.states[2].color_on_hover = UIUtils.Lighten(buttonColor, 20);
				debugTimeButton.TryGetComponent<ToolTip>(out ToggleColorOverlayButtonTooltip);
				debugTimeButton.SetSiblingIndex(__instance.kleiItemDropButton.transform.GetSiblingIndex());
				ToggleColorOverlayButton.onClick = (System.Action)Delegate.Combine(ToggleColorOverlayButton.onClick, new System.Action(ToggleNoteVisibility));
				ToggleColorOverlayButtonTooltip.SetSimpleTooltip(STRINGS.UI.ACTIONS.TOGGLENOTEVIS + " "+GameUtil.GetHotkeyString(ModAssets.Actions.BlueprintsToggleNoteVisibility.GetKAction()));
				UpdateToggleState();
			}
		}
	}
}

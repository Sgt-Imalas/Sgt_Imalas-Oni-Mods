using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace DebugButton
{
	internal class Patches
	{

		static DebugHandler _debugHandler;
		[HarmonyPatch(typeof(TopLeftControlScreen))]
		[HarmonyPatch(nameof(TopLeftControlScreen.OnActivate))]
		public static class Add_Debug_Button
		{
			static MultiToggle DebugInstaBuildButton = null;
			static MultiToggle DebugSuperSpeeButton = null;
			static MultiToggle DebugToggleButton = null;
			static ToolTip DebugInstaBuildButtonTooltip = null;
			static ToolTip DebugSuperSpeedButtonTooltip = null;

			public static void UpdateDebugToggleState()
			{
				if (DebugSuperSpeeButton != null)
				{
					if (!DebugHandler.enabled)
					{
						DebugSuperSpeeButton.ChangeState(0);
						DebugSuperSpeedButtonTooltip.SetSimpleTooltip(STRINGS.UI.TOOLS.TOOLTIP_DEBUG_LOCKED);
					}
					else
					{
						DebugSuperSpeedButtonTooltip.SetSimpleTooltip(GameUtil.ReplaceHotkeyString(STRINGS.UI.TOOLS.DEBUG_SUPERSPEED_TOGGLE.TOOLTIP_TOGGLE, Action.DebugUltraTestMode));

						if (Time.timeScale > 10f)
						{
							DebugSuperSpeeButton.ChangeState(2);
						}
						else
						{
							DebugSuperSpeeButton.ChangeState(1);
							if (_debugHandler != null && _debugHandler.ultraTestMode)
								_debugHandler.ultraTestMode = false;
						}
					}
				}
				if (DebugInstaBuildButton != null)
				{
					if (!DebugHandler.enabled)
					{
						DebugInstaBuildButton.ChangeState(0);
						DebugInstaBuildButtonTooltip.SetSimpleTooltip(STRINGS.UI.TOOLS.TOOLTIP_DEBUG_LOCKED);
					}
					else
					{
						DebugInstaBuildButtonTooltip.SetSimpleTooltip(GameUtil.ReplaceHotkeyString(STRINGS.UI.TOOLS.DEBUG_INSTABUILD_TOGGLE.TOOLTIP_TOGGLE, Action.DebugInstantBuildMode));

						if (DebugHandler.InstantBuildMode)
						{
							DebugInstaBuildButton.ChangeState(2);
						}
						else
						{
							DebugInstaBuildButton.ChangeState(1);
						}
					}
				}

				if (DebugToggleButton != null)
				{
					if (DebugHandler.enabled)
					{
						DebugToggleButton.ChangeState(2);
					}
					else
					{
						DebugToggleButton.ChangeState(1);
					}
				}
			}
			public static void OnClickSuperSpeedToggle()
			{
				if (!DebugHandler.enabled)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
				}
				else if (_debugHandler != null)
				{
					SetSuperSpeed(!_debugHandler.ultraTestMode);
				}
				UpdateDebugToggleState();
			}

			static void SetSuperSpeed(bool isOn)
			{
				if (_debugHandler != null)
				{
					if (isOn)
					{
						Time.timeScale = 30f;
					}
					else if(!isOn && SpeedControlScreen.Instance != null)
					{
						SpeedControlScreen.Instance.OnChanged();
					}
					else
					{
						Time.timeScale = 1f;
					}
					_debugHandler.ultraTestMode = isOn;
				}
			}
			static void ToggleInstaBuild(bool forceOff = false)
			{
				if (forceOff && !DebugHandler.InstantBuildMode)
					return;

				_debugHandler.OnKeyDown(new KButtonEvent(null, InputEventType.KeyUp, Action.DebugInstantBuildMode));
			}

			public static void OnClickDebugToggle()
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));

				if (DebugHandler.enabled)
				{
					if (!SpeedControlScreen.Instance.IsPaused)
						SetSuperSpeed(false);
					ToggleInstaBuild(true);
				}
				DebugHandler.SetDebugEnabled(!DebugHandler.enabled);

				UpdateDebugToggleState();
			}

			public static void OnClickInstaBuildToggle()
			{
				if (!DebugHandler.enabled)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
				}
				else
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));

					ToggleInstaBuild();
				}
				UpdateDebugToggleState();
			}

			public static void Postfix(TopLeftControlScreen __instance)
			{

				var debugTimeButton = Util.KInstantiateUI(__instance.sandboxToggle.gameObject, __instance.sandboxToggle.transform.parent.gameObject, true).transform;
				//UIUtils.ListAllChildrenWithComponents(debugButton);
				debugTimeButton.SetSiblingIndex(__instance.sandboxToggle.transform.GetSiblingIndex() + 1);
				debugTimeButton.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("icon_schedule");
				debugTimeButton.Find("Label").GetComponent<LocText>().text = STRINGS.UI.TOOLS.DEBUG_SUPERSPEED_TOGGLE.NAME;
				debugTimeButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 120f);
				debugTimeButton.TryGetComponent<MultiToggle>(out DebugSuperSpeeButton);
				debugTimeButton.TryGetComponent<ToolTip>(out DebugSuperSpeedButtonTooltip);
				DebugSuperSpeeButton.onClick = (System.Action)Delegate.Combine(DebugSuperSpeeButton.onClick, new System.Action(OnClickSuperSpeedToggle));


				var debugBuildButton = Util.KInstantiateUI(__instance.sandboxToggle.gameObject, __instance.sandboxToggle.transform.parent.gameObject, true).transform;
				//UIUtils.ListAllChildrenWithComponents(debugButton);
				debugBuildButton.SetSiblingIndex(__instance.sandboxToggle.transform.GetSiblingIndex() + 1);
				debugBuildButton.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("icon_archetype_build");
				debugBuildButton.Find("Label").GetComponent<LocText>().text = STRINGS.UI.TOOLS.DEBUG_INSTABUILD_TOGGLE.NAME;
				debugBuildButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 120f);
				debugBuildButton.TryGetComponent<MultiToggle>(out DebugInstaBuildButton);
				debugBuildButton.TryGetComponent<ToolTip>(out DebugInstaBuildButtonTooltip);
				DebugInstaBuildButton.onClick = (System.Action)Delegate.Combine(DebugInstaBuildButton.onClick, new System.Action(OnClickInstaBuildToggle));


				var debugButton = Util.KInstantiateUI(__instance.sandboxToggle.gameObject, __instance.sandboxToggle.transform.parent.gameObject, true).transform;
				//UIUtils.ListAllChildrenWithComponents(debugButton);
				debugButton.SetSiblingIndex(__instance.sandboxToggle.transform.GetSiblingIndex() + 1);
				debugButton.Find("FG").GetComponent<Image>().sprite = Def.GetUISprite(Assets.GetPrefab("LightBug")).first;
				debugButton.Find("Label").GetComponent<LocText>().text = STRINGS.UI.TOOLS.DEBUG_TOGGLE.NAME;
				debugButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 120f);
				debugButton.TryGetComponent<MultiToggle>(out DebugToggleButton);
				debugButton.TryGetComponent<ToolTip>(out var tt);
				tt.SetSimpleTooltip(STRINGS.UI.TOOLS.DEBUG_TOGGLE.TOOLTIP_TOGGLE);
				DebugToggleButton.onClick = (System.Action)Delegate.Combine(DebugToggleButton.onClick, new System.Action(OnClickDebugToggle));


				Game.Instance.Subscribe((int)GameHashes.DebugInsantBuildModeChanged, delegate
				{
					UpdateDebugToggleState();
				});
				UpdateDebugToggleState();
			}
		}

		[HarmonyPatch(typeof(TopLeftControlScreen))]
		[HarmonyPatch(nameof(TopLeftControlScreen.OnForcedCleanUp))]
		public static class RemoveListenerForDebugButton
		{
			public static void Postfix(TopLeftControlScreen __instance)
			{
				KInputManager.InputChange.RemoveListener(Add_Debug_Button.UpdateDebugToggleState);
			}
		}
		[HarmonyPatch(typeof(InputInit), "Awake")]
		public static class InputInit_Awake_Patch
		{
			public static void Postfix()
			{
				GameInputManager inputManager = Global.GetInputManager();
				if (inputManager == null)
					return;

				var debughandler = inputManager.usedMenus.Find(item => item is DebugHandler) as DebugHandler;
				if (debughandler != null)
				{
					_debugHandler = debughandler;
				}
			}
		}
		[HarmonyPatch(typeof(SpeedControlScreen), nameof(SpeedControlScreen.OnChanged))]
		public static class Refresh_Speed_Button
		{
			public static void Postfix()
			{
				Add_Debug_Button.UpdateDebugToggleState();
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}

		//public static string bugIconName = "no_bugs_instabuild_icon";

	}
}

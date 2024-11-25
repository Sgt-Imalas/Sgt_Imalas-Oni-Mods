
using AkisSnowThings.Content.Scripts.Buildings;
using HarmonyLib;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace AkisSnowThings.Patches
{
	public class SelectToolHoverTextCardPatch
	{
		public static HoverTextConfiguration.TextStylePair Styles_BodyText;
		public static Sprite iconDash;

		[HarmonyPatch(typeof(SelectToolHoverTextCard), "ConfigureHoverScreen")]
		public class SelectToolHoverTextCard_ConfigureHoverScreen_Patch
		{
			public static void Postfix(SelectToolHoverTextCard __instance)
			{
				Styles_BodyText = __instance.Styles_BodyText;
				iconDash = __instance.iconDash;
			}
		}

		[HarmonyPatch(typeof(SelectToolHoverTextCard), "UpdateHoverElements")]
		public class SelectToolHoverTextCard_UpdateHoverElements_Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				var m_GetDecorAtCell = AccessTools.Method(typeof(GameUtil), "GetDecorAtCell");
				var index = codes.FindIndex(c => c.Calls(m_GetDecorAtCell));

				if (index == -1)
				{
					return codes;
				}

				var m_EndShadowBar = AccessTools.Method(typeof(HoverTextDrawer), "EndShadowBar");
				var index2 = codes.FindIndex(index, c => c.Calls(m_EndShadowBar));

				if (index2 == -1)
				{
					return codes;
				}

				var m_AttachSnowInfo = AccessTools.Method(
					typeof(SelectToolHoverTextCard_UpdateHoverElements_Patch),
					"AttachSnowInfo",
					new[] { typeof(HoverTextDrawer), typeof(int) });

				codes.InsertRange(index2, new[]
				{
					new CodeInstruction(OpCodes.Dup), // hovertextdrawer
                    new CodeInstruction(OpCodes.Ldloc_0), // cell
                    new CodeInstruction(OpCodes.Call, m_AttachSnowInfo)
				});

				//Log.PrintInstructions(codes);

				return codes;
			}

			private static void AttachSnowInfo(HoverTextDrawer hoverTextDrawer, int cell)
			{
				if (SnowMachine.AllSnowInfos.TryGetValue(cell, out var infos))
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawText(STRINGS.UI.DECOROVERLAYTITLE, Styles_BodyText.Standard);

					foreach (var info in infos)
					{
						hoverTextDrawer.NewLine(18);
						hoverTextDrawer.DrawIcon(iconDash, 18);

						var decor = GameUtil.GetFormattedDecor(info.GetDecorForCell(cell), false);
						var text = string.Format(UI.OVERLAYS.DECOR.ENTRY, decor, info.GetProperName(), "");

						hoverTextDrawer.DrawText(text, Styles_BodyText.Standard);
					}
				}
			}

			private static float GetDecorForCell(float originalValue, DecorProvider provider, int cell)
			{
				// Log.Debuglog("GetDecorForCell", originalValue)
				if (provider.HasTag(ModAssets.customHighlightTag) && provider.TryGetComponent(out SnowMachine snowMachine))
				{
					snowMachine.GetDecorForCell(cell);
				}

				return originalValue;
			}
		}
	}
}

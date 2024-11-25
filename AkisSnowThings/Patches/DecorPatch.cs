using AkisSnowThings.Content.Scripts.Buildings;
using HarmonyLib;
using UnityEngine;

namespace AkisSnowThings.Patches
{
	public class DecorPatch
	{

		[HarmonyPatch(typeof(OverlayModes.Decor), MethodType.Constructor)]
		public class OverlayModes_Decor_Ctor_Patch
		{
			public static void Postfix(ref OverlayModes.ColorHighlightCondition[] ___highlightConditions)
			{
				var condition = new OverlayModes.ColorHighlightCondition(dp =>
				{
					return GlobalAssets.Instance.colorSet.decorHighlightPositive;
				},
				dp =>
				{
					if (!dp.HasTag(ModAssets.customHighlightTag))
					{
						return false;
					}

					int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));

					return
						SnowMachine.AllSnowInfos.TryGetValue(cell, out var info) &&
						dp.TryGetComponent(out SnowMachine snowMachine) &&
						info.Contains(snowMachine);
				});

				___highlightConditions = ___highlightConditions.AddToArray(condition);
			}
		}
	}
}

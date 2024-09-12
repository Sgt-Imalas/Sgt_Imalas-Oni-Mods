using HarmonyLib;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
	internal class FrameModificationPatches
	{
		[HarmonyPatch(typeof(CanvasConfig), nameof(CanvasConfig.DoPostConfigureComplete))]
		public static class ManualGenerator_AnimFix
		{
			public static void Postfix(GameObject go)
			{
				//go.AddOrGet<EditableFrame>();

			}
		}
	}
}

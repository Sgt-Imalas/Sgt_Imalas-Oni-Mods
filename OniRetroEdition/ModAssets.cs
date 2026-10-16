using UnityEngine;
using UtilLibs;

namespace OniRetroEdition
{
	internal class ModAssets
	{
		public static readonly Color RetroYellow = new Color(1, 167f / 255f, 45f / 255f) * 3f;
		public static readonly Color RetroYellowOverlay = new Color(1, 196f / 255f, 44f / 255f) * 1.5f;

		public static void ApplyLegacyLighting()
		{
			if (Lighting.Instance == null || !Config.Instance.oldShading)
				return;
			var settings = Lighting.Instance.Settings;

			settings.DarkenTints[0] = new Color32(125, 125, 125, byte.MaxValue);
			settings.DarkenTints[1] = new Color32(200, 200, 200, byte.MaxValue);
			settings.DarkenTints[2] = new Color32(207, 159, 85, byte.MaxValue);

			settings.characterLighting.litColour = new Color32(255, 245, 160, byte.MaxValue);
			settings.characterLighting.unlitColour = new Color32(200, 200, 200, byte.MaxValue);


			settings.ShowDust = 0.5f;

			var edgeParams = settings.substanceEdgeParameters;
			edgeParams.intensity = 0.17f;
			settings.substanceEdgeParameters = edgeParams;
			SgtLogger.l("Legacy Lighting applied");
		}

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.UI
{
	internal class RonivanAIO_ToxicityOverlayMode : OverlayModes.Mode
	{
		public static readonly HashedString ID = "RonivanAIO_ToxicityOverlay";
		public override string GetSoundName() => "Decor";
		static Color toxic = UIUtils.rgb(238, 255, 0);

		public static Color GetColorForCell(SimDebugView _, int cell)
		{
			var t = ToxicityGrid.GetToxicityMaxPercentage(cell);
			return Color.Lerp(Color.black, toxic, t);
		}

		public override HashedString ViewMode() => ID;
	}
}

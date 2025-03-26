using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks
{
    internal class ModAssets
	{
		static Dictionary<SimHashes, Color> CachedColors = new Dictionary<SimHashes, Color>();

		public static Color GetElementColor(SimHashes simhash)
		{
			if (!CachedColors.TryGetValue(simhash, out var color))
			{
				var element = ElementLoader.GetElement(simhash.CreateTag());
				color = element.substance.conduitColour;
				CachedColors[simhash] = color;
			}
			return color;

		}

	}
}

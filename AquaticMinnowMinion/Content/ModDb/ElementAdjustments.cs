using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class ElementAdjustments
	{
		internal static void DoModifications()
		{
			MakeLiquidBreathable(SimHashes.Water);
			MakeLiquidBreathable(SimHashes.DirtyWater);
			MakeLiquidBreathable(SimHashes.Brine);
			MakeLiquidBreathable(SimHashes.SaltWater);
			MakeLiquidBreathable(SimHashes.MurkyBrine);
			//yucky lungs:
			AddElementTag(SimHashes.DirtyWater, ModTags.PollutedLiquid);
			AddElementTag(SimHashes.MurkyBrine, ModTags.PollutedLiquid);
		}
		static void MakeLiquidBreathable(SimHashes hashes)
		{
			AddElementTag(hashes, ModTags.BreathableLiquid);

		}
		static void AddElementTag(SimHashes hashes, Tag tag)
		{
			var element = ElementLoader.FindElementByHash(hashes);
			if (element == null)
			{
				SgtLogger.warning(hashes + " was not found");
				return;
			}
			element.oreTags ??= [];
			if (element.oreTags.Contains(tag))
				return;
			element.oreTags = element.oreTags.Append(tag);

		}
	}
}

using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace AquaticMinnowMinion
{
	internal class ModAssets
	{
		public static class ModTags
		{
			public static Tag AquaticMinion = TagManager.Create("AquaticMinion", STRINGS.DUPLICANTS.MODEL.AQUATIC.NAME);
			public static Tag BreathableLiquid = TagManager.Create("AM_OxygenizedLiquid", STRINGS.MISC.TAGS.AM_OXYGENIZEDLIQUID);
			public static Tag PollutedLiquid = TagManager.Create("AM_PollutedLiquid", STRINGS.MISC.TAGS.AM_POLLUTEDLIQUID);
		}
		public static ModHashes PoorBreathableLiquidQuality = new("Aq_"+ nameof(PoorBreathableLiquidQuality));
	}
}

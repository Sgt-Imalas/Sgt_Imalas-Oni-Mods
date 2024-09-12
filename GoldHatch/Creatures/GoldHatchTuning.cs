using System.Collections.Generic;

namespace GoldHatch.Creatures
{
	internal class GoldHatchTuning
	{


		public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_GOLD = new List<FertilityMonitor.BreedingChance>()
		{
			new FertilityMonitor.BreedingChance()
			{
				egg = HatchConfig.EGG_ID.ToTag(),
				weight = 0.11f
			},
			new FertilityMonitor.BreedingChance()
			{
				egg = HatchHardConfig.EGG_ID.ToTag(),
				weight = 0.22f
			},
			new FertilityMonitor.BreedingChance()
			{
				egg = HatchMetalConfig.EGG_ID.ToTag(),
				weight = 0.22f
			},
			new FertilityMonitor.BreedingChance()
			{
				egg = HatchGoldConfig.EGG_ID.ToTag(),
				weight = 0.67f
	}
		};
	}
}

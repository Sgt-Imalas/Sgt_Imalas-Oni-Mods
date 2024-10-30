using SatisfyingPowerShards.Defs.Critters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatisfyingPowerShards.Data
{
	internal class PowerShardStaterpillarTuning
	{
		public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_YELLOW = new List<FertilityMonitor.BreedingChance>
		{
			new FertilityMonitor.BreedingChance
			{
				egg = StaterpillarConfig.EGG_ID.ToTag(),
				weight = 0.60f
			},
			new FertilityMonitor.BreedingChance
			{
				egg = StaterpillarYellowConfig.EGG_ID.ToTag(),
				weight = 0.35f
			},new FertilityMonitor.BreedingChance
			{
				egg = StaterpillarPurpleConfig.EGG_ID.ToTag(),
				weight = 0.5f
			}
		};

		public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_PURPLE = new List<FertilityMonitor.BreedingChance>
		{
			new FertilityMonitor.BreedingChance
			{
				egg = StaterpillarConfig.EGG_ID.ToTag(),
				weight = 0.40f
			},
			new FertilityMonitor.BreedingChance
			{
				egg = StaterpillarYellowConfig.EGG_ID.ToTag(),
				weight = 0.45f
			},new FertilityMonitor.BreedingChance
			{
				egg = StaterpillarPurpleConfig.EGG_ID.ToTag(),
				weight = 0.15f
			}
		};
	}
}

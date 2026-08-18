using System;
using System.Collections.Generic;
using System.Text;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_TUNING
	{
		public const float OC_Wattage = 10;
		public const float OC_Stressdelta = 15f;

		public const float DreamBooster_Wattage = 60;

		public const float Batteryslot_Stressdelta = 5f;

		public const float Waterproofed_ExtraOilConsumption = -5f;

		public const float Medkit_RadsRemovedPerSecond = -480f / 600f; // -480 rads per cycle
		public const float Medkit_HealthRegeneratedPerSecond = 100f / 200f; //100hp in 200s

		public const float Medkit_RadsThreshold_Upper = 25;
		public const float Medkit_RadsThreshold_Lower = 5;
		public const float Medkit_Wattage = 50;

		public const float SolarBooster_LuxThreshold = 2000;
	}
}

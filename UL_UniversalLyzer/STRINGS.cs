
namespace UL_UniversalLyzer
{
	internal class STRINGS
	{
		class UL_CONFIGSTRINGS
		{
			class CATEGORIES
			{
				public static LocString A_GENERAL = "(1) General Settings";
				public static LocString B_WATER = "(2) Default Settings / Water";
				public static LocString C_POLLUTEDWATER = "(3) Polluted Water";
				public static LocString D_SALTWATER = "(4) Salt Water";
				public static LocString E_BRINE = "(5) Brine";
				public static LocString F_MURKYBRINE = "(6) Polluted Brine";
			}

			class IS_PIPED
			{
				public static LocString NAME = "Piped Output Compatibility - Use piped outputs";
				public static LocString DESC = "Does nothing on its own.\nWhen the mod \"Piped Output\" or \"Piped Everything\" is installed,\nthe Electrolyzer will ignore that mod unless this option here is checked.\nIf checked and piped output is enabled, the electrolyzer will gain two additional piped outputs,\nchlorine and polluted oxygen";
			}

			class SOLID_DEBRIS
			{
				public static LocString NAME = "Solid Debris";
				public static LocString DESC = "Using polluted water, salt water or brine will cause a tiny amount of debris to drop. Deactivate to have that mass added to the gasses instead.";
			}

			class PERLIQUIDSETTINGS
			{
				public static LocString NAME = "Water Type used affects Electrolyzer";
				public static LocString DESC = "When active, the power consumption, output temperature and overpressurisation threshold are affected by the water type.";
			}

			class CONSUMPTION
			{
				public static LocString NAME = "Power Consumption";
				public static LocString DESC_WATER = "Power Consumption of the electrolyzer when it uses Water.\nWhen \"Liquid Conductivity\" is disabled, this option defines the general electrolyzer power consumption.";
				public static LocString DESC_POLLUTEDWATER = "Power Consumption of the electrolyzer when it uses Polluted Water.";
				public static LocString DESC_SALTWATER = "Power Consumption of the electrolyzer when it uses Salt Water.";
				public static LocString DESC_BRINE = "Power Consumption of the electrolyzer when it uses Brine.";
				public static LocString DESC_MURKYBRINE = "Power Consumption of the electrolyzer when it uses Brine.";
			}

			class MINOUTPUTTEMP
			{
				public static LocString NAME = "Minimum Gas Output Temperature";
				public static LocString DESC = "Minimum Temperature of the Output Gasses in °C ";
			}

			class PRESSURETHRESHOLDMASS
			{
				public static LocString NAME = "Overpressurisation Threshold";
				public static LocString DESC_WATER = "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Water.";
				public static LocString DESC_POLLUTEDWATER = "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Polluted Water.";
				public static LocString DESC_SALTWATER = "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Salt Water.";
				public static LocString DESC_BRINE = "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Brine.";
				public static LocString DESC_MURKYBRINE = "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Brine.";
			}
		}
	}
}

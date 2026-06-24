using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_SkillPerks
	{
		//public static SkillPerk Weeb1;
		//public static SkillPerk Weeb2;

		//public const string WeebPerk1ID = "WD_WeebPerk1";
		//public const string WeebPerk2ID = "WD_WeebPerk2";
		public static SkillPerk
			Adapt_WaterbreathingEfficiency

			, Adapt_ItchyGillsImmunity
			, Adapt_SuitAirImmunity
			, Adapt_GillMoisturizing

			, Adapt_EyeProtectionMinor
			, Adapt_EyeProtectionMajor

			, Adapt_FatLayer
			, Adapt_ColdImmunity
			, Adapt_HeatImmunity
			;
		public static void Register(SkillPerks __instance)
		{
			Adapt_WaterbreathingEfficiency = __instance.Add(new SimpleSkillPerk(nameof(Adapt_WaterbreathingEfficiency), STRINGS.UI.ROLES_SCREEN.PERKS.ADAPT_WATERBREATHINGEFFICIENCY.DESCRIPTION));

			Adapt_ItchyGillsImmunity = __instance.Add(new ImmunitySkillPerk(nameof(Adapt_ItchyGillsImmunity), Aq_Effects.ItchyGills.Id));
			Adapt_SuitAirImmunity = __instance.Add(new ImmunitySkillPerk(nameof(Adapt_SuitAirImmunity), Aq_Effects.DrySuitAir.Id));
			Adapt_GillMoisturizing = __instance.Add(new SkillAttributePerk(nameof(Adapt_GillMoisturizing), Aq_Amounts.Aquatic_GillMoisture.deltaAttribute.Id, AQ_TUNING.ADAPTATION_PERKS.ADAPTATION_MOIST_GILLS, STRINGS.DUPLICANTS.ROLES.ADAPTATION_GILLPROTECTION.NAME));

			Adapt_EyeProtectionMinor = __instance.Add(new ImmunitySkillPerk(nameof(Adapt_EyeProtectionMinor), "MinorIrritation"));
			Adapt_EyeProtectionMajor = __instance.Add(new ImmunitySkillPerk(nameof(Adapt_EyeProtectionMajor), "MajorIrritation"));

			Adapt_FatLayer = __instance.Add(new SkillAttributePerk(nameof(Adapt_FatLayer), "ThermalConductivityBarrier", AQ_TUNING.ADAPTATION_PERKS.ADAPTATION_FAT_INSULATION, global::STRINGS.DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.DESC));
			Adapt_ColdImmunity = __instance.Add(new ImmunitySkillPerk(nameof(Adapt_ColdImmunity), "ColdAir"));
			Adapt_HeatImmunity = __instance.Add(new ImmunitySkillPerk(nameof(Adapt_HeatImmunity), "WarmAir"));
		}

	}
}

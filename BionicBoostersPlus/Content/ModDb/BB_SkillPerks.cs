using Database;
using STRINGS;
using UtilLibs;
using static STRINGS.UI.ROLES_SCREEN.PERKS;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_SkillPerks
	{
		public static SkillPerk
			BB_BionicDream,
			BB_BionicCarryCapacityOC,
			BB_ReducedWaterStress,
			BB_CanCraftOverclocks,

			BB_Plating_RadResistance,
			BB_Plating_Health,

			BB_Circuits_StressRelief,
			BB_Circuits_WattageReduction

			;
		public static float PLATING_RADIATION_RESISTANCE = 0.50f;
		public static float PLATING_EXTRAHP = 25f;

		public static float CIRCUITS_STRESSREDUCTION = -20f / CONSTS.CYCLE_LENGTH;
		public static float CIRCUITS_WATTAGEREDUCTIONPERCENTAGE = 0.25f;


		public static string WATTAGE_UI_TOOLTIP;

		public static void Register(SkillPerks __instance)
		{
			var db = Db.Get();

			var stressDelta = db.Amounts.Stress.deltaAttribute.Id;

			BB_CanCraftOverclocks = __instance.Add(new SimpleSkillPerk(nameof(BB_CanCraftOverclocks), string.Format(CAN_USE_BUILDING.DESCRIPTION, STRINGS.BUILDINGS.PREFABS.BBP_MK3BOOSTERMAKER.NAME)));
			BB_BionicDream = __instance.Add(new SimpleSkillPerk(nameof(BB_BionicDream), STRINGS.UI.ROLES_SCREEN.PERKS.BB_BIONICDREAM.DESCRIPTION));
			BB_BionicCarryCapacityOC = __instance.Add(new SkillAttributePerk(nameof(BB_BionicCarryCapacityOC), Db.Get().Attributes.CarryAmount.Id, 500f, BB_Boosters.GetOCName(BionicUpgradeComponentConfig.Booster_Carry1), true));
			BB_ReducedWaterStress = __instance.Add(new SimpleSkillPerk(nameof(BB_ReducedWaterStress), STRINGS.UI.ROLES_SCREEN.PERKS.BB_REDUCEDWATERSTRESS.DESCRIPTION));

			BB_Plating_RadResistance = __instance.Add(new SkillAttributePerk(nameof(BB_Plating_RadResistance), Db.Get().Attributes.RadiationResistance.Id, PLATING_RADIATION_RESISTANCE, STRINGS.DUPLICANTS.ROLES.BIONICS_D3_RADS.NAME));
			var plating_health = new SkillAttributePerk(nameof(BB_Plating_Health), Db.Get().Amounts.HitPoints.maxAttribute.Id, PLATING_EXTRAHP, STRINGS.DUPLICANTS.ROLES.BIONICS_D3_RADS.NAME);
			plating_health.Name = string.Format(UI.ROLES_SCREEN.PERKS.ATTRIBUTE_EFFECT_FMT, GameUtil.AddPositiveSign(PLATING_EXTRAHP.ToString(), true), DUPLICANTS.STATS.HITPOINTS.NAME);
			BB_Plating_Health = __instance.Add(plating_health);


			BB_Circuits_StressRelief = __instance.Add(new SkillAttributePerk(nameof(BB_Circuits_StressRelief), stressDelta, CIRCUITS_STRESSREDUCTION, STRINGS.DUPLICANTS.ROLES.BIONICS_C3_WATTS.NAME));
			BB_Circuits_WattageReduction = __instance.Add(new SimpleSkillPerk(nameof(BB_Circuits_WattageReduction), string.Format(UI.ROLES_SCREEN.PERKS.ATTRIBUTE_EFFECT_FMT, GameUtil.GetFormattedPercent(-CIRCUITS_WATTAGEREDUCTIONPERCENTAGE * 100f), STRINGS.UI.ROLES_SCREEN.PERKS.BB_WATTAGEREDUCTION.DESCRIPTION)));

			WATTAGE_UI_TOOLTIP = STRINGS.DUPLICANTS.ROLES.BIONICS_C3_WATTS.NAME + ": " + UIUtils.StripAllFormatting(BB_Circuits_WattageReduction.Name) + " ({0}" + UI.UNITSUFFIXES.ELECTRICAL.WATT + ")";
		}
	}
}

using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.DUPLICANTS.STATS;

namespace Planticants.Content.ModDb
{
    class PlantAmounts
	{
		public static Amount Plant_Glucose;
		public static Amount Plant_CO2_Tank;
		public static Amount Plant_Water;
		public static Amount Plant_LightLevelAverage;

		internal static void RegisterAmounts(Database.Amounts instance)
		{
			SgtLogger.l("Registering PlantAmounts...");

			Plant_Glucose = instance.CreateAmount(nameof(Plant_Glucose), 0f, 100f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_stamina");
			Plant_Glucose.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));

			Plant_CO2_Tank = instance.CreateAmount(nameof(Plant_CO2_Tank), 0f, PLANT_TUNING.CO2_TANK_CAPACITY, show_max: true, Units.Flat, 5f, show_in_ui: true, "STRINGS.DUPLICANTS", "ui_icon_breath", null, "mod_breath");
			Plant_CO2_Tank.SetDisplayer(new BionicOxygenTankDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));

			Plant_Water = instance.CreateAmount(nameof(Plant_Water), 0f, PLANT_TUNING.WATER_TANK_CAPACITY, show_max: true, Units.Flat, 5f, show_in_ui: true, "STRINGS.DUPLICANTS", "ui_icon_liquid");
			Plant_Water.SetDisplayer(new BionicOxygenTankDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));

			Plant_LightLevelAverage = instance.CreateAmount(nameof(Plant_LightLevelAverage), 0f, 100f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_battery");
			Plant_LightLevelAverage.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle));

			instance.Add(Plant_Glucose);
			instance.Add(Plant_CO2_Tank);
			instance.Add(Plant_Water);
			instance.Add(Plant_LightLevelAverage);

		}
		public static List<Amount> GetAmounts()
		{
			return
			[
				Plant_LightLevelAverage,
				Plant_Water,
				Plant_Glucose,
				Plant_CO2_Tank
			];
		}
		public static List<string> GetAmountIDs()
		{
			return
			[
				nameof(Plant_LightLevelAverage),
				nameof(Plant_Water),
				nameof(Plant_Glucose),
				nameof(Plant_CO2_Tank)
			];
		}
		public static void InitializeDefaultValues(GameObject go)
		{
			AmountInstance glucose = Plant_Glucose.Lookup(go);
			glucose.value = 100;

			AmountInstance co2Tank = Plant_CO2_Tank.Lookup(go);
			co2Tank.value = PLANT_TUNING.CO2_TANK_CAPACITY;

			AmountInstance waterTank = Plant_Water.Lookup(go);
			waterTank.value = PLANT_TUNING.WATER_TANK_CAPACITY;
		}
	}
}

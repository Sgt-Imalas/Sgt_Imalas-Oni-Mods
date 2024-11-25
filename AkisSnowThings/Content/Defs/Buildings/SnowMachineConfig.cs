using AkisSnowThings.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AkisSnowThings.Content.Defs.Buildings
{
	public class SnowMachineConfig : IBuildingConfig
	{
		public static string ID = "SnowSculptures_SnowMachine";

		public override BuildingDef CreateBuildingDef()
		{
			var def = BuildingTemplates.CreateBuildingDef(
			   ID,
			   1,
			   1,
			   "sm_snowmachine_kanim",
			   BUILDINGS.HITPOINTS.TIER2,
			   BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
			   BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
			   MATERIALS.RAW_METALS,
			   BUILDINGS.MELTING_POINT_KELVIN.TIER0,
			   BuildLocationRule.OnCeiling,
			   DECOR.NONE,
			   NOISE_POLLUTION.NONE
		   );

			def.AudioCategory = AUDIO.CATEGORY.GLASS;
			def.ViewMode = OverlayModes.Power.ID;
			def.PowerInputOffset = new CellOffset(0, 1);

			def.RequiresPowerInput = true;
			def.ExhaustKilowattsWhenActive = 0.2f;// Config.Instance.SnowMachinePower.ExhaustKilowattsWhenActive;
			def.EnergyConsumptionWhenActive = 2f;// Config.Instance.SnowMachinePower.EnergyConsumptionWhenActive;
			def.SelfHeatKilowattsWhenActive = 0f;// Config.Instance.SnowMachinePower.SelfHeatKilowattsWhenActive;

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			var snowMachine = go.AddComponent<SnowMachine>();
			snowMachine.speed = 0.5f;
			snowMachine.turbulence = 0.04f;
			snowMachine.lifeTime = 4f;
			snowMachine.density = 6;

			go.AddOrGet<CopyBuildingSettings>().copyGroupTag = ID;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGetDef<PoweredController.Def>();

			go.AddTag(ModAssets.customHighlightTag);
		}
	}
}

using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics
{
    class LogisticFilterConfig : IBuildingConfig, IHasConfigurableWattage
	{
		public static float Wattage = HighPressureConduitRegistration.GetLogisticConduitMultiplier() * 120f;

		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;
	
		public static string ID = "LogisticFilter";

		private const ConduitType CONDUIT_TYPE = ConduitType.Solid;
		private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(0, 0));

		
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER1;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "logistic_filter_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.EnergyConsumptionWhenActive = Wattage;
			def1.SelfHeatKilowattsWhenActive = 0f;
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.InputConduitType = ConduitType.Solid;
			def1.OutputConduitType = ConduitType.Solid;
			def1.Floodable = false;
			def1.Overheatable = false;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.AudioCategory = "Metal";
			def1.UtilityInputOffset = new CellOffset(-1, 0);
			def1.UtilityOutputOffset = new CellOffset(1, 0);
			def1.PermittedRotations = PermittedRotations.R360;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("logistic_filter_kanim", "filter_material_conveyor_kanim");
			return def1;
		}
		private void AttachPort(GameObject go)
		{
			go.AddComponent<ConduitSecondaryOutput>().portInfo = this.secondaryPort;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<Structure>();
			go.AddOrGet<ElementFilter>().portInfo = this.secondaryPort;
			go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Solid;
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}

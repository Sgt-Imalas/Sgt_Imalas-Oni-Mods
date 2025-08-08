using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HPA_Gas
{
    class HPAFilterGasConfig : IBuildingConfig
	{
		public const string ID = "HPA_MultiGasFilter";
		private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 0));

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [200f, 100f];
			string[] construction_materials = [GameTags.Steel.ToString(), GameTags.Plastic.ToString()];
			EffectorValues tieR0 = TUNING.BUILDINGS.DECOR.PENALTY.TIER0;
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "hpa_filter_gas_kanim", 30, 10f, construction_mass, construction_materials, 1600f, BuildLocationRule.Anywhere, tieR0, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 360f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.ExhaustKilowattsWhenActive = 0.0f;
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.Floodable = false;
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.FILTER);
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			SoundUtils.CopySoundsToAnim("hpa_filter_gas_kanim", "filter_gas_kanim");
			return buildingDef;
		}

		private void AttachPort(GameObject go) => go.AddComponent<ConduitSecondaryOutput>().portInfo = this.secondaryPort;

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

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

			var storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.showDescriptor = false;
			storage.storageFilters = STORAGEFILTERS.GASES;
			storage.capacityKg = 0;
			storage.showInUI = true;
			storage.allowSettingOnlyFetchMarkedItems = false;

			go.AddOrGet<TreeFilterable>();
			go.AddOrGet<MultiTagConduitFilter>().FilteredOutputPort = this.secondaryPort;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
		}
	}
}

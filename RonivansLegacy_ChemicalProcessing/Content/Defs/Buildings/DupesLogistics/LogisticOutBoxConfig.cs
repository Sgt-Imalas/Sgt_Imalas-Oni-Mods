using PeterHan.PLib.Options;
using ProcGen;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics
{
	public class LogisticOutBoxConfig : IBuildingConfig, IHasConfigurableStorageCapacity
	{
		public static float StorageCapacity = HighPressureConduit.GetLogisticConduitMultiplier() * 100f; // regular SolidConduitOutbox capacity/2
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;
		public static string ID = "LogisticOutBox";

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "logistic_outbox_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
			def1.Floodable = false;
			def1.Overheatable = false;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.AudioCategory = "Metal";
			def1.InputConduitType = ConduitType.Solid;
			def1.UtilityInputOffset = new CellOffset(0, 0);
			def1.PermittedRotations = PermittedRotations.R360;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			return def1;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			go.AddOrGet<SolidConduitOutbox>();
			go.AddOrGet<SolidConduitConsumer>();
			Storage storage1 = BuildingTemplates.CreateDefaultStorage(go, false);
			storage1.capacityKg = GetStorageCapacity();
			storage1.showInUI = true;
			storage1.allowItemRemoval = true;
			go.AddOrGet<SimpleVent>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Prioritizable.AddRef(go);
			go.AddOrGet<Automatable>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
		}
	}
}

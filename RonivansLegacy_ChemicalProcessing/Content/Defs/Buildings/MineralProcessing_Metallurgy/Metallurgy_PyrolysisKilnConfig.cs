using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//==== [ CHEMICAL: PYROLYSIS KILN CONFIG ] =====================================================================
	public class Metallurgy_PyrolysisKilnConfig : IBuildingConfig
	{
		public static string ID = "Metallurgy_PyrolysisKiln";

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Prioritizable.AddRef(go);

			///handles element converter changes I guess..
			Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
			electrolyzer.maxMass = 10f;
			electrolyzer.hasMeter = false;

			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = 330f;
			storage.showInUI = true;

			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.WoodLog.CreateTag(), 1f)];
			converter.outputElements = [new ElementConverter.OutputElement(0.33f, SimHashes.Carbon, 312.15f, false, true, 0f, 1f, 1f, 0xff, 0), 
				new ElementConverter.OutputElement(0.1f, SimHashes.CarbonDioxide, 370.15f, false, false, 0f, 1f, 1f, 0xff, 0)];

			///requests new wood for conversion
			ManualDeliveryKG ykg = go.AddOrGet<ManualDeliveryKG>();
			ykg.SetStorage(storage);
			ykg.RequestedItemTag = SimHashes.WoodLog.CreateTag();
			ykg.capacity = 500f;
			ykg.refillMass = 150f;
			ykg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			///drops coal in 20kg chunks
			var dropper = go.AddOrGet<ElementDropper>();
			dropper.emitMass = 20;
			dropper.emitTag = SimHashes.Carbon.CreateTag();
			dropper.emitOffset = new Vector3(0f, 1f);
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER3;
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "pyrolysis_kiln_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
			def.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			def.ExhaustKilowattsWhenActive = 16f;
			def.SelfHeatKilowattsWhenActive = 4f;
			def.Overheatable = false;
			def.AudioCategory = "HollowMetal";
			def.Breakable = true;
			SoundUtils.CopySoundsToAnim("pyrolysis_kiln_kanim", "kiln_kanim");
			return def;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}
	}
}

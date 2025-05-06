using ComplexFabricatorRibbonController.Scripts.Buildings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace ComplexFabricatorRibbonController.Defs.Buildings
{
	class ComplexFabricatorRecipeControlAttachmentConfig : IBuildingConfig
	{

		public static readonly string PORT_ID = "ComplexFabricatorRecipeControlAttachment_RibbonPort";

		public const string ID = "CFRC_ComplexFabricatorRecipeControlAttachment";
		public override BuildingDef CreateBuildingDef()
		{
			string[] Materials = [MATERIALS.REFINED_METAL];
			float[] MaterialCosts = [150f];
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
					ID,
					1,
					1,
					"critter_sensor_kanim",
					30,
					15f,
					MaterialCosts,
					Materials,
					1600f,
					BuildLocationRule.NotInTiles,
					noise: NOISE_POLLUTION.NONE,
					decor: BUILDINGS.DECOR.PENALTY.TIER0);

			buildingDef.SceneLayer = Grid.SceneLayer.TransferArm;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
			buildingDef.CanMove = false;
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.AUTOMATION);
			buildingDef.ViewMode = OverlayModes.Logic.ID;
			buildingDef.AlwaysOperational = true;
			buildingDef.LogicInputPorts = [LogicPorts.Port.RibbonInputPort(PORT_ID, new CellOffset(0, 0),
				STRINGS.BUILDINGS.PREFABS.CFRC_COMPLEXFABRICATORRECIPECONTROLATTACHMENT.LOGIC_PORTS_DESCRIPTION,
				STRINGS.BUILDINGS.PREFABS.CFRC_COMPLEXFABRICATORRECIPECONTROLATTACHMENT.LOGIC_PORTS_ACTIVE,
				STRINGS.BUILDINGS.PREFABS.CFRC_COMPLEXFABRICATORRECIPECONTROLATTACHMENT.LOGIC_PORTS_INACTIVE, true)];
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<ComplexFabricatorRecipeControlAttachment>();

		}
		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}

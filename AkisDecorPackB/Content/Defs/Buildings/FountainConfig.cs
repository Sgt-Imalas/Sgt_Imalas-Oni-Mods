using AkisDecorPackB.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AkisDecorPackB.Content.Defs.Buildings
{
	public class FountainConfig : IBuildingConfig
	{
		public static string ID = "DECORPACKB_Fountain";

		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef def = BuildingTemplates.CreateBuildingDef(
				ID,
				5,
				3,
				"decorpackb_fountain_default_kanim",
				BUILDINGS.HITPOINTS.TIER2,
				BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
				[800f],
				MATERIALS.RAW_MINERALS,
				BUILDINGS.MELTING_POINT_KELVIN.TIER1,
				BuildLocationRule.OnFloor,
				DECOR.BONUS.TIER5,
				NOISE_POLLUTION.NONE);

			def.Floodable = false;
			def.Overheatable = false;
			def.AudioCategory = "glass";
			def.BaseTimeUntilRepair = -1f;
			def.ViewMode = OverlayModes.Decor.ID;
			def.DefaultAnimState = "idle";
			def.PermittedRotations = PermittedRotations.FlipH;

			def.InputConduitType = ConduitType.Liquid;
			def.UtilityInputOffset = new CellOffset(1, 0);

			def.OutputConduitType = ConduitType.Liquid;
			def.UtilityInputOffset = new CellOffset(-1, 0);

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddTag(GameTags.Decoration);
			go.AddOrGet<BuildingComplete>().isArtable = true;

			var storageIn = go.AddComponent<Storage>();
			storageIn.capacityKg = 10f;
			storageIn.storageFilters = STORAGEFILTERS.LIQUIDS;
			storageIn.allowItemRemoval = false;

			var conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.capacityKG = 10f;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			conduitConsumer.storage = storageIn;
			conduitConsumer.forceAlwaysSatisfied = true;

			var storageOut = go.AddComponent<Storage>();
			storageOut.capacityKg = 10f;
			storageOut.storageFilters = STORAGEFILTERS.LIQUIDS;
			storageOut.allowItemRemoval = false;

			var conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.storage = storageIn;

			var fountain = go.AddOrGet<Fountain>();
			fountain.storageIn = storageIn;
			fountain.storageOut = storageOut;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddComponent<Sculpture>().defaultAnimName = "idle";
		}
	}
}

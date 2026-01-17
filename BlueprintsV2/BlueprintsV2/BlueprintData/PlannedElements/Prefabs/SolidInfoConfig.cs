using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.LiquidInfo
{
	internal class SolidInfoConfig : CommonPlacerConfig, IEntityConfig
	{
		public static string ID = "BlueprintsV2_Solid_Placer";
		static Material slurpPlacerMaterial;
		public GameObject CreatePrefab()
		{
			slurpPlacerMaterial = new Material(Assets.instance.mopPlacerAssets.material);
			slurpPlacerMaterial.mainTexture = ModAssets.Solid_Placer_Sprite.texture;

			GameObject prefab = this.CreatePrefab(ID, ID, slurpPlacerMaterial);
			prefab.AddTag(GameTags.NotConversationTopic);
			UnityEngine.Object.Destroy(prefab.GetComponent<Prioritizable>());
			prefab.AddOrGet<KSelectable>();
			prefab.AddOrGet<InfoDescription>();
			prefab.AddOrGet<ElementOnlyFilterable>().filterElementState = Filterable.ElementState.Solid;
			prefab.AddOrGet<ElementPlanInfo>();
			return prefab;
		}
		public string[] GetDlcIds() => null;
		public void OnPrefabInit(GameObject go)
		{
		}

		public void OnSpawn(GameObject go)
		{
			RocketModuleHexCellCollector
		}
	}
}

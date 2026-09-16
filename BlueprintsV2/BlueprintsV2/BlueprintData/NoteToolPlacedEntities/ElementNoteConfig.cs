using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LogicGateVisualizer;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements
{
	internal class ElementNoteConfig : CommonPlacerConfig, IEntityConfig
	{
		public static string ID = "BlueprintsV2_Element_Note";
		static Material slurpPlacerMaterial;
		public GameObject CreatePrefab()
		{
			//slurpPlacerMaterial = new Material(Assets.instance.mopPlacerAssets.material);
			//slurpPlacerMaterial.mainTexture = ModAssets.Gas_Placer_Sprite.texture;
			//GameObject prefab = this.CreatePrefab(ID, ID, slurpPlacerMaterial);
			GameObject prefab = EntityTemplates.CreateEntity(ID, ID, true);
			prefab.AddOrGet<SaveLoadRoot>();
			KBoxCollider2D kBoxCollider2D = prefab.AddOrGet<KBoxCollider2D>();
			kBoxCollider2D.offset = new Vector2(0f, 0.5f);
			kBoxCollider2D.size = new Vector2(1f, 1f);
			prefab.AddTag(GameTags.NotConversationTopic);
			prefab.AddOrGet<KSelectable>();
			prefab.AddOrGet<InfoDescription>();
			prefab.AddOrGet<ElementOnlyFilterable>();
			prefab.AddOrGet<ElementNote>();


			return prefab;
		}
		public string[] GetDlcIds() => null;
		public void OnPrefabInit(GameObject go)
		{
		}

		public void OnSpawn(GameObject go)
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using TemplateClasses;
using UnityEngine;
using UnityEngine.UI;

namespace BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers
{
	internal class ReplacementVisualizerMultiEntityConfig : IMultiEntityConfig
	{
		public const string
			TILE_ID = "BPV2_TileReplacer",
			UTILITY_ID = "BPV2_UtilityReplacer",
			BUILDING_ID = "BPV2_BuildingReplacer";


		public List<GameObject> CreatePrefabs()
		{
			return [
				CreateVisPrefab(BUILDING_ID,typeof(ReplacementVis)),
				CreateVisPrefab(UTILITY_ID,typeof(UtilityReplacementVis)),
				CreateVisPrefab(TILE_ID,typeof(TileReplacementVis))
				];
		}

		static GameObject CreateVisPrefab(string id, Type visType)
		{
			GameObject prefab = EntityTemplates.CreateEntity(id, id);
			prefab.layer = LayerMask.NameToLayer("PlaceWithDepth");
			prefab.AddOrGet<SaveLoadRoot>();
			KBoxCollider2D kBoxCollider2D = prefab.AddOrGet<KBoxCollider2D>();
			kBoxCollider2D.offset = new Vector2(0f, 0.5f);
			kBoxCollider2D.size = new Vector2(1f, 1f);
			if (visType != typeof(TileReplacementVis))
			{
				prefab.AddComponent<KBatchedAnimController>().AnimFiles = [Assets.GetAnim("balloon_anim_kanim")];
				prefab.AddOrGet<VisualizerRotatable>();
			}
			else
			{
				//prefab.AddOrGet<RectMask2D>();
				var renderer = prefab.AddOrGet<SpriteRenderer>();
				var mat = new Material(Shader.Find("TextMeshPro/Sprite"))
				{
					renderQueue = 4501
				};
				mat.SetInt("_ZWrite", 1);
				renderer.material = mat;
			}
			prefab.AddTag(GameTags.NotConversationTopic);
			UnityEngine.Object.Destroy(prefab.GetComponent<Prioritizable>());
			prefab.AddOrGet<KSelectable>();
			prefab.AddOrGet<InfoDescription>();
			prefab.AddComponent(visType);
			return prefab;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}

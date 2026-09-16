using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities
{
	internal class TextNoteConfig : CommonPlacerConfig, IEntityConfig
	{
		public static string ID = "BlueprintsV2_Text_Note";
		static Material slurpPlacerMaterial;
		public GameObject CreatePrefab()
		{
			//slurpPlacerMaterial = new Material(Assets.instance.mopPlacerAssets.material);
			//slurpPlacerMaterial.mainTexture = ModAssets.Note_Placer_Sprite.texture;
			//GameObject prefab = this.CreatePrefab(ID, ID, slurpPlacerMaterial);
			//prefab.AddTag(GameTags.NotConversationTopic);
			//UnityEngine.Object.Destroy(prefab.GetComponent<Prioritizable>());
			//prefab.AddOrGet<KSelectable>();
			//prefab.AddOrGet<InfoDescription>();
			//prefab.AddOrGet<TextNote>();
			//return prefab;

			GameObject prefab = EntityTemplates.CreateEntity(ID, ID, true);
			prefab.AddOrGet<SaveLoadRoot>();
			KBoxCollider2D kBoxCollider2D = prefab.AddOrGet<KBoxCollider2D>();
			kBoxCollider2D.offset = new Vector2(0f, 0.5f);
			kBoxCollider2D.size = new Vector2(1f, 1f);
			prefab.AddTag(GameTags.NotConversationTopic);
			prefab.AddOrGet<KSelectable>();
			prefab.AddOrGet<InfoDescription>();
			prefab.AddOrGet<ElementOnlyFilterable>();
			prefab.AddOrGet<TextNote>();
			prefab.AddOrGet<CopyBuildingSettings>();


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

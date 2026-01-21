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
			slurpPlacerMaterial = new Material(Assets.instance.mopPlacerAssets.material);
			slurpPlacerMaterial.mainTexture = ModAssets.Note_Placer_Sprite.texture;
			GameObject prefab = this.CreatePrefab(ID, ID, slurpPlacerMaterial);
			prefab.AddTag(GameTags.NotConversationTopic);
			UnityEngine.Object.Destroy(prefab.GetComponent<Prioritizable>());
			prefab.AddOrGet<KSelectable>();
			prefab.AddOrGet<InfoDescription>();
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

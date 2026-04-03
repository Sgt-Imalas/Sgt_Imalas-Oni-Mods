using MassMoveTo.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MassMoveTo.Content.Defs.Entities
{
	internal class MMT_MultiMovePickupablePlacerConfig : CommonPlacerConfig, IEntityConfig
	{
		public static string ID = "MMT_MultiMovePickupablePlacer";

		public GameObject CreatePrefab()
		{
			GameObject prefab = this.CreatePrefab(ID, global::STRINGS.MISC.PLACERS.MOVEPICKUPABLEPLACER.NAME, Assets.instance.movePickupToPlacerAssets.material);
			prefab.AddOrGet<MultiFetch_CancellableMove>();
			Storage storage = prefab.AddOrGet<Storage>();
			storage.showInUI = false;
			storage.showUnreachableStatus = true;
			prefab.AddOrGet<Approachable>();
			prefab.AddOrGet<Prioritizable>();
			prefab.AddTag(GameTags.NotConversationTopic);
			return prefab;
		}

		public void OnPrefabInit(GameObject go)
		{
		}

		public void OnSpawn(GameObject go)
		{
		}
	}
}

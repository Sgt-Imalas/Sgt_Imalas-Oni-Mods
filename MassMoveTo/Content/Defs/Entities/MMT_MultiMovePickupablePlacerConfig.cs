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
			GameObject prefab = this.CreatePrefab(ID, Config.UseMultiDelivery ? STRINGS.MISC.PLACERS.MMT_MULTIMOVEPICKUPABLEPLACER.NAME : global::STRINGS.MISC.PLACERS.MOVEPICKUPABLEPLACER.NAME, Assets.instance.movePickupToPlacerAssets.material);

			///reset existing move chores on change so if it crashes, the setting can be disabled and the crash causers are removed
			if (Config.UseMultiDelivery)
				prefab.AddOrGet<MultiFetch_CancellableMove>();
			else
				prefab.AddOrGet<CancellableMove>();

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

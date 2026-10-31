using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class StorageWaterSource : KMonoBehaviour
	{
		[SerializeField]
		public Storage waterStorage = null;
		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Sub((int)GameHashes.OnStorageChange, OnStorageChanged);
			MarkAsSources(true);
		}
		public override void OnCleanUp()
		{
			this.UnsubAll();
			MarkAsSources(false);
			base.OnCleanUp();
		}

		void OnStorageChanged(object o)
		{
			MarkAsSources(true);
		}
		void MarkAsSources(bool mark)
		{
			if (waterStorage == null)
				return;
			foreach (var item in waterStorage.items)
			{
				if(!item.TryGetComponent<Pickupable>(out var pickupable))
					continue;
				if(mark)
					pickupable.KPrefabID.AddTag(GameTags.LiquidSource);
				else
					pickupable.KPrefabID.RemoveTag(GameTags.LiquidSource);
			}
		}
	}
}

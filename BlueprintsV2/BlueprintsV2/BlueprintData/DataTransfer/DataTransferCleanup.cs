using KSerialization;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
	public class DataTransferCleanup : KMonoBehaviour
	{
		[Serialize]
		public bool ComponentInUse = false;
		public void CleanupAfterUse()
		{
			ComponentInUse = true;
		}
		bool destroyed = false;
		int handle = -1;

		public override void OnSpawn()
		{
			base.OnSpawn();
			handle = Game.Instance.Subscribe((int)GameHashes.SelectObject, GlobalSelectHandler);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Game.Instance.Unsubscribe(handle);
		}
		void GlobalSelectHandler(object data)
		{
			if (!ComponentInUse || destroyed)
				return;

			if (data != null && data is GameObject go && go == gameObject)
				return;

			destroyed = true;
			UnderConstructionDataSettingHelper.CollectDataFrom(this);
			Destroy(gameObject);
		}
	}
}

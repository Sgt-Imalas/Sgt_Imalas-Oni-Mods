using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
	public class DataTransferCleanup : KMonoBehaviour
	{
		[Serialize]
		public bool ComponentInUse = false;
		public bool SetInUse(bool active = true)
		{
			ComponentInUse = active;
			return ComponentInUse;
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
			Unsubscribe(handle);
		}
		void GlobalSelectHandler(object data)
		{
			if (!ComponentInUse || destroyed)
				return;

			if (data != null && data is GameObject go && go == gameObject)
				return;

			UnderConstructionDataSettingHelper.HandleDeselection(this);
			Destroy(gameObject);
			destroyed = true;
		}
	}
}

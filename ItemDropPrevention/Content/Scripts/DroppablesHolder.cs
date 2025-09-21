using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ItemDropPrevention.Content.Scripts
{
	internal class DroppablesHolder : KMonoBehaviour
	{
		[MyCmpReq] Storage internalStorage;
		[MyCmpReq] KPrefabID kprefabID;

		HashSet<int> MarkedForDrop = [];

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.PathAdvanced, OnPathAdvanced);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.PathAdvanced, OnPathAdvanced);
			base.OnCleanUp();
		}
		void OnPathAdvanced(object _)
		{
			if (!MarkedForDrop.Any())
				return;
			//SgtLogger.l("OnPathAdvanced");

			///dont drop when not above solid tile
			if (!AboveSolidGround())
				return;

			///dont drop in transit tube
			if (kprefabID.HasTag(GameTags.InTransitTube))
				return;

			var items = internalStorage.items;

			//SgtLogger.l("items in storage: "+internalStorage.items.Count);

			//SgtLogger.l("Dropping all...");

			for (int i = items.Count - 1; i >= 0; --i)
			{
				var item = items[i];

				if (item == null)
					continue;
				int instanceID = item.GetInstanceID();
				//SgtLogger.l("index: " + i + ", item: " + item.name + " instance: " + instanceID);

				//foreach(var item2 in internalStorage.items)
				//{
				//	SgtLogger.l("item in storage: "+item2.name+", id: "+item2.GetInstanceID());
				//}
				if (MarkedForDrop.Contains(instanceID))
					internalStorage.Drop(item);
			}
			MarkedForDrop.Clear();
		}
		bool AboveSolidGround()
		{
			var cell = Grid.PosToCell(this);
			if(!Grid.IsValidCell(cell))  //should never happen, but check anyway
				return false;

			var floorCell = Grid.CellBelow(cell);
			if(!Grid.IsValidCell(floorCell))
				return false;

			bool isSolidGround = Grid.Solid[floorCell];
			//SgtLogger.l("cell "+floorCell + " is solid? " + isSolidGround);
			return isSolidGround;
		}

		internal void MarkAllItemsForDrop()
		{
			foreach (var item in internalStorage.items)
			{
				//SgtLogger.l("Marking " + item + " as drop");
				MarkedForDrop.Add(item.GetInstanceID());
			}
		}
	}
}

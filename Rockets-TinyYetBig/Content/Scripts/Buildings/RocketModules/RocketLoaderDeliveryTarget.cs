using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class RocketLoaderDeliveryTarget : KMonoBehaviour
	{
		[SerializeField] public Tag DeliveryTarget = Tag.Invalid;
		[SerializeField] public Storage TargetStorage;
		[SerializeField] public float TargetCapacity = -1;

		public override void OnSpawn()
		{
			base.OnSpawn();
		}
		public float GetRemainingCapacity()
		{
			if(TargetCapacity <= 0)
			{
				return TargetStorage.RemainingCapacity();
			}
			else
			{
				float stored = 0f;
				var items = TargetStorage.items;
				for (int i = 0; i < items.Count; i++)
				{
					GameObject gameObject = items[i];
					if (gameObject != null && gameObject.HasTag(DeliveryTarget))
					{
						stored += gameObject.GetComponent<PrimaryElement>().Units;
					}
				}
				return TargetCapacity - stored;
			}
		}
		public bool AllowedToStore(KPrefabID item) => item.HasTag(DeliveryTarget);
		
	}
}

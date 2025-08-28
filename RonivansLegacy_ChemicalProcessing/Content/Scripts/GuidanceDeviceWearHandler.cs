using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class GuidanceDeviceWearHandler : KMonoBehaviour
	{
		[SerializeField]
		public float wearMin = 0.02f, wearMax = 0.06f;

		public Storage SourceStorage, TargetStorage;
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.Unsubscribe((int)GameHashes.OnStorageChange, OnStorageChanged);

		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Subscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
		}
		void OnStorageChanged(object data)
		{
			for (int i = SourceStorage.items.Count - 1; i >= 0; i--)
			{
				var item = SourceStorage.items[i];
				if (item.TryGetComponent<ProgrammableGuidanceModule>(out var module))
				{
					var itemDamage = UnityEngine.Random.Range(wearMin, wearMax);
					module.DealWearDamage(itemDamage, out bool destroyed);
					if (!destroyed)
						SourceStorage.Transfer(item, TargetStorage, true, true);
					else
					{
						SourceStorage.Drop(item, true);
						module.SelfDestruct();
					}
				}
			}
		}
	}
}

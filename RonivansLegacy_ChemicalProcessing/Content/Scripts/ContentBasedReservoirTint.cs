using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class ContentBasedReservoirTint : KMonoBehaviour
	{
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] SmartReservoir reservoir;

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
			RecalculateTint();
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
			base.OnCleanUp();
		}

		void OnStorageChanged(object o)
		{
			RecalculateTint();
		}

		Dictionary<SimHashes, float> CachedElementMasses = new();
		void RecalculateTint()
		{
			var color = Color.clear;

			if (reservoir.storage.MassStored() > 0)
			{
				CachedElementMasses.Clear();
				foreach (var item in reservoir.storage.items)
				{
					if (!item.TryGetComponent<PrimaryElement>(out var element))
						continue;
					var id = element.ElementID;

					if (!CachedElementMasses.ContainsKey(id))
						CachedElementMasses[id] = 0;
					CachedElementMasses[id] += element.Mass;
				}
				if (CachedElementMasses.Any())
				{
					var highestValue = CachedElementMasses.OrderByDescending(item => item.Value)?.FirstOrDefault();
					if (highestValue != null)
					{
						var element = ElementLoader.FindElementByHash(highestValue.Value.Key);
						color = element.substance.colour;
						color.a = 1;
					}
				}
			}
			kbac.SetSymbolTint("back", color);
		}
	}
}

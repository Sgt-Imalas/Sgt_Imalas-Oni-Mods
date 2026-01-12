using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class BabyProofedStorageCapacity : KMonoBehaviour
	{
		[MyCmpReq] Storage storage;

		public override void OnSpawn()
		{
			base.OnSpawn();
			var storageCapacity = storage.Capacity();
			var maxMass = PrimaryElement.MAX_MASS;

			if (storageCapacity <= maxMass)
				return;

			SgtLogger.l($"Someone the reservoir [{this.GetProperName()} a broken capacity that will get clamped: {storageCapacity} capacity, while the max allowed mass by the game is {maxMass}. Applying clamping to the reservoir to prevent mass loss on the next save.");
			storage.capacityKg = maxMass;			
			if(TryGetComponent<ConduitConsumer>(out var input))
				input.capacityKG = maxMass;
			var portConduitConsumers = GetComponents<PortConduitConsumer>();
			if (portConduitConsumers.Any())
			{
				foreach(var consumer in portConduitConsumers)
					consumer.capacityKG = maxMass;
			}

		}
	}
}

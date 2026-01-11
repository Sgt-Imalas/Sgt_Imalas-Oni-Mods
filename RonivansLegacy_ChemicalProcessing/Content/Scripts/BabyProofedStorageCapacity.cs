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

			SgtLogger.l($"Someone missused the building config and gave the reservoir [{this.GetProperName()} a broken capacity that will get clamped: {storageCapacity} capacity, while the max mass is {maxMass}");
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

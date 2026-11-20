using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{

	public class LifeSupportInteriorBuilding : KMonoBehaviour, ISim1000ms
	{

		[SerializeField] public Storage co2Storage;
		[SerializeField] public Storage o2Storage;

		private int nextModuleIndex = 0;
		private int handle = -1;
		private WorldContainer myWorld;
		private CraftModuleInterface rocket;

		private readonly List<LifeSupportRocketModule> connectedModules = [];

		public override void OnSpawn()
		{
			ModComps.InteriorLifeSupports.Add(this);
			base.OnSpawn();
			myWorld = this.GetMyWorld();
			handle = myWorld.Subscribe((int)GameHashes.RocketModuleChanged, RefreshConnectedHandlers);
			rocket = myWorld.GetComponent<CraftModuleInterface>();
			StartCoroutine(DelayedRefresh());
		}
		public override void OnCleanUp()
		{
			ModComps.InteriorLifeSupports.Remove(this);
			if (!myWorld.IsNullOrDestroyed() && handle != -1)
				myWorld.Unsubscribe(handle);
			base.OnCleanUp();
		}
		IEnumerator DelayedRefresh()
		{
			yield return null;
			RefreshConnectedHandlers(null);
		}

		void RefreshConnectedHandlers(object _)
		{
			connectedModules.Clear();
			nextModuleIndex = 0;

			foreach (var moduleRef in rocket.ClusterModules)
			{
				var module = moduleRef.Get();
				if (module == null)
					continue;

				if (!module.TryGetComponent<LifeSupportRocketModule>(out var support))
					continue;

				connectedModules.Add(support);
			}
		}

		void ResourceTransfer()
		{
			int moduleCount = connectedModules.Count;
			if (moduleCount == 0)
				return;

			if (nextModuleIndex >= moduleCount)
				nextModuleIndex = 0;

			float remainingOxygenToTake = o2Storage.RemainingCapacity();
			float remainingCo2ToPut = co2Storage.MassStored();

			for (int i = 0; i < moduleCount; i++)
			{
				int index = (nextModuleIndex + i) % moduleCount;
				LifeSupportRocketModule module = connectedModules[index];

				bool hasRemainingOxygenSpace = remainingOxygenToTake > 0;
				bool hasRemainingCo2ToSend = remainingCo2ToPut > 0;

				if (hasRemainingOxygenSpace)
				{
					remainingOxygenToTake -= module.OperationalStorage.Transfer(this.o2Storage, GameTags.Oxygen, remainingOxygenToTake, hide_popups: true);
				}
				float remainingTargetCo2Cacpacity = module.Co2Target - module.OperationalStorage.GetMassAvailable(GameTags.CarbonDioxide);

				if (hasRemainingCo2ToSend && remainingTargetCo2Cacpacity > 0)
				{
					remainingCo2ToPut -= this.co2Storage.Transfer(module.OperationalStorage, GameTags.CarbonDioxide, remainingTargetCo2Cacpacity, hide_popups: true);
				}
				if (remainingCo2ToPut <= 0 && remainingOxygenToTake <= 0)
					break;
			}
			nextModuleIndex = (nextModuleIndex + 1) % moduleCount;
		}

		public void Sim1000ms(float dt)
		{
			ResourceTransfer();
		}
	}
}

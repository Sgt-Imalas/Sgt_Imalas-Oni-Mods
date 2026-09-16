using Rockets_TinyYetBig.Buildings.Nosecones;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Utility
{
	public class DrillConeAssistentModuleHEP : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet] public HighEnergyParticleStorage HEPStorage;
		[MyCmpGet] RocketModuleCluster module;

		HighEnergyParticleStorage TargetStorage = null;

		public void Sim1000ms(float dt)
		{
			var clustercraft = module.CraftInterface.m_clustercraft;
			if (clustercraft.Status == Clustercraft.CraftStatus.Grounded)
			{
				CheckTarget();
			}
			if (clustercraft.Status == Clustercraft.CraftStatus.InFlight)
			{
				if (TargetStorage != null && !TargetStorage.IsNullOrDestroyed())
				{
					TransferParticle();
				}
				else
					CheckTarget();

			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			CheckTarget();
			//this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition)new ConditionHasResource(this.particleStorage, SimHashes.particle, (float)Config.Instance.DrillconeSupportparticleMass));
		}

		private void TransferParticle()
		{
			float remainingCapacity = TargetStorage.RemainingCapacity();
			float currentparticles = HEPStorage.Particles;
			var particlesToTransfer = Mathf.Min(currentparticles, remainingCapacity);
			TargetStorage.Store(HEPStorage.ConsumeAndGet(particlesToTransfer));
		}

		private void CheckTarget()
		{

			if (TargetStorage != null && !TargetStorage.IsNullOrDestroyed())
				return;

			foreach (var otherModule in module.CraftInterface.ClusterModules)
			{
				if (otherModule.Get().GetDef<ResourceHarvestModuleHEP.Def>() != null)
				{
					if (otherModule.Get().gameObject.TryGetComponent<HighEnergyParticleStorage>(out var storage))
					{
						TargetStorage = storage;
						return;
					}
				}
			}
		}
	}
}

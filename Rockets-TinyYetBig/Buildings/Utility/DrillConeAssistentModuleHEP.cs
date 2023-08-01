using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI.DEVELOPMENTBUILDS.ALPHA;
using UnityEngine;
using Rockets_TinyYetBig.Buildings.Nosecones;

namespace Rockets_TinyYetBig.Buildings.Utility
{
    public class DrillConeAssistentModuleHEP : KMonoBehaviour, ISim4000ms
    {
        [MyCmpGet] public HighEnergyParticleStorage HEPStorage;
        [MyCmpGet] RocketModuleCluster module;

        HighEnergyParticleStorage TargetStorage = null;

        public void Sim4000ms(float dt)
        {
            var clustercraft = module.CraftInterface.m_clustercraft;
            if (clustercraft.Status == Clustercraft.CraftStatus.Grounded)
            {
                CheckTarget();
            }
            if(clustercraft.Status == Clustercraft.CraftStatus.InFlight)
            {
                if (TargetStorage != null || !TargetStorage.IsNullOrDestroyed())
                {
                    Transferparticle();
                }
            }
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            CheckTarget();
            //this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition)new ConditionHasResource(this.particleStorage, SimHashes.particle, (float)Config.Instance.DrillconeSupportparticleMass));
        }

        private void Transferparticle()
        {
            float remainingCapacity = TargetStorage.RemainingCapacity();
            float currentparticles = HEPStorage.Particles;
            var particlesToTransfer = Mathf.Min(currentparticles, remainingCapacity);
            TargetStorage.Store(HEPStorage.ConsumeAndGet(particlesToTransfer));
        }

        private void CheckTarget()
        {

            if (TargetStorage != null && TargetStorage.IsNullOrDestroyed())
                return;

            foreach (var otherModule in module.CraftInterface.ClusterModules)
            {
                if (otherModule.Get().GetDef<NoseConeHEPHarvest.Def>() != null)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI.DEVELOPMENTBUILDS.ALPHA;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Utility
{
    internal class DrillConeAssistentModule : KMonoBehaviour, ISim4000ms
    {
        [MyCmpGet] public Storage DiamondStorage;
        [MyCmpGet] RocketModuleCluster module;
        private MeterController meter;

        Storage TargetStorage = null;

        private void OnStorageChange(object data) => this.meter.SetPositionPercent(this.DiamondStorage.MassStored() / this.DiamondStorage.Capacity());
        private static readonly EventSystem.IntraObjectHandler<DrillConeAssistentModule> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<DrillConeAssistentModule>(((component, data) => component.OnStorageChange(data)));

        public void Sim4000ms(float dt)
        {
            module.CraftInterface.TryGetComponent<Clustercraft>(out var clustercraft);
            if (clustercraft.Status == Clustercraft.CraftStatus.Grounded)
            {
                CheckTarget();
            }
            if(clustercraft.Status == Clustercraft.CraftStatus.InFlight)
            {
                if (TargetStorage != null || !TargetStorage.IsNullOrDestroyed())
                {
                    TransferDiamond();
                }
            }
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            CheckTarget();



            this.meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, null);
            this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;

            this.Subscribe<DrillConeAssistentModule>(-1697596308, OnStorageChangeDelegate);
            this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition)new ConditionHasResource(this.DiamondStorage, SimHashes.Diamond, 1500f));
        }

        private void TransferDiamond()
        {
                float remainingCapacity = TargetStorage.RemainingCapacity();
                float currentDiamonds = DiamondStorage.MassStored();
            for (int num = DiamondStorage.items.Count - 1; num >= 0; num--)
            {
                GameObject gameObject = DiamondStorage.items[num];

                bool filterable = TargetStorage.storageFilters != null && TargetStorage.storageFilters.Count > 0;

                if (remainingCapacity > 0f && currentDiamonds > 0f && (filterable ? TargetStorage.storageFilters.Contains(gameObject.PrefabID()) : true))
                {
                    Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(remainingCapacity);
                    if (pickupable != null)
                    {
                        TargetStorage.Store(pickupable.gameObject,true);
                        remainingCapacity -= pickupable.PrimaryElement.Mass;
                    }
                }
            }
        }

        private void CheckTarget()
        {

            if (TargetStorage != null && TargetStorage.IsNullOrDestroyed())
                return;

            foreach (var otherModule in module.CraftInterface.ClusterModules)
            {
                if (otherModule.Get().GetDef<ResourceHarvestModule.Def>() != null)
                {
                    if (otherModule.Get().gameObject.TryGetComponent<Storage>(out var storage))
                    {
                        TargetStorage = storage;
                        return;
                    }
                }
            }
        }
    }
}

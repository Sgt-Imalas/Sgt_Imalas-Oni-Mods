using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoboRockets.LearningBrain
{
    internal class BrainTeacher : KMonoBehaviour//, ISim4000ms
    {
        [MyCmpReq]
        public Storage BrainStorage;

        [MyCmpReq]
        private RocketModuleCluster module;
        private Clustercraft craft;
        public MeterController brain_content { get; private set; }
        //[Serialize]
        //AxialI oldLocation;

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.craft = this.module.CraftInterface.GetComponent<Clustercraft>();
            brain_content = new MeterController(GetComponent<KBatchedAnimController>(), "bowl_target", "brain_status", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            brain_content.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;


            craft.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.OnClusterLocationChanged));


            OnStorageChange(null);
            //this.Subscribe<RadiationBatteryOutputHandler>((int)GameHashes.ParticleStorageCapacityChanged, OnStorageChangedDelegate);
            BrainStorage.Subscribe((int)GameHashes.OnStorageChange, OnStorageChange);
        }
        private void OnStorageChange(object data) 
        {
            var loadedBrain = BrainStorage.FindFirst(ModAssets.Tags.SpaceBrain);
            brain_content.SetPositionPercent(loadedBrain != null ? 1f : 0f);

            if(loadedBrain != null) { 
                if(loadedBrain.TryGetComponent<FlyingBrain>(out var flyer))
                {
                    this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.ExperienceLevel, (object)flyer);
                }
            }
            else
            {
               // this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.NoBrain, (object)null);
            }

        }
        public void OnClusterLocationChanged(object o)


        {
            var brain = BrainStorage.FindFirst(ModAssets.Tags.SpaceBrain);
            if(brain != null)
            {
                var speedHandler = brain.GetComponent<FlyingBrain>();
                speedHandler.TraveledDistance(1);
                Debug.Log("Brain Learned a bit; new skill level: "+ speedHandler.GetCurrentSpeed());
                craft.AutoPilotMultiplier = speedHandler.GetCurrentSpeed();
                //craft.PilotSkillMultiplier = speedHandler.GetCurrentSpeed();
            }

        }

        public override void OnCleanUp()
        {

            craft.Unsubscribe((int)GameHashes.ClusterDestinationChanged, new System.Action<object>(this.OnClusterLocationChanged));
            base.OnCleanUp();
        }

        public void Sim4000ms(float dt)
        {
            throw new NotImplementedException();
        }
    }
}

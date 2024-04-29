using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.Behaviors
{
    internal class StoragesMeter:KMonoBehaviour
    {
        [MyCmpGet]
        KBatchedAnimController kbac;

        private MeterController storageMeter;
        private List<Storage> storages;

        public override void OnSpawn()
        {
            base.OnSpawn();
            storages = this.GetComponents<Storage>().ToList();

            string meterTarget;
            if (kbac.curBuild.GetSymbol("block_frame") != null)
                meterTarget = "block_frame";
            else if (kbac.curBuild.GetSymbol("target_meter") != null)
                meterTarget = "target_meter";
            else
                meterTarget = "meter_target";


            storageMeter = new MeterController(kbac, meterTarget, "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[]
            {});

            foreach (Storage s in storages)
            {
                s.Subscribe((int)GameHashes.OnStorageChange, UpdateMeter);
            }
            UpdateMeter(null);
        }
        public override void OnCleanUp()
        {
            foreach(Storage s in storages)
            {
                s.Unsubscribe((int)GameHashes.OnStorageChange, UpdateMeter);
            }

            base.OnCleanUp();
        }
        
        void UpdateMeter(object data)
        {
            float maxValue =0,currentValue = 0;
            foreach (Storage s in storages)
            {
                maxValue += s.capacityKg;
                currentValue += s.MassStored();
            }
            storageMeter.SetPositionPercent(Mathf.Clamp(currentValue / maxValue, 0,1f));
            
        }
    }
}

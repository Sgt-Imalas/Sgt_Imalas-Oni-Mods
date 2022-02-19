using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KnastoronOniMods
{
    class RocketControlStationNoChorePrecondition : RocketControlStation
    {
        [Serialize]
        
        private SchedulerHandle newSweepyHandle;
        private GameObject brainController;
        public void MakeNewPilotBot()
        {
            if (this.newSweepyHandle.IsValid)
                return;
            this.newSweepyHandle = GameScheduler.Instance.Schedule("Make brain", 1f, (System.Action<object>)(obj =>
            {
                GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"AiBrain"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures);
                go.SetActive(true);
                brainController = go;


                this.newSweepyHandle.ClearScheduler();
            }), (object)null, (SchedulerGroup)null);

        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            MakeNewPilotBot(); smi.enableConsoleLogging = true;
        }
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            brainController.GetComponent<SelfDestructInWrongEnvironmentComponent>().SelfDestruct();
        }
    }
}

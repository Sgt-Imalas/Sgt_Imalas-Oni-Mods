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
        public void MakeNewPilotBot()
        {
            if (this.newSweepyHandle.IsValid)
                return;
            this.newSweepyHandle = GameScheduler.Instance.Schedule("Make brain", 1f, (System.Action<object>)(obj =>
            {
                GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"AiBrain"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures);
                go.SetActive(true);
                
                this.newSweepyHandle.ClearScheduler();
            }), (object)null, (SchedulerGroup)null);

        }

    }
}

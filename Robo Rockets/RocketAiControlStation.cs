using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KnastoronOniMods
{
    class RocketAiControlStation : KMonoBehaviour
    {
        private SchedulerHandle newSweepyHandle;
        [Serialize]
        public Ref<KSelectable> sweepBot;
        protected override void OnSpawn()
        {
            this.MakeNewScoutBot();
        }
        private void MakeNewScoutBot()
        {
            if (this.newSweepyHandle.IsValid)
                return;
            this.newSweepyHandle = GameScheduler.Instance.Schedule("MakeSweepy", 2f, (System.Action<object>)(obj =>
            {
                GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"AiBrain"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures);
                go.SetActive(true);
                this.sweepBot = new Ref<KSelectable>(go.GetComponent<KSelectable>());
                this.newSweepyHandle.ClearScheduler();
            }), (object)null, (SchedulerGroup)null);
            
        }
    }
}

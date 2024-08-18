using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace BathTub.Duck
{
    internal class DuckNoises:KMonoBehaviour
    {        
        private System.Action<object> m_onSelectObjectDelegate;
        private SchedulerHandle NextQuackHandle;

        public override void OnSpawn()
        {
            this.m_onSelectObjectDelegate = new System.Action<object>(this.OnSelect);
            base.OnSpawn(); 
            this.Subscribe((int)GameHashes.SelectObject, m_onSelectObjectDelegate);
            ScheduleNextQuack();
        }
        public override void OnCleanUp()
        {
            this.Unsubscribe((int)GameHashes.SelectObject, m_onSelectObjectDelegate);
            if (NextQuackHandle.IsValid)
                NextQuackHandle.ClearScheduler();

            base.OnCleanUp();
        }
        public void ScheduleNextQuack()
        {
            var time = new KRandom().Next(25, 100);
            NextQuackHandle = GameScheduler.Instance.Schedule("Quack", time, Quack);
        }
        public void Quack(object o)
        {
            ModAssets.PlayRandomQuack(this);
            ScheduleNextQuack();
        }

        void OnSelect(object data)
        {
            if (!(bool)data) //on deselect
                return;
            ModAssets.PlayRandomSqueak(this);
        }
    }
}

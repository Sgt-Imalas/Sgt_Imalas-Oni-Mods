using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShockWormMob
{
    internal class TestEventSpawner : KMonoBehaviour,ISingleSliderControl, ISidescreenButtonControl
    {
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

        }
        public override void OnSpawn()
        {
            base.OnSpawn();




            //foreach(var eve in Db.Get().GameplayEvents.resources)
            //{
            //    events.Add(eve);
            //}
        }

        
        int currentEvent=0;
        public string SliderTitleKey => "Event number";

        public string SliderUnits => "nr";

        public string SidescreenButtonText => currentEvent < ModAssets.events.Count ? ModAssets.events[currentEvent].Id.ToString() : "no selected";

        public string SidescreenButtonTooltip => "spawn event";

        public int ButtonSideScreenSortOrder() => 21;

        public float GetSliderMax(int index) => ModAssets.events.Count-1;

        public float GetSliderMin(int index) => 0;

        public string GetSliderTooltip() => "SelectEvent";
        public string GetSliderTooltip(int i) => "SelectEvent";

        public string GetSliderTooltipKey => "";

        public float GetSliderValue(int index) => currentEvent;

        void ExecuteCurrentEvent() 
        {
            GameplayEventInstance eventInstance = GameplayEventManager.Instance.StartNewEvent(ModAssets.events[currentEvent]);
            //eventInstance.ShowEventPopup();
        }

        public void OnSidescreenButtonPressed() => ExecuteCurrentEvent();

        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
        {
        }

        public void SetSliderValue(float value, int index) 
        { 
            this.currentEvent = (int)value;
            //DetailsScreen.Instance.Refresh(this.gameObject);
        }

        public bool SidescreenButtonInteractable() => true;

        public bool SidescreenEnabled() => true;

        public int SliderDecimalPlaces(int index) => 0;

        string ISliderControl.GetSliderTooltipKey(int index)
        {
            return string.Empty;
        }

        public int HorizontalGroupID() => -1;
    }
}

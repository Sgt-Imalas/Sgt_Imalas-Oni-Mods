using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.Utility
{
    public class  DrillConeModeHandler : KMonoBehaviour, ISidescreenButtonControl
    {
        [MyCmpGet] public Storage DiamondStorage;
        [Serialize] bool IsAutoLoader = false;
        [Serialize] float ManualDeliveryOriginalCapacity;


        public bool LoadingAllowed => IsAutoLoader;

        public string SidescreenButtonText => IsAutoLoader ? "Switch to manual loading" : "Switch to automated loading"; //TODO LOC

        public string SidescreenButtonTooltip => "toggle between automatic and manual loading";//TODO LOC

        public override void OnSpawn()
        {
            base.OnSpawn(); 
            if (gameObject.TryGetComponent<ManualDeliveryKG>(out var deliveryKG))
            {
                ManualDeliveryOriginalCapacity = deliveryKG.capacity;
            }
            ToggleBetweenAutoAndManual();
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        void ToggleBetweenAutoAndManual()
        {
            if(gameObject.TryGetComponent<ManualDeliveryKG>(out var deliveryKG))
            {
                if (IsAutoLoader)
                {
                    deliveryKG.AbortDelivery("Mode Toggled on drill module.");
                    //deliveryKG.ClearRequests();
                    ManualDeliveryOriginalCapacity = deliveryKG.capacity;
                    deliveryKG.capacity = 0;
                }
                else
                {

                    deliveryKG.capacity = ManualDeliveryOriginalCapacity;
                }

            }
            SgtLogger.debuglog("DeliveryEnabled? : "+ !IsAutoLoader+", Capacity of Manual: "+deliveryKG.capacity+", Original:"+ManualDeliveryOriginalCapacity);
        }

        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
        {
            throw new NotImplementedException();
        }

        public bool SidescreenEnabled() => true;

        public bool SidescreenButtonInteractable() => true;

        public void OnSidescreenButtonPressed()
        {
            IsAutoLoader = !IsAutoLoader;
            ToggleBetweenAutoAndManual();
        }

        public int ButtonSideScreenSortOrder() => 21;

        public int HorizontalGroupID()
        {
            return -1;
        }
    }
}

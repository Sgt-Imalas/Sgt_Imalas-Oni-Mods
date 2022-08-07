using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopod.Buildings
{
    
    class CryopodLiquidPortAddon : KMonoBehaviour, ISim200ms
    {
        [MyCmpGet] private Storage storage;
        [Serialize] private float heatToThrowOut;

        public void AddHeatToLiquid(float heatAmount)
        {
            heatToThrowOut += heatAmount;
        }

        public void Sim200ms(float dt)
        {
            if (storage.MassStored() > 0.5f )
            {
                var contents = storage.GetItems().First().GetComponent<PrimaryElement>();
                var element = ElementLoader.FindElementByHash(contents.ElementID);
                var tempDelta = heatToThrowOut / contents.Mass / element.specificHeatCapacity;
                if (tempDelta != float.NaN) 
                {   if((contents.Temperature + tempDelta)>0f && (contents.Temperature + tempDelta )< Sim.MaxTemperature) {
                        //Debug.Log(contents.Temperature + tempDelta);
                        contents.Temperature = contents.Temperature + tempDelta;
                    }
                }
                heatToThrowOut = 0;
            }
            if (storage.MassStored() > 49f)
            {
                this.GetComponent<ConduitDispenser>().isOn = true;
            }
            else
            {
                this.GetComponent<ConduitDispenser>().isOn = false;

            }
        }
    }
}

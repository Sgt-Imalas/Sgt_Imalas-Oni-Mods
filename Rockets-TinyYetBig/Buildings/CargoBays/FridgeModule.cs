using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static Storage;

namespace Rockets_TinyYetBig.Buildings.CargoBays
{
    public class FridgeModule : KMonoBehaviour, ISim200ms
    {
        [MyCmpGet]
        public Storage fridgeStorage;
        [MyCmpGet]
        Operational operational;
        [Serialize]
        bool WasPreviouslyOperational;
        bool CurrentlyOperational;

        public void Sim200ms(float dt)
        {
            CurrentlyOperational = operational.IsOperational;
            if (CurrentlyOperational != WasPreviouslyOperational)
            {
                WasPreviouslyOperational = CurrentlyOperational;
                if (CurrentlyOperational)
                {
                    SgtLogger.debuglog("Changed to: " + CurrentlyOperational);
                    fridgeStorage.SetDefaultStoredItemModifiers(new List<StoredItemModifier>
                    {
                        StoredItemModifier.Hide,
                        StoredItemModifier.Seal,
                        StoredItemModifier.Preserve, 
                        StoredItemModifier.Insulate
                    });
                }
                else
                {
                    fridgeStorage.SetDefaultStoredItemModifiers(new List<StoredItemModifier>
                    {
                        StoredItemModifier.Hide,
                        StoredItemModifier.Seal
                    });
                }
                foreach(var item in fridgeStorage.items)
                {
                    if (item != null)
                    {
                        fridgeStorage.ApplyStoredItemModifiers(item, true, false);
                    }
                }
            }
        }
    }
}

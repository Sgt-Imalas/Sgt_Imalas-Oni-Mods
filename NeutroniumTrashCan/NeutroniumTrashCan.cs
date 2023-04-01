using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeutroniumTrashCan
{
    internal class NeutroniumTrashCan : KMonoBehaviour, ISim1000ms
    {
        [MyCmpGet]
        Storage storage;
        public void Sim1000ms(float dt)
        {
            if (storage.MassStored() > 0)
            {
                foreach(var item in storage.items)
                {
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RadiatorMod.Util
{
    public class panel : KMonoBehaviour, ISim1000ms
    {
        public bool SpaceExposedCurrent = false;

        public bool ExposedToSpace()
        {
            return Grid.IsCellOpenToSpace(Grid.PosToCell(this));
            //Debug.Log(SpaceExposedCurrent);
        }
        public void Sim1000ms(float dt)
        {
            Debug.Log(ExposedToSpace());
        }
    }
}

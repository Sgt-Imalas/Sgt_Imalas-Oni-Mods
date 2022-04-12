using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiatorMod
{
    class Radiator : KMonoBehaviour
        //, IGameObjectEffectDescriptor
        , ISim200ms
    {
        [MyCmpGet]
        private Rotatable rotatable;

        public void Sim200ms(float fl)
        {
            if ((bool)(UnityEngine.Object)this.rotatable)
                Debug.Log(rotatable.GetOrientation());
        }
    }
}

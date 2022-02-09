using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KnastoronOniMods
{
    class SelfDestructInWrongEnvironmentComponent : KMonoBehaviour, ISim4000ms
    {
       
        public void Sim4000ms(float dt)
        {
            // Debug.Log(StartWorldId + "<- WorldID");
            if (this.gameObject == null||this.GetMyWorld()==null)
            {
                Destroy(this.gameObject);
            }

        }
    }
}

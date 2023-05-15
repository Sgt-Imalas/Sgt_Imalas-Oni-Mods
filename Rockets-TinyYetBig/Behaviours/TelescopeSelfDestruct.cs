
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    public class TelescopeSelfDestruct : KMonoBehaviour, ISim4000ms
    {
        [MyCmpGet]
        TelescopeTarget target;
        public void Sim4000ms(float dt)
        {
            if (target.targetMeteorShower == null && Mathf.Approximately(target.GetProgress(), 1))
            {
                Util.KDestroyGameObject(gameObject);
            }
        }
    }
}

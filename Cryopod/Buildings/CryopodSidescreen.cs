using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryopod.Buildings
{
    class CryopodSidescreen : SideScreenContent
    {
        public override bool IsValidForTarget(GameObject target) => (UnityEngine.Object)target.GetComponent<CryopodReusable>() != (UnityEngine.Object)null;
    }
}

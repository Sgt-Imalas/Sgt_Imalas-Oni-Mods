using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Behaviours
{
    public class RTB_RocketBooster : KMonoBehaviour
    {
        [MyCmpReq]
        RocketModuleCluster module;
        [MyCmpReq]
        Storage boosterStorage;

        //Engine Power when fueled
        [Serialize]
        float AdditionalEnginePower = 1000;

    }
}

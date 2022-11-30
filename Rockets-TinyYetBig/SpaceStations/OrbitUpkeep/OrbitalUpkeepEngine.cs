using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.SpaceStations.OrbitUpkeep
{
    internal class OrbitalUpkeepEngine : KMonoBehaviour
    {
        [Serialize]
        Ref<OrbitalUpkeepObject> worldUpkeepMng;
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            var worldUpkeepMng = ClusterManager.Instance.GetMyWorld().GetSMI<OrbitalUpkeepObject>();

        }
    }
}

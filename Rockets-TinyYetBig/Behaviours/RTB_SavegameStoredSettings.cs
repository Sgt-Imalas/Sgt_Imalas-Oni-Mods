using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Behaviours
{
    internal class RTB_SavegameStoredSettings : KMonoBehaviour
    {
        public static RTB_SavegameStoredSettings Instance;


        [Serialize]
        public bool useModuleCategories = true;

        [Serialize]
        public bool trulyUsingCompressedInteriors;


        public override void OnSpawn()
        {
            base.OnSpawn();
            Instance = this;
            trulyUsingCompressedInteriors = Config.Instance.CompressInteriors;
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
            Instance = null;
        }

        public bool CheckCompressionState()
        {
            if (!trulyUsingCompressedInteriors)
                return false;

            foreach(var world in ClusterManager.Instance.WorldContainers)
            {
                if(world != null && world.IsModuleInterior && world.WorldSize == TUNING.ROCKETRY.ROCKET_INTERIOR_SIZE)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

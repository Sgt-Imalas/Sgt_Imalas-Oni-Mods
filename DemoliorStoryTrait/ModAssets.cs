using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DemoliorStoryTrait
{
    class ModAssets
    {
        public static int GetImpactorWorldID()
        {
            int worldId = 0;
            if(DlcManager.IsPureVanilla())
                return worldId;
			foreach (var worldcontainer in ClusterManager.Instance.WorldContainers)
			{
				//there can only be one asteroid targeted by the impactor in the cluster at the same time
				if (worldcontainer.GetSeasonIds().Contains("LargeImpactor"))
				{
					worldId = worldcontainer.id;
					break;
				}
			}
			return worldId;
        }
    }
}

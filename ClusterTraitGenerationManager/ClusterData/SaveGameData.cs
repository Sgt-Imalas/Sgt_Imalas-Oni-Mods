using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterTraitGenerationManager.ClusterData
{
    internal class SaveGameData : KMonoBehaviour
    {
        internal bool TryGetGeyserOverride(GameObject placer, out string overrideID)
        {
            overrideID = null;
            return false;
            var world = placer.GetMyWorld();

        }
    }
}

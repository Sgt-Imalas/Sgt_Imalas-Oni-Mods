using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.CarePackageEditor.UnlockConditions
{
    internal class ItemDiscoveredCondition : ICarePackageUnlockCondition
    {
        public string PrefabId;
        public ItemDiscoveredCondition(string prefabId)
        {
            PrefabId = prefabId;
        }

        public bool UnlockConditionFulfilled()
        {
            return Immigration.DiscoveredCondition(PrefabId);
        }
    }
}

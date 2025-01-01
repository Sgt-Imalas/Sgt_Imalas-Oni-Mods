using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.CarePackageEditor.UnlockConditions
{
    internal class ElementUnlockCondition : ICarePackageUnlockCondition
    {
        public SimHashes Element;

        public bool UnlockConditionFulfilled()
        {
            if (ElementLoader.FindElementByHash(Element) == null)
                return false;
            return Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(Element).tag);
        }
    }
}

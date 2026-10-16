namespace UnlockConditions
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

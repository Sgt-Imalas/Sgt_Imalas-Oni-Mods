namespace UnlockConditions
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

namespace UnlockConditions
{
    public class CycleUnlockCondition : ICarePackageUnlockCondition
    {
        public int CycleUnlock;
        public CycleUnlockCondition(int CycleUnlock)
        {
            this.CycleUnlock = CycleUnlock;
        }
        public bool UnlockConditionFulfilled()
        {
            return Immigration.CycleCondition(CycleUnlock);
        }
    }
}

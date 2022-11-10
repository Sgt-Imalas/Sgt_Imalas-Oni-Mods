

using UnityEngine;
using static Robo_Rockets.STRINGS;

namespace Robo_Rockets
{
    public class ConditionAiHasControl : ProcessCondition
    {
        private RocketModuleCluster module;
        public ConditionAiHasControl(RocketModuleCluster module) => this.module = module;

        public override Status EvaluateCondition()
        {
            if(module.TryGetComponent<Storage>(out var storage))
            {
                if (storage.storageFilters.Contains(ModAssets.Tags.SpaceBrain))
                {
                    if (storage.MassStored() > 5)
                    {
                        return ProcessCondition.Status.Ready;
                    }
                    else
                    {
                        return ProcessCondition.Status.Failure;
                    }
                }
            }
            return ProcessCondition.Status.Ready;
        }
        public override string GetStatusMessage(Status status) => (string)UI.STARMAP.AISTATUS.NAME;
        public override string GetStatusTooltip(Status status) => (string)UI.STARMAP.AISTATUS.TOOLTIP;
        public override bool ShowInUI() => true;
    }
}



using static RoboRockets.STRINGS;

namespace KnastoronOniMods
{
    public class ConditionAiHasControl : ProcessCondition
    {
        public ConditionAiHasControl() { }

        public override Status EvaluateCondition() => ProcessCondition.Status.Ready;
        public override string GetStatusMessage(Status status) => (string)UI.STARMAP.AISTATUS.NAME;
        public override string GetStatusTooltip(Status status) => (string)UI.STARMAP.AISTATUS.TOOLTIP;
        public override bool ShowInUI() => true;
    }
}

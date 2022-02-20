
using STRINGS;

namespace KnastoronOniMods
{
    public class ConditionAiHasControl : ProcessCondition
    {
        public ConditionAiHasControl() { }

        public override Status EvaluateCondition() => ProcessCondition.Status.Ready;
        public override string GetStatusMessage(Status status) => (string)"Ai controlled";
        public override string GetStatusTooltip(Status status) => (string)"This Rocket flies on it's own - your duplicants are scared yet impressed!";
        public override bool ShowInUI() => true;
    }
}

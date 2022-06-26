using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatelites.Behaviours
{
    class SatelliteConstructionCompleteCondition : ProcessCondition
    {
        private BuildingInternalConstructorRocket.Instance target;

        public SatelliteConstructionCompleteCondition(BuildingInternalConstructorRocket.Instance target) => this.target = target;

        public override ProcessCondition.Status EvaluateCondition() => this.target.IsRequestingConstruction() && !this.target.HasOutputInStorage() ? ProcessCondition.Status.Warning : ProcessCondition.Status.Ready;

        public override string GetStatusMessage(ProcessCondition.Status status) => (string)(status == ProcessCondition.Status.Ready ? "Satellite Construction" : "Satellite Construction");

        public override string GetStatusTooltip(ProcessCondition.Status status) => (string)(status == ProcessCondition.Status.Ready ? "All satellites have been built and are ready for deployment" : "Additional satellites must be constructed to fulfill the satellite requests of this rocket");

        public override bool ShowInUI() => true;
    }
}

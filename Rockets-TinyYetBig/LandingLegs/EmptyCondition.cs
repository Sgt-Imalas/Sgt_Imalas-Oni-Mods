using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;

namespace Rockets_TinyYetBig.LandingLegs
{
    internal class EmptyCondition : ProcessCondition
    {
        public GameObject target;

        public EmptyCondition(GameObject target)
        {
            this.target = target;
        }

        public override Status EvaluateCondition()
        {
            return Status.Ready;
        }

        public override string GetStatusMessage(Status status)
        {
            if (status == Status.Ready)
            {
                return global::STRINGS.UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.STATUS.READY;
            }

            return global::STRINGS.UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.STATUS.WARNING;
        }

        public override string GetStatusTooltip(Status status)
        {
            if (status == Status.Ready)
            {
                return global::STRINGS.UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.TOOLTIP.READY;
            }

            return global::STRINGS.UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.TOOLTIP.WARNING;
        }

        public override bool ShowInUI()
        {
            return false;
        }
    }
}
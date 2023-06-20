using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.LandingLegs
{
    internal class EmptyLaunchPadConditions: LaunchPadConditions
    {
        public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
        {
            if (conditionType != ProcessCondition.ProcessConditionType.RocketStorage)
            {
                return null;
            }

            return conditions;
        }

        public override void OnSpawn()
        {
            conditions = new List<ProcessCondition>();
            //conditions.Add(new EmptyCondition(base.gameObject));
        }
    }
}

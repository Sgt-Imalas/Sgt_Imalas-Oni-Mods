using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.LandingLegs
{
    public class EmptyLaunchPadConditions : LaunchPadConditions, IProcessConditionSet
    {
        public new List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
        {
            return new List<ProcessCondition>();
        }
        public override void OnSpawn()
        {
        }
    }
}

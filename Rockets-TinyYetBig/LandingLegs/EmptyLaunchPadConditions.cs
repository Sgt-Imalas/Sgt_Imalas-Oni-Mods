using System.Collections.Generic;

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

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules.ModuleConditions
{
	internal class CustomConditionHasResource : ConditionHasResource
	{
		[SerializeField] private bool displayWhenReady = true;
		[SerializeField] private Status failureStatus = Status.Warning;


		public CustomConditionHasResource(Storage storage, SimHashes resource, float thresholdMass, Status failureStatus, bool displayWhenReady) : base(storage, resource, thresholdMass)
		{
			this.displayWhenReady = displayWhenReady;
			this.failureStatus = failureStatus;
		}

		public override Status EvaluateCondition()
		{
			bool ready = base.EvaluateCondition() == Status.Ready;
			return ready ? Status.Ready : failureStatus;
		}

		public override bool ShowInUI() => displayWhenReady || EvaluateCondition() != Status.Ready;
	}
}

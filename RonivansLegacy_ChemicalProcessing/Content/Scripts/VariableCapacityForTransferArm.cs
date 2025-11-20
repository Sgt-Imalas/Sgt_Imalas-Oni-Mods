using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
    class VariableCapacityForTransferArm : KMonoBehaviour
	{
		[SerializeField] public float TargetCarryCapacity = 1000; //1000 is vanilla arm default

		[SerializeField] public float TargetTurnRate = 360; //360 is vanilla arm default, rotation speed per second

		public override void OnPrefabInit()
		{
			var sweeper = gameObject.GetComponent<SolidTransferArm>();
			sweeper.max_carry_weight = TargetCarryCapacity;
			sweeper.turn_rate = TargetTurnRate;
			gameObject.GetComponent<KBatchedAnimController>().PlaySpeedMultiplier = TargetTurnRate / 360f; //Adjust the animation speed to match the turn rate
			base.OnPrefabInit();
		}
	}
}

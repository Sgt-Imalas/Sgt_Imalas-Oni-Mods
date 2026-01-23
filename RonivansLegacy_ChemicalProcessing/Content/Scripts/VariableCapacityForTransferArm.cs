using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class VariableCapacityForTransferArm : KMonoBehaviour
	{
		[SerializeField] public float TargetCarryCapacity = 1000; //1000 is vanilla arm default

		[SerializeField] public float TargetTurnRate = 360; //360 is vanilla arm default, rotation speed per second

		[SerializeField] public float WorkingSpeedMultiplier = -1;
		[MyCmpReq] SolidTransferArm sweeper;


		public override void OnPrefabInit()
		{
			if(WorkingSpeedMultiplier <= 0)
			{
				WorkingSpeedMultiplier = TargetTurnRate / 360f; //set to default if not specified
			}

			sweeper.max_carry_weight = TargetCarryCapacity;
			sweeper.turn_rate = TargetTurnRate;
			gameObject.GetComponent<KBatchedAnimController>().PlaySpeedMultiplier = WorkingSpeedMultiplier; //Adjust the animation speed to match the turn rate
			base.OnPrefabInit();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			StartCoroutine(DelayedArmSpeedSetter());
		}
		/// <summary>
		/// set the play speed of the fetch and deliver animation after a frame delay to ensure the arm anim controller is initialized
		/// </summary>
		/// <returns></returns>
		IEnumerator DelayedArmSpeedSetter()
		{
			yield return null;
			sweeper.arm_anim_ctrl?.PlaySpeedMultiplier = WorkingSpeedMultiplier;
		}
		public float GetWorkSpeedMultiplier()
		{
			return WorkingSpeedMultiplier;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
    class VariableCapacityTransferArm : SolidTransferArm
	{
		[SerializeField] public float TargetCarryCapacity = 1000; //1000 is vanilla arm default

		public override void OnPrefabInit()
		{
			this.max_carry_weight = TargetCarryCapacity;
			base.OnPrefabInit();
		}
	}
}

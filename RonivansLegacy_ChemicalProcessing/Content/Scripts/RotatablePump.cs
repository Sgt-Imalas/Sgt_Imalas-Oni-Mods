using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class RotatablePump : KMonoBehaviour
	{
		[MyCmpReq] ElementConsumer pump;
		[MyCmpReq] Building building;

		[SerializeField]
		public CellOffset PumpOffset = new(0,0);	
		public override void OnSpawn()
		{
			base.OnSpawn();
			pump.sampleCellOffset = building.GetRotatedOffset(PumpOffset).ToVector3();
		}
	}
}

using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class RotatablePump : FilterablePump
	{
		[MyCmpReq] Building building;

		[SerializeField]
		public CellOffset PumpOffset = new(0,0);	
		public override void OnSpawn()
		{
			base.OnSpawn();
			consumer.sampleCellOffset = building.GetRotatedOffset(PumpOffset).ToVector3();
		}
	}
}

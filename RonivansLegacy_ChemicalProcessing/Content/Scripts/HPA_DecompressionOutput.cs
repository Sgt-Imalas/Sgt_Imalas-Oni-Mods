using PeterHan.PLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class HPA_DecompressionOutput : KMonoBehaviour
	{
		[MyCmpReq] public ConduitDispenser ConduitDispenser;
		[MyCmpReq] public Building building;

		public override void OnSpawn()
		{
			base.OnSpawn();
			HighPressureConduitRegistration.RegisterDecompressionValve(this);
		}
		public override void OnCleanUp()
		{
			HighPressureConduitRegistration.UnregisterDecompressionValve(this);
			base.OnCleanUp();
		}
	}
}

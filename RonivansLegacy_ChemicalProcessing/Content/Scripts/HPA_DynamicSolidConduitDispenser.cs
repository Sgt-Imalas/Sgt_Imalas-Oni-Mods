using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	/// <summary>
	/// SolidConduitDispensers with this addon cmp 
	/// </summary>
	internal class HPA_DynamicSolidConduitDispenser : KMonoBehaviour
	{
		[MyCmpGet] SolidConduitDispenser scd;
		public override void OnPrefabInit()
		{
			HighPressureConduitRegistration.RegisterDynamicSolidConduitDispenser(this);
			base.OnPrefabInit();
		}
		public override void OnCleanUp()
		{
			HighPressureConduitRegistration.UnregisterDynamicSolidConduitDispenser(this);
			base.OnCleanUp();
		}
	}
}

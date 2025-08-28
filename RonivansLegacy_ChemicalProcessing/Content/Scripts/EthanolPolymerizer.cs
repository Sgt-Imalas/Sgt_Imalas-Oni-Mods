using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class EthanolPolymerizer : Polymerizer
	{
		internal void UpdateEthanolMeter()
		{
			float num = storage.GetAmountAvailable(SimHashes.Ethanol.CreateTag());
			oilMeter.SetPositionPercent(Mathf.Clamp01(num / consumer.capacityKG));
		}
	}
}

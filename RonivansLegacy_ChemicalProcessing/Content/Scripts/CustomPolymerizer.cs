using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class CustomPolymerizer : Polymerizer
	{
		public override void OnSpawn()
		{
			base.OnSpawn();
			UpdateCustomMeter();
		}

		[SerializeField] public Tag OilElementTag;
		internal void UpdateCustomMeter()
		{
			float num = storage.GetAmountAvailable(OilElementTag);
			oilMeter.SetPositionPercent(Mathf.Clamp01(num / consumer.capacityKG));
		}
	}
}

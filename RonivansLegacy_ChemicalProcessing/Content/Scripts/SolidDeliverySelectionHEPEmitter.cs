using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.UI.USERMENUACTIONS;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class SolidDeliverySelectionHEPEmitter : SolidDeliverySelection
	{
		[MyCmpReq] RadiationEmitter radiationEmitter;
		[MyCmpReq] ElementConverter converter;
		[MyCmpReq] Operational operational;

		const float DefaultEmission = 900f;

		static Dictionary<Tag, float> RadEmissions = new()
		{
			{ SimHashes.UraniumOre.CreateTag(), DefaultEmission },
			{ SimHashes.EnrichedUranium.CreateTag(), 4800 },
		};

		public override void OnSpawn()
		{
			base.OnSpawn();
			Options = RadEmissions.Keys.ToList();
		}
		protected override void OverrideDeliveryRequest()
		{
			base.OverrideDeliveryRequest();
			if (RadEmissions.TryGetValue(manualDelivery.RequestedItemTag, out float rads))
			{
				SgtLogger.l("Setting Emission to " + rads + " for " + manualDelivery.RequestedItemTag);
				SetEmissionRads(rads);
				SetEmissionTemperature(rads);
			}
			else
			{
				SgtLogger.l("Setting Emission to Default for " + manualDelivery.RequestedItemTag);
				SetEmissionRads(DefaultEmission);
				SetEmissionTemperature(DefaultEmission);
			}
		}

		void SetEmissionRads(float rads)
		{
			radiationEmitter.emitRads = rads;
			radiationEmitter.Refresh();
			radiationEmitter.SetEmitting(operational.IsActive);
		}
		void SetEmissionTemperature(float rads)
		{
			converter.outputElements[0].minOutputTemperature = UtilMethods.GetKelvinFromC(30 + (rads / 20));
		}
	}
}

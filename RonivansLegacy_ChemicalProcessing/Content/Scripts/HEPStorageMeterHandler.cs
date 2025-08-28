using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class HEPStorageMeterHandler : KMonoBehaviour
	{

		[MyCmpReq]
		HighEnergyParticleStorage hepStorage;
		[MyCmpReq]
		KBatchedAnimController kbac;
		MeterController hepMeter;
		public override void OnSpawn()
		{
			base.OnSpawn();
			hepMeter = new MeterController(kbac, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, ["meter_target", "meter_fill", "meter_frame", "meter_OL"]);			
			Subscribe((int)GameHashes.OnParticleStorageChanged, OnParticleStorageChanged);
			OnParticleStorageChanged(null);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.OnParticleStorageChanged, OnParticleStorageChanged);
			base.OnCleanUp();
		}
		void OnParticleStorageChanged(object data)
		{
			hepMeter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());
		}
	}
}

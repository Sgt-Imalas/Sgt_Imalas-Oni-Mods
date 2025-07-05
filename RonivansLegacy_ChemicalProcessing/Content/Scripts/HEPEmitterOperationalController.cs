using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class HEPEmitterOperationalController : KMonoBehaviour
	{
		[MyCmpReq] Operational operational;
		[MyCmpReq] RadiationEmitter hepEmitter;

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.OperationalChanged, OnOperationalChanged);
			Subscribe((int)GameHashes.ActiveChanged, OnOperationalChanged);
			OnOperationalChanged(null);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.OperationalChanged, OnOperationalChanged);
			Unsubscribe((int)GameHashes.ActiveChanged, OnOperationalChanged);
		}
		void OnOperationalChanged(object data)
		{
			if (operational.IsActive)
			{
				hepEmitter.SetEmitting(true);
			}
			else
			{
				hepEmitter.SetEmitting(false);
			}
		}
	}
}

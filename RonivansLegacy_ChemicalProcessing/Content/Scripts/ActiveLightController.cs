using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class ActiveLightController : KMonoBehaviour
	{
		[MyCmpReq] Operational operational;
		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.ActiveChanged, OnActiveChanged);
			OnActiveChanged(null);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.ActiveChanged, OnActiveChanged);
			base.OnCleanUp();

		}
		void OnActiveChanged(object sender)
		{
			bool enable = operational.IsActive;
			var lights = GetComponents<Light2D>();
			foreach (Light2D light in lights)
			{
				light.enabled = enable;
			}

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.Elements
{
	class BottleFlipper : KMonoBehaviour
	{
		[MyCmpGet] KBatchedAnimController kbac;
		public override void OnSpawn()
		{
			base.OnSpawn();
			kbac.flipY = true;
		}
	}
}

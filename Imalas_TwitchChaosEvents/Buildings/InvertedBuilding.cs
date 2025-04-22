using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Buildings
{
	class InvertedBuilding : KMonoBehaviour
	{
		[MyCmpReq] KBatchedAnimController kbac;
		[SerializeField] public float xOffset = 0.5f;
		[SerializeField] public float yOffset = 0;

		public override void OnSpawn()
		{
			kbac.flipY = true;
			kbac.offset = new UnityEngine.Vector3(xOffset, yOffset);
			base.OnSpawn();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class ColliderOffsetHandler : KMonoBehaviour
	{
		[MyCmpReq] KBoxCollider2D collider;
		[SerializeField]
		public int ColliderOffsetY = 0, ColliderOffsetX = 0;
		public override void OnSpawn()
		{
			base.OnSpawn();
			collider.offset = collider.offset + new Vector2(ColliderOffsetX, ColliderOffsetY);
		}
	}
}

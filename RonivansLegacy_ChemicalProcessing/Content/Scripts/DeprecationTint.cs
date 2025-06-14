using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	/// <summary>
	/// Tints buildings bright red to indicate deprecation
	/// </summary>
	class DeprecationTint : KMonoBehaviour
	{
		[MyCmpReq] KBatchedAnimController kbac;
		[SerializeField] public Color Tint = Color.red;

		public override void OnSpawn()
		{
			base.OnSpawn();
			kbac.TintColour = Tint;
		}
	}
}

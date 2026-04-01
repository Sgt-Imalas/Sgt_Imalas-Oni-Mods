using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class ToxicityEmitter : KMonoBehaviour, ISim1000ms
	{
		[SerializeField] public float ToxicityEmissionRate = 1f; // Amount of toxicity emitted per second

		public void Sim1000ms(float dt)
		{
			EmitToxicity(dt);

		}

		private void EmitToxicity(float dt)
		{
			int cell = Grid.PosToCell(this);
			ToxicityGrid.EmitToxicity(cell, dt * ToxicityEmissionRate);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class LightReactor : Reactor
	{
		[SerializeField]//normal reactor has 2400
		public float emissionRads = 105f;

		public float GetEmissionRads(bool inMeltdown) => inMeltdown ? emissionRads * 2f : emissionRads; //mirrors game behavior
		public double GetRadGermMultiplierRads(double baseAmount) => baseAmount * 10;

		public override void OnSpawn()
		{
			dumpOffset = new Vector3(0.5f, -1f, 0.0f);
			int ownPos = Grid.PosToCell(this);
			ventCells = [Grid.OffsetCell(ownPos, new (0, -1)),Grid.OffsetCell(ownPos, new(1,-1))];

			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}
	}
}

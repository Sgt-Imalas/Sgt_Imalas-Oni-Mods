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

		[SerializeField]
		public List<CellOffset> OffsetCells = [new(0,-1),new(1,-1)];
		[SerializeField]
		public Vector3 dumpOffsetOverride = new Vector3(0.5f, -1f, 0.0f);

		[MyCmpGet] ConduitConsumer consumer;
		public float MaxCoolantCapacity => consumer.capacityKG;
		public float GetEmissionRads(bool inMeltdown) => inMeltdown ? emissionRads * 2f : emissionRads; //mirrors game behavior
		public double GetRadGermMultiplierRads(double baseAmount) => baseAmount * 10;

		public override void OnSpawn()
		{
			dumpOffset = dumpOffsetOverride;
			int ownPos = Grid.PosToCell(this);
			ventCells = OffsetCells.Select(offset => Grid.OffsetCell(ownPos, offset)).ToArray();

			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}
	}
}

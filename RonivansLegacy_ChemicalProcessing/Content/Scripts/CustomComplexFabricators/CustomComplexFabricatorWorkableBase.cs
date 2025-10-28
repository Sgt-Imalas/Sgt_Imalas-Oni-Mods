using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.DUPLICANTS.DISEASES;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class CustomComplexFabricatorWorkableBase : ComplexFabricatorWorkable
	{
		IOnWorkTickActionProvider[] additionalWorkTickActions;
		public override void OnSpawn()
		{
			base.OnSpawn();
			additionalWorkTickActions = GetComponents<IOnWorkTickActionProvider>();
		}

		public override bool OnWorkTick(WorkerBase worker, float dt)
		{
			foreach (var provider in additionalWorkTickActions)
			{
				provider.OnWorkTick(worker, dt);
			}
			return base.OnWorkTick(worker, dt);
		}
	}
}

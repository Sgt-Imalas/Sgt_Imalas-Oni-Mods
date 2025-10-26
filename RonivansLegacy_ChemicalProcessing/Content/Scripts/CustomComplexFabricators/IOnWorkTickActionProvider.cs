using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal interface IOnWorkTickActionProvider
	{
		public void OnWorkTick(WorkerBase worker, float dt);
	}
}

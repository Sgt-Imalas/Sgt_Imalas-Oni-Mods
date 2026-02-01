using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class TutorialOxygenGenerator : KMonoBehaviour
	{
		public override void OnSpawn()
		{
			base.OnSpawn();
			Tutorial.Instance.oxygenGenerators.Add(this.gameObject);
		}
		public override void OnCleanUp()
		{
			Tutorial.Instance.oxygenGenerators.Remove(this.gameObject);
			base.OnCleanUp();
		}
	}
}

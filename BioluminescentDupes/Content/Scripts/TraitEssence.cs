using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioluminescentDupes.Content.Scripts
{
	internal class TraitEssence: SelfImprovement
	{

		public static List<string> GlowTraits = [nameof(GlowStick), nameof(BD_Bioluminescence)];

		public override bool CanUse(MinionIdentity minionIdentity)
		{
			var traits = minionIdentity.GetComponent<Traits>().GetTraitIds();
			return !traits.Any(t => GlowTraits.Contains(t));
		}
		public override void OnUse(WorkerBase worker)
		{
			var traits = worker.GetComponent<Traits>();

			var traitIds = traits.GetTraitIds();
			if (!traitIds.Any(t => GlowTraits.Contains(t)))
			{
				var toAdd = Db.Get().traits.Get(BD_Bioluminescence.ID);
				traits.Add(toAdd);
			}
		}
	}
}

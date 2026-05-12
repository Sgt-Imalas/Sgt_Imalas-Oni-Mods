using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.DUPLICANTS.DISEASES;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class CustomComplexFabricatorWorkableBase : ComplexFabricatorWorkable
	{
		IOnWorkTickActionProvider[] additionalWorkTickActions;
		IModifiesEfficiencyMultiplier[] additionalEfficiencyMultiplierChangers_Additive;
		IModifiesEfficiencyMultiplier[] additionalEfficiencyMultiplierChangers_Multiplicative;
		public override void OnSpawn()
		{
			base.OnSpawn();
			additionalWorkTickActions = GetComponents<IOnWorkTickActionProvider>();
			var changers = GetComponents<IModifiesEfficiencyMultiplier>();
			additionalEfficiencyMultiplierChangers_Additive = changers.Where(e => !e.Multiplicative).ToArray();
			additionalEfficiencyMultiplierChangers_Multiplicative = changers.Where(e => e.Multiplicative).ToArray();

		}
		public override float GetEfficiencyMultiplier(WorkerBase worker)
		{
			float baseMultiplier = base.GetEfficiencyMultiplier(worker);
			foreach (var changer in additionalEfficiencyMultiplierChangers_Additive)
			{
				baseMultiplier = changer.ApplyEfficiencyModifierChanges(baseMultiplier);
			}
			foreach (var changer in additionalEfficiencyMultiplierChangers_Multiplicative)
			{
				baseMultiplier = changer.ApplyEfficiencyModifierChanges(baseMultiplier);
			}
			return baseMultiplier;
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

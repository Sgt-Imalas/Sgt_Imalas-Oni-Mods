using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace BioluminescentDupes.Content.Scripts
{
	internal class BD_Bioluminescence : StateMachineComponent<BD_Bioluminescence.StatesInstance>
	{
		public static string ID = nameof(BD_Bioluminescence);

		public static DUPLICANTSTATS.TraitVal GetTrait()
		{
			return new DUPLICANTSTATS.TraitVal()
			{
				id = ID,
				statBonus = DUPLICANTSTATS.NO_STATPOINT_BONUS,
				rarity = DUPLICANTSTATS.RARITY_EPIC,
				dlcId = DlcManager.VANILLA_ID,
				mutuallyExclusiveTraits =[nameof(GlowStick)]
			};
		}

		public static bool HasAffectingTrait(GameObject duplicant)
		{
			if (duplicant == null || !duplicant.TryGetComponent<Klei.AI.Traits>(out var traits))
				return false;

			bool hasTrait = traits.HasTrait(ID);
			return hasTrait;
		}
		public override void OnSpawn() {
			this.smi.StartSM();
		}
			
		public class StatesInstance :
		  GameStateMachine<BD_Bioluminescence.States, BD_Bioluminescence.StatesInstance, BD_Bioluminescence, object>.GameInstance
		{
			public AttributeModifier luminescenceModifier;

			public StatesInstance(BD_Bioluminescence master)
			  : base(master)
			{
				this.luminescenceModifier = new AttributeModifier(Db.Get().Attributes.Luminescence.Id, TUNING.TRAITS.GLOWSTICK_LUX_VALUE, STRINGS.DUPLICANTS.TRAITS.BD_BIOLUMINESCENCE.NAME);
			}
		}

		public class States : GameStateMachine<BD_Bioluminescence.States, BD_Bioluminescence.StatesInstance, BD_Bioluminescence>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.root;
				this.root
					.ToggleAttributeModifier("Luminescence Modifier", (smi => smi.luminescenceModifier));
			}
		}
	}
}

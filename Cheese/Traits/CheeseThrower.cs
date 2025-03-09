using Cheese.ModElements;
using KSerialization;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.YeetUtils;

namespace Cheese.Traits
{
	/// <summary>
	/// Credit: Akis Beached, PlushieGifter
	/// </summary>
	public class CheeseThrower : GameStateMachine<CheeseThrower, CheeseThrower.Instance>
	{
		public static DUPLICANTSTATS.TraitVal GetTrait()
		{
			return new DUPLICANTSTATS.TraitVal()
			{
				id = ID
			};
		}

		public FloatParameter CheeseThrownMass;
		public static string ID = "CheeseThrower";
		public static float TIME_PER_CHEESE = 120f;
		public Signal doneStickerBomb;
		public State neutral;
		public OverjoyedStates overjoyed;



		public override void InitializeStates(out BaseState default_state)
		{
			default_state = neutral;
			root
				.TagTransition(GameTags.Dead, null);
			neutral
				.TagTransition(GameTags.Overjoyed, overjoyed);
			overjoyed
				.TagTransition(GameTags.Overjoyed, neutral, true)
				.DefaultState(overjoyed.idle)
				.ParamTransition(CheeseThrownMass, overjoyed.exitEarly, EnoughCheeseMade)
				.Exit(ResetRemainingCheeseNumber)
				;
			overjoyed.idle
				.EnterTransition(overjoyed.throw_cheese, IsRecTime)
				.ToggleStatusItem(Db.Get().DuplicantStatusItems.BalloonArtistPlanning)
				.EventTransition(GameHashes.ScheduleBlocksChanged, overjoyed.throw_cheese, IsRecTime);
			overjoyed.throw_cheese
				.ToggleStatusItem(Db.Get().DuplicantStatusItems.BalloonArtistHandingOut)
				.EventTransition(GameHashes.ScheduleBlocksChanged, overjoyed.idle, Not(IsRecTime))
				.ToggleChore(smi => new CheeseThrowerChore(smi.master), overjoyed.idle);

			overjoyed.exitEarly
				.Enter(ExitJoyReactionEarly);
		}
		private void ResetRemainingCheeseNumber(Instance smi)
		{
			CheeseThrownMass.Set(0, smi);
		}
		private bool EnoughCheeseMade(Instance smi, float num) => num > 75f;
		public void ExitJoyReactionEarly(Instance smi)
		{
			var joyBehaviourMonitor = smi.master.gameObject.GetSMI<JoyBehaviourMonitor.Instance>();
			joyBehaviourMonitor.sm.exitEarly.Trigger(joyBehaviourMonitor);
		}
		public bool IsRecTime(Instance smi) => smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);

		public class OverjoyedStates :
		  State
		{
			public State idle;
			public State throw_cheese;
			public State exitEarly;
		}

		public new class Instance :
		  GameInstance
		{
			[Serialize]
			public float nextCheese;

			public Instance(IStateMachineTarget master)
			  : base(master)
			{
			}
			public void YeetCheese(int cell)
			{
				float amount = UnityEngine.Random.Range(5, 15);

				//TODO: cheese diseese
				GameObject go = ElementLoader.FindElementByHash(ModElementRegistration.Cheese).substance.SpawnResource(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore), amount, UtilMethods.GetKelvinFromC(12f), byte.MaxValue, 0, false);
				go.SetActive(true);
				YeetHelper.YeetRandomly(go, true, 2, 3, true);
				var prevMassYote = sm.CheeseThrownMass.Get(this);
				prevMassYote += amount;
				sm.CheeseThrownMass.Set(prevMassYote, this);

				SoundUtils.PlaySound(ModAssets.SOUNDS.CHEESE, SoundUtils.GetSFXVolume() * 1.0f, attached: smi.gameObject);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace BioluminescentDupes.Content.Scripts
{
	/// <summary>
	/// Credit: Akis Bio Inks with his permission https://github.com/aki-art/ONI-Mods/tree/master/PrintingPodRecharge
	/// </summary>
	public class SelfImprovementChore : Chore<SelfImprovementChore.StatesInstance>
	{
		public SelfImprovementChore(IStateMachineTarget book) : base(
			Db.Get().ChoreTypes.GeneShuffle,
			book,
			null)
		{
			smi = new StatesInstance(this);
			smi.sm.readableSource.Set(book.gameObject, smi);
			smi.sm.requestedUnits.Set(1f, smi);

			showAvailabilityInHoverText = false;

			Prioritizable.AddRef(book.gameObject);

			Game.Instance.Trigger((int)GameHashes.UIRefresh, book.gameObject);

			AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, book.GetComponent<Assignable>());
			AddPrecondition(ChorePreconditions.instance.CanPickup, book.GetComponent<Pickupable>());
		}

		public override void Begin(Precondition.Context context)
		{
			if (context.consumerState.consumer == null || smi == null || smi.sm == null || smi.sm.readableSource == null)
			{
				SgtLogger.error("GetRerolledChore null");
				return;
			}

			smi.sm.reader.Set(context.consumerState.gameObject, smi);
			base.Begin(context);
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, SelfImprovementChore, object>.GameInstance
		{
			public StatesInstance(SelfImprovementChore master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, SelfImprovementChore>
		{
			public TargetParameter reader;
			public TargetParameter readableSource;
			public TargetParameter bookResult;
			public FloatParameter requestedUnits;
			public FloatParameter actualUnits;
			public FetchSubState fetch;
			public State useItem;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = fetch;
				Target(reader);
				var state = fetch.InitializeStates(reader, readableSource, bookResult, requestedUnits, actualUnits, useItem);

				root
					.DoNothing();

				useItem
					.ToggleWork<SelfImprovementWorkable2>(bookResult, null, null, null);
			}
		}
	}
}

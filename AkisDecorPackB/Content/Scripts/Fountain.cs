using AkisDecorPackB.Content.ModDb;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.ELEMENTAL;

namespace AkisDecorPackB.Content.Scripts
{
	public class Fountain : StateMachineComponent<Fountain.SMInstance>, ISaveLoadable
	{
		protected const float DRIP_KG_PER_SECOND = 0.5f;

		[MyCmpReq] private readonly KBatchedAnimController kbac;
		[MyCmpReq] private readonly Sculpture artable;
		[MyCmpReq] private readonly Operational operational;

		[SerializeField] public Storage storageIn;
		[SerializeField] public Storage storageOut;

		public override void OnSpawn()
		{
			base.OnSpawn();
			smi.StartSM();

			ConduitConsumer consumer = GetComponent<ConduitConsumer>();
			consumer.storage = storageIn;
			consumer.forceAlwaysSatisfied = true;
			consumer.IsSatisfied = true;

			if (storageIn.items.Any() && storageIn.items.First().TryGetComponent<PrimaryElement>(out var primaryElement))
			{
				kbac.SetSymbolTint("stream", primaryElement.Element.substance.colour);
			}
		}

		public class States : GameStateMachine<States, SMInstance, Fountain>
		{
			public State unsculpted;
			public State ready;
			public State flowing;

			public Signal dryUp;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = unsculpted;

				unsculpted
					.Enter(smi => SetConduits(smi, false))
					.PlayAnim(smi => smi.GetAnimName(smi, false), KAnim.PlayMode.Paused)
					.EventTransition(GameHashes.WorkableCompleteWork, ready, IsSculpted);

				ready
					.EventTransition(ModAssets.Hashes.FossilStageUnset, unsculpted, IsUnSculpted)
					.Enter(smi => SetConduits(smi, true))
					.PlayAnim(smi => smi.GetAnimName(smi, false), KAnim.PlayMode.Paused)
					.EventTransition(GameHashes.OnStorageChange, flowing, CanFlow)
					.EventTransition(GameHashes.FunctionalChanged, flowing, CanFlow)
					.ToggleStatusItem(ModStatusItems.fountainDriedOut); 

				flowing
					.EventTransition(ModAssets.Hashes.FossilStageUnset, unsculpted, IsUnSculpted)
					.Enter(smi => SetConduits(smi, true))
					.Enter(smi => smi.ForceColorRefresh())
					.PlayAnim(smi => smi.GetAnimName(smi, true), KAnim.PlayMode.Loop)
					.ToggleAttributeModifier("FountainDecor", smi => smi.modifier)
					.Update((smi, dt) => smi.DripLiquid(smi, dt))
					.OnSignal(dryUp, ready)
					.EventTransition(GameHashes.FunctionalChanged, ready, smi => !smi.GetComponent<Operational>().IsFunctional)
					.ToggleStatusItem(ModStatusItems.fountainFlowing);
			}

			private bool CanFlow(SMInstance smi)
			{
				return smi.master.operational.IsFunctional && !smi.master.storageIn.IsEmpty();
			}

			private bool IsSculpted(SMInstance smi) => smi.master.artable.CurrentStage != "Default";
			private bool IsUnSculpted(SMInstance smi) => smi.master.artable.CurrentStage == "Default";

			private static void SetConduits(SMInstance smi, bool on)
			{
				smi.GetComponent<ConduitDispenser>().SetOnState(on);
				ConduitConsumer consumer = smi.GetComponent<ConduitConsumer>();
				consumer.SetOnState(on);
				consumer.forceAlwaysSatisfied = !on;
				if (!on)
				{
					consumer.IsSatisfied = true;
				}
			}
		}


		public class SMInstance : GameStateMachine<States, SMInstance, Fountain, object>.GameInstance
		{
			public AttributeModifier modifier;

			private readonly Queue<float> samples;

			private SimHashes lastElement = SimHashes.Void;
			private bool colorDirty = false;

			public SMInstance(Fountain master) : base(master)
			{
				samples = new Queue<float>(3);
				samples.Append(0);
				samples.Append(0);
				samples.Append(0);

				modifier = new AttributeModifier(global::Db.Get().BuildingAttributes.Decor.Id, 30, "Fountain", false, false, true);
			}

			public string GetAnimName(SMInstance _, bool on) => on ? "on" : "off";

			internal void DripLiquid(SMInstance smi, float _)
			{
				Fountain fountain = smi.master;

				if (fountain.storageIn.IsEmpty())
				{
					UpdateSamples(smi, 0);
					return;
				}

				GameObject item = fountain.storageIn[0];
				PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();

				RefreshColor(fountain, primaryElement);

				float mass = TransferMass(fountain, item, primaryElement);

				UpdateSamples(smi, mass);
			}

			private void UpdateSamples(SMInstance smi, float mass)
			{
				samples.Enqueue(mass);

				if (samples.Average() <= 0)
					sm.dryUp.Trigger(smi);
			}

			private static float TransferMass(Fountain fountain, GameObject item, PrimaryElement primaryElement)
			{
				float mass = primaryElement.Mass;

				if (mass <= fountain.storageOut.capacityKg)
				{
					// transfer all, otherwise rounding issues with floats will cause issues if it's just transferred by available mass.
					// (the 2 overloads for Storage#Transfer are very different)
					fountain.storageIn.Transfer(fountain.storageOut, false, true);
				}
				else
				{
					mass = fountain.storageOut.capacityKg;
					fountain.storageIn.Transfer(fountain.storageOut, item.PrefabID(), mass, false, true);
				}

				return mass;
			}
			internal void ForceColorRefresh()
			{
				colorDirty = true;
			}

			internal void RefreshColor(Fountain fountain, PrimaryElement primaryElement)
			{
				if ((lastElement != primaryElement.ElementID || colorDirty))
				{
					fountain.kbac.SetSymbolTint("stream", primaryElement.Element.substance.colour);
					lastElement = primaryElement.ElementID;
					colorDirty = false;
				}
			}
		}
	}
}

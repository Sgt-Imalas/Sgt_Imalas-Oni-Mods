using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicSatellites.Behaviours
{
	public class BuildingInternalConstructorRocket : GameStateMachine<BuildingInternalConstructorRocket, BuildingInternalConstructorRocket.Instance, IStateMachineTarget, BuildingInternalConstructorRocket.Def>
	{
		public GameStateMachine<BuildingInternalConstructorRocket, BuildingInternalConstructorRocket.Instance, IStateMachineTarget, BuildingInternalConstructorRocket.Def>.State inoperational;
		public BuildingInternalConstructorRocket.OperationalStates operational;
		public StateMachine<BuildingInternalConstructorRocket, BuildingInternalConstructorRocket.Instance, IStateMachineTarget, BuildingInternalConstructorRocket.Def>.BoolParameter constructionRequested = new StateMachine<BuildingInternalConstructorRocket, BuildingInternalConstructorRocket.Instance, IStateMachineTarget, BuildingInternalConstructorRocket.Def>.BoolParameter(true);

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = (StateMachine.BaseState)this.inoperational;
			this.serializable = StateMachine.SerializeType.ParamsOnly;

			this.inoperational
				.EventTransition(GameHashes.OperationalChanged, this.operational, (smi => smi.GetComponent<Operational>().IsOperational))
				.Enter((smi => smi.ShowConstructionSymbol(false)));

			this.operational
				.DefaultState(this.operational.constructionRequired)
				.EventTransition(GameHashes.OperationalChanged, this.inoperational, (smi => !smi.GetComponent<Operational>().IsOperational));

			this.operational.constructionRequired
				.EventTransition(GameHashes.OnStorageChange, this.operational.constructionHappening, (smi => smi.GetMassForConstruction() != null))
				.EventTransition(GameHashes.OnStorageChange, this.operational.constructionSatisfied, (smi => smi.HasOutputInStorage()))
				.ToggleFetch((smi => smi.CreateFetchList()), this.operational.constructionHappening)
				.ParamTransition<bool>(this.constructionRequested, this.operational.constructionSatisfied, IsFalse)
				.Enter((smi => smi.ShowConstructionSymbol(true)))
				.Exit((smi => smi.ShowConstructionSymbol(false)));

			this.operational.constructionHappening
				.EventTransition(GameHashes.OnStorageChange, this.operational.constructionSatisfied, (smi => smi.HasOutputInStorage()))
				.EventTransition(GameHashes.OnStorageChange, this.operational.constructionRequired, (smi => smi.GetMassForConstruction() == null))
				.ToggleChore((smi => (Chore)smi.CreateWorkChore()), this.operational.constructionHappening, this.operational.constructionHappening)
				.ParamTransition<bool>(this.constructionRequested, this.operational.constructionSatisfied, IsFalse)
				.Enter((smi => smi.ShowConstructionSymbol(true)))
				.Exit((smi => smi.ShowConstructionSymbol(false)));

			this.operational.constructionSatisfied
				.EventTransition(GameHashes.OnStorageChange, this.operational.constructionRequired, (smi => !smi.HasOutputInStorage() && this.constructionRequested.Get(smi)))
				.ParamTransition<bool>(this.constructionRequested, this.operational.constructionRequired, ((smi, p) => p && !smi.HasOutputInStorage()));
		}

		public class Def : StateMachine.BaseDef
		{
			public DefComponent<Storage> storage;
			public float constructionMass;
			public List<Tag> outputIDs;
			public bool spawnIntoStorage;
			public string constructionSymbol;
			public Tag ConstructionMaterial;
		}

		public class OperationalStates :
		  GameStateMachine<BuildingInternalConstructorRocket, BuildingInternalConstructorRocket.Instance, IStateMachineTarget, BuildingInternalConstructorRocket.Def>.State
		{
			public State constructionRequired;
			public State constructionHappening;
			public State constructionSatisfied;
		}

		public new class Instance :
		  GameStateMachine<BuildingInternalConstructorRocket, BuildingInternalConstructorRocket.Instance, IStateMachineTarget, BuildingInternalConstructorRocket.Def>.GameInstance, ISidescreenButtonControl

		{
			[Serialize]
			private Storage storage;
			[Serialize]
			private float constructionElapsed;
			private Tag constructionMaterial;
			private ProgressBar progressBar;

			public Instance(IStateMachineTarget master, BuildingInternalConstructorRocket.Def def)
			  : base(master, def)
			{
				this.constructionMaterial = def.ConstructionMaterial;
				this.storage = def.storage.Get((StateMachine.Instance)this);
				this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition)new SatelliteConstructionCompleteCondition(this));
			}

			private void DropConstructionUnits(Tag constructionElement, float mass)
			{
				var MassPerUnit = Assets.GetPrefab(constructionElement).GetComponent<PrimaryElement>().MassPerUnit;
				if (mass >= MassPerUnit)
				{
					var constructionPart = GameUtil.KInstantiate(Assets.GetPrefab(constructionElement), gameObject.transform.position, Grid.SceneLayer.Ore);
					constructionPart.SetActive(true);
					var constructionPartElement = constructionPart.GetComponent<PrimaryElement>();
					constructionPartElement.Units = mass / MassPerUnit;
				}
			}


			public override void OnCleanUp()
			{
				Element element = (Element)null;
				float mass = 0.0f;

				foreach (var outputId in this.def.outputIDs)
				{
					GameObject first = this.storage.FindFirst(outputId);
					if ((UnityEngine.Object)first != (UnityEngine.Object)null)
					{
						PrimaryElement component = first.GetComponent<PrimaryElement>();
						Debug.Assert(element == null || element == component.Element);
						element = component.Element;
						mass += component.Mass;
						first.DeleteObject();
					}
				}

				DropConstructionUnits((Tag)def.ConstructionMaterial, mass);
				base.OnCleanUp();
			}

			public FetchList2 CreateFetchList()
			{
				FetchList2 fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.BuildFetch);
				fetchList.Add(constructionMaterial, amount: this.def.constructionMass);
				return fetchList;
			}

			public PrimaryElement GetMassForConstruction() => storage.FindFirstWithMass(def.ConstructionMaterial, this.def.constructionMass);


			public bool HasOutputInStorage() => (bool)this.storage.FindFirst(this.def.outputIDs[0]);
			
			public bool IsRequestingConstruction()
			{
				this.sm.constructionRequested.Get(this);
				return this.smi.sm.constructionRequested.Get(this.smi);
			}

			public void ConstructionComplete(bool force = false)
			{
				if (!force)
				{
					PrimaryElement massForConstruction = this.GetMassForConstruction();
					///klei broke this so it spawns weird ghost entities.. delete it instead.
					//massForConstruction.TryGetComponent<Pickupable>(out var pickupable);
					//pickupable.Take(def.constructionMass);
					UnityEngine.Object.Destroy(massForConstruction.gameObject);

				}
				foreach (var outputId in this.def.outputIDs)
				{
					GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(outputId), this.transform.GetPosition(), Grid.SceneLayer.Ore);
					go.SetActive(true);
					if (this.def.spawnIntoStorage)
						this.storage.Store(go);

					int type = gameObject.GetSMI<ISatelliteCarrier>().SatelliteType();
					if (type == -1)
						type = 0;
					go.GetComponent<SatelliteTypeHolder>().SatelliteType = type;
				}
				// this.sm.constructionRequested.Set(false,this);
				//var carrierModule = gameObject.GetSMI<ISatelliteCarrier>();
				//carrierModule.SatelliteConstructed();
				//Debug.Log("Type: "+carrierModule.SatelliteType() +", Holding? "+ carrierModule.HoldingSatellite());
			}

			public WorkChore<BuildingInternalConstructorRocketWorkable> CreateWorkChore() => new WorkChore<BuildingInternalConstructorRocketWorkable>(Db.Get().ChoreTypes.Build, this.master);

			public void ShowConstructionSymbol(bool show)
			{
				KBatchedAnimController component = this.master.GetComponent<KBatchedAnimController>();
				if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
					return;
				component.SetSymbolVisiblity((KAnimHashedString)this.def.constructionSymbol, show);
			}
			public string SidescreenButtonText => !this.smi.sm.constructionRequested.Get(this.smi) ? string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR.text, (object)Assets.GetPrefab((Tag)this.def.outputIDs[0]).GetProperName()) : string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR.text, (object)Assets.GetPrefab((Tag)this.def.outputIDs[0]).GetProperName());

			public string SidescreenButtonTooltip => !this.smi.sm.constructionRequested.Get(this.smi) ? string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, (object)Assets.GetPrefab((Tag)this.def.outputIDs[0]).GetProperName()) : string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, (object)Assets.GetPrefab((Tag)this.def.outputIDs[0]).GetProperName());

			public void OnSidescreenButtonPressed()
			{
				this.smi.sm.constructionRequested.Set(!this.smi.sm.constructionRequested.Get(this.smi), this.smi);
				if (!DebugHandler.InstantBuildMode || !this.smi.sm.constructionRequested.Get(this.smi) || this.HasOutputInStorage())
					return;
				this.ConstructionComplete(true);
			}
			public bool SidescreenEnabled() => true;

			public bool SidescreenButtonInteractable() => true;

			public int ButtonSideScreenSortOrder() => 20;
			public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();

			public int HorizontalGroupID()
			{
				return -1;
			}
		}
	}
}

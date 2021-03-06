using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryopod.Buildings
{
    class CryopodReusable : StateMachineComponent<CryopodReusable.StatesInstance>, ISaveLoadable, ISidescreenButtonControl
	//, IGameObjectEffectDescriptor
	{
		[MyCmpReq]
		public Assignable assignable;
		[MyCmpGet]
		private LogicPorts ports;
		[MyCmpGet]
		private Operational operational;
		[MyCmpReq]
		private OpenCryopodWorkable WorkableOpen;
		[MyCmpReq]
		private CryopodFreezeWorkable Workable;
		//[MyCmpReq] protected Operational operational;
		[MyCmpReq] private KSelectable selectable;
		[MyCmpReq] private MinionStorage DupeStorage;
		[Serialize] private float ForceThawed; //amount of damage done on thawing based on forced process (no power f.e.)

		[Serialize] public float storedDupeDamage = -1; //Damage the dupe has recieved prior to storing
		[Serialize] public List<string> StoredSicknessIDs = new List<string>(); //Sicknessses the dupe had prior to storing
		[Serialize] public float InternalTemperatureKelvin;

		public float InternalTemperatureKelvinUpperLimit = 310.15f;
		public float InternalTemperatureKelvinLowerLimit = 77.15f;
		public float TimeForProcess = 60f;
		[Serialize] public CellOffset dropOffset = CellOffset.none;

		private Chore AnimationChore;
		public float GetDamage()
        {
			return ForceThawed;
        }
		internal string GetDupeName()
        {
            if (HoldingDupe())
            {
				return DupeStorage.GetStoredMinionInfo().First().name;
			}
			return "No duplicant stored.";
        }

        
		public void RefreshSideScreen()
		{
			if (!this.GetComponent<KSelectable>().IsSelected)
				return;
			DetailsScreen.Instance.Refresh(this.gameObject);
		}
		public void SetAssignable(bool set_it)
		{
			this.assignable.SetCanBeAssigned(set_it);
			this.RefreshSideScreen();
		}
		public void ClearAssignable()
        {
			this.assignable.Unassign();
		}
		public bool HoldingDupe()
        {
			return DupeStorage.GetStoredMinionInfo().Count>0;
		}

        internal void FreezeChoreDone()
		{
			this.smi.GoTo(this.smi.sm.RecievedDupe);
		}

        #region Spawn&Cleanup
        protected override void OnSpawn()
		{
			base.OnSpawn();
			this.smi.StartSM();
			ModAssets.CryoPods.Add(this); 
		}

		protected override void OnCleanUp()
		{
			ModAssets.CryoPods.Remove(this);
			if (this.HoldingDupe())
			{
				ForceThawed += (InternalTemperatureKelvinUpperLimit- InternalTemperatureKelvin);
				ThrowOutDupe(true);
				AnimationChore = null;
			}
			base.OnCleanUp();
		}

		private void UpdateLogicCircuit()
		{
			bool on = this.HoldingDupe();
			this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, on ? 1 : 0);
		}

		#endregion

		#region SideScreen
		public string SidescreenButtonText => WorkableOpen.ChoreExisting() ? STRINGS.BUILDINGS.PREFABS.CRY_BUILDABLECRYOTANK.DEFROSTBUTTONCANCEL : STRINGS.BUILDINGS.PREFABS.CRY_BUILDABLECRYOTANK.DEFROSTBUTTON;

		public string SidescreenButtonTooltip => STRINGS.BUILDINGS.PREFABS.CRY_BUILDABLECRYOTANK.DEFROSTBUTTONTOOLTIP;

		public bool SidescreenEnabled() => true;

		public bool SidescreenButtonInteractable() => this.HoldingDupe() &&
			!this.smi.IsInsideState(this.smi.sm.HoldingDuplicant.Thawing);

        public void OnSidescreenButtonPressed()
        {
			Debug.Log("DEBUG: CHORE EXISTING: "+WorkableOpen.ChoreExisting());

			if (!WorkableOpen.ChoreExisting())
            {
				WorkableOpen.CreateOpenChore();
			}
            else
            {
				WorkableOpen.CancelOpenChore();
			}
			this.RefreshSideScreen();
		}
		public int ButtonSideScreenSortOrder() => 20;

		#endregion



        #region Freezing&Thawing
		
		private enum TemperatureMode
        {
			Freezing,
			Thawing,
			ForceThawing
        }

		public void OpenChoreDone()
        {

			this.smi.GoTo(this.smi.sm.HoldingDuplicant.Thawing);
			this.RefreshSideScreen();
		}
		
		public void ThrowOutDupe(bool skipAnim = false)
		{
			assignable.Unassign();
			var newDupe = DupeStorage.GetStoredMinionInfo().First();
			var spawn_position = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this.transform.position), this.dropOffset), Grid.SceneLayer.BuildingUse);

			var NewDupeDeserialized = DupeStorage.DeserializeMinion(newDupe.id, spawn_position);
			NewDupeDeserialized.transform.SetLocalPosition(spawn_position);
			this.smi.sm.defrostedDuplicant.Set(NewDupeDeserialized, this.smi);

			var dupeModifiers = NewDupeDeserialized.GetComponent<MinionModifiers>();
			SicknessExposureInfo cold = new SicknessExposureInfo(ColdBrain.ID, "Frozen within self made cryopod.");
			dupeModifiers.sicknesses.Infect(cold);


			foreach (var sickness in StoredSicknessIDs)
            {
				NewDupeDeserialized.GetComponent<MinionModifiers>().sicknesses.Infect(new SicknessExposureInfo(sickness, "Got frozen with the disease"));
			}

			if(storedDupeDamage != -1f)
            {
				NewDupeDeserialized.GetComponent<Health>().Damage(storedDupeDamage);
				storedDupeDamage = -1;
			}

			if (ForceThawed>0)
            {
				HandleCryoDamage(NewDupeDeserialized, ForceThawed);
				//Debug.Log(NewDupeDeserialized + " should have CryoSickness with Hardness "+ForceThawed );
				ForceThawed = 0;
			}
			
			ChoreProvider choreProvider = NewDupeDeserialized.GetComponent<ChoreProvider>();
			Debug.Log(this.transform.GetPosition().z+ " FG-Layer of building");

			if ((UnityEngine.Object)choreProvider != (UnityEngine.Object) null && !skipAnim)
			{
				this.AnimationChore = (Chore)new EmoteChore((IStateMachineTarget)choreProvider, Db.Get().ChoreTypes.EmoteHighPriority, (HashedString)"anim_interacts_cryo_chamber_kanim", new HashedString[2]
					{
						(HashedString) "defrost",
						(HashedString) "defrost_exit"
					}, KAnim.PlayMode.Once);
				Vector3 position = NewDupeDeserialized.transform.GetPosition() with
				{
					z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse)
				};
				NewDupeDeserialized.transform.SetPosition(position);
				
			}

			SetAssignable(true);
			DupeStorage.GetStoredMinionInfo().Clear(); 
			UpdateLogicCircuit();
		}

		private void HandleCryoDamage(GameObject dupe, float dmgVal)
        {
			if (dmgVal > 120f)
			{
				//SicknessExposureInfo zombie = new SicknessExposureInfo(ZombieSickness.ID, "Cryogenic Damage");
				//dupeModifiers.sicknesses.Infect(zombie);
			}
			var helf = dupe.GetComponent<Health>();
			var doDamage = dmgVal / 2 ;

			Effect cryoSickness = new Effect(
				 ModAssets.ForcedCryoThawedID,
				 STRINGS.DUPLICANTS.STATUSITEMS.FORCETHAWED.NAME,
				 STRINGS.DUPLICANTS.STATUSITEMS.FORCETHAWED.NAME,
				 120f,
				 true,
				 true,
				 true);

			var debuffs = new List<AttributeModifier>();
			var debuffStrength = -(dmgVal / 16) <=-1f? -(dmgVal / 16) : -1;
			
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Strength.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Construction.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Machinery.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Caring.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Ranching.Id, debuffStrength));
				//debuffs.Add(new AttributeModifier("AirConsumptionRate", 7.5f));

			if (dmgVal > 80)
			{
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Art.Id, debuffStrength));
				debuffs.Add(new AttributeModifier("SPACENAVIGATION", debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Learning.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Cooking.Id, debuffStrength));
				debuffs.Add(new AttributeModifier(Db.Get().Attributes.Botanist.Id, debuffStrength));
			}
			debuffs.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, (5f + (-debuffStrength)) / 600, STRINGS.DISEASES.CRYOSICKNESS.NAME));
			cryoSickness.duration = (60 * dmgVal);
			cryoSickness.SelfModifiers = debuffs;
			//helf.Damage(doDamage);
			helf.StartCoroutine(this.KillOnEndEditRoutine(helf, doDamage));
			if (helf.hitPoints == 0)
            {
				//helf.Incapacitate(GameTags.HitPointsDepleted);
			}
			Debug.Log("Health State: " + helf.State);
			dupe.GetComponent<Effects>().Add(cryoSickness, true);
		}
		private IEnumerator KillOnEndEditRoutine(Health helf, float dmg)
		{

			yield return (object)new WaitForEndOfFrame();
			yield return (object)new WaitForEndOfFrame();
			helf.Damage(dmg);
		}

		private void StartCoolingProcess()
		{
			UpdateLogicCircuit();
			SetAssignable(false);
			this.InternalTemperatureKelvin = InternalTemperatureKelvinUpperLimit; 
		}


		private void ChangeInternalTemperature(float dt, TemperatureMode cooling)
        {
			float temperatureStep = ((InternalTemperatureKelvinUpperLimit - InternalTemperatureKelvinLowerLimit) / TimeForProcess) * dt;

			if (cooling == TemperatureMode.ForceThawing)
			{
				ForceThawed += temperatureStep;
			}
            else
            {
                if (ForceThawed > 0)
                {
					var recoveryVal = temperatureStep / 3;
					ForceThawed = ForceThawed - recoveryVal < 0f ? 0f : ForceThawed - recoveryVal;
				}
            }
			InternalTemperatureKelvin = cooling==TemperatureMode.Freezing ? InternalTemperatureKelvin - temperatureStep: InternalTemperatureKelvin + temperatureStep;
			if(cooling == TemperatureMode.Freezing && InternalTemperatureKelvin <= InternalTemperatureKelvinLowerLimit || !(cooling == TemperatureMode.Freezing) && InternalTemperatureKelvin >= InternalTemperatureKelvinUpperLimit)
            {
				smi.sm.HasReachedTargetTemp.Set(true, smi);
				InternalTemperatureKelvin = cooling == TemperatureMode.Freezing ? InternalTemperatureKelvinLowerLimit : InternalTemperatureKelvinUpperLimit;

			}
		}

        #endregion
        #region StateMachine

        public class StatesInstance : GameStateMachine<States, StatesInstance, CryopodReusable, object>.GameInstance
		{
			private HandleVector<int>.Handle structureTemperature;
			private float coolingHeatKW = 80.0f;
			private float steadyHeatKW = 0.0f;
			public void ApplyCoolingExhaust(float dt,bool coolingExterior =false) {
                if (coolingExterior)
					GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, (-this.coolingHeatKW*0.98f) * dt, (string)BUILDING.STATUSITEMS.OPERATINGENERGY.FOOD_TRANSFER, dt);
				else
					GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, this.coolingHeatKW * dt, (string)BUILDING.STATUSITEMS.OPERATINGENERGY.FOOD_TRANSFER, dt);
			}
			public void ApplySteadyExhaust(float dt) => GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, this.steadyHeatKW * dt, (string)BUILDING.STATUSITEMS.OPERATINGENERGY.FOOD_TRANSFER, dt);
			public float GetSaverPower() => this.GetComponent<EnergyConsumer>().WattsNeededWhenActive/10;

			public float GetNormalPower() => this.GetComponent<EnergyConsumer>().WattsNeededWhenActive;
			public void SetEnergySaver(int energySaving)
			{
				EnergyConsumer component = this.GetComponent<EnergyConsumer>();
				if (energySaving==1)
					component.BaseWattageRating = this.GetSaverPower();
				else if(energySaving == 0)
					component.BaseWattageRating = this.GetNormalPower();
				else if (energySaving == 2)
					component.BaseWattageRating = 0f;
			}
			public StatesInstance(CryopodReusable master) : base(master)
			{
				this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
			}
		}

		public class States : GameStateMachine<States, StatesInstance, CryopodReusable>
		{
			public TargetParameter defrostedDuplicant;
			[Serialize] public BoolParameter HasReachedTargetTemp;
			[Serialize] public BoolParameter HoldsDuplicant;
			public class HoldingDuplicantStates : State
			{
				public State Cooling;
				public State OnTemperature;
				public State Thawing;
				public ForceStates ForceThawing;
				public State ThrowDupeOut;
				public State ThrowDupeOutPost;
				public class ForceStates : State{
					public State Entombed;
					public State OverHeated;
					public State NoPower;
				}
			}

			public State Init;
			public State Idle;
			public State RecievedDupe;
			public HoldingDuplicantStates HoldingDuplicant;

			public override void InitializeStates(out BaseState defaultState)
			{

				defaultState = Init;

				Init.Enter((smi) => {
					HoldsDuplicant.Set(smi.master.HoldingDupe(), smi);
					if (smi.master.HoldingDupe())
					{
						smi.GoTo(HoldingDuplicant);
					}
					else
					{
						smi.GoTo(Idle);
					}
					smi.master.operational.SetActive(true);
					Debug.Log(smi.master.operational.IsActive);
				})
					//.ToggleStatusItem("State: Init", "")
					;


				Idle
					.Enter(smi => { smi.SetEnergySaver(2);
						smi.sm.defrostedDuplicant.Set(null, smi);
					})
					.Update((smi,dt)=>smi.ApplySteadyExhaust(dt))
					.QueueAnim("off");

				RecievedDupe
					.Enter(smi => HoldsDuplicant.Set(true, smi))
					//.ToggleStatusItem("State: RecievingDupe", "")
					.PlayAnim("working_pre")
					.Exit(smi => smi.master.StartCoolingProcess())
					.GoTo(HoldingDuplicant);

				HoldingDuplicant.defaultState = HoldingDuplicant.Cooling;
				HoldingDuplicant
					.Enter(smi => { smi.master.UpdateLogicCircuit();

						})
					.Exit(smi => smi.master.UpdateLogicCircuit())
					.ToggleStatusItem(ModAssets.StatusItems.DupeName, smi => smi.master)
					.ToggleStatusItem(ModAssets.StatusItems.CurrentDupeTemperature, smi => smi.master);
				HoldingDuplicant.Cooling
					.Enter(smi => smi.SetEnergySaver(0))
					.Exit(smi => smi.SetEnergySaver(1))
					.QueueAnim("working_loop")
					.Update("Freezing", (smi, dt) =>
					{
						smi.ApplyCoolingExhaust(dt, false);
						smi.master.ChangeInternalTemperature(dt, TemperatureMode.Freezing);

					}, UpdateRate.SIM_200ms)
					.ParamTransition<bool>(this.HasReachedTargetTemp, this.HoldingDuplicant.OnTemperature, IsTrue)
					.EventTransition(GameHashes.OperationalChanged, HoldingDuplicant.ForceThawing, smi => !smi.GetComponent<Operational>().IsOperational)
					;

				HoldingDuplicant.OnTemperature
					//.ToggleStatusItem("State: OnTemp.", "")
					.Enter((smi) => HasReachedTargetTemp.Set(false, smi))
					.Update((smi, dt) => smi.ApplySteadyExhaust(dt))
					.ToggleStatusItem(ModAssets.StatusItems.EnergySaverModeCryopod, smi => smi.master)
					.EventTransition(GameHashes.OperationalChanged, HoldingDuplicant.ForceThawing, smi => !smi.GetComponent<Operational>().IsOperational)
					;
				HoldingDuplicant.ForceThawing

					.ToggleStatusItem(ModAssets.StatusItems.CryoDamage, smi => smi.master)
					.Update("ForceThawing", (smi, dt) =>
					{
						smi.ApplyCoolingExhaust(dt, true);
						smi.master.ChangeInternalTemperature(dt, TemperatureMode.ForceThawing);

					}, UpdateRate.SIM_200ms)
					.ParamTransition<bool>(this.HasReachedTargetTemp, this.HoldingDuplicant.ThrowDupeOut, IsTrue)
						.EventTransition(GameHashes.OperationalChanged, HoldingDuplicant.Cooling, smi => smi.GetComponent<Operational>().IsOperational)
					;
				HoldingDuplicant.Thawing
					//.ToggleStatusItem("State: Thawing.", "")
					.Enter(smi => smi.SetEnergySaver(0))
					.Exit(smi => smi.SetEnergySaver(1))
					.Update("Thawing", (smi, dt) =>
					{
						smi.ApplyCoolingExhaust(dt, true);
						smi.master.ChangeInternalTemperature(dt, TemperatureMode.Thawing);

					}, UpdateRate.SIM_200ms)
					.ParamTransition<bool>(this.HasReachedTargetTemp, this.HoldingDuplicant.ThrowDupeOut, IsTrue)
					.EventTransition(GameHashes.OperationalChanged, HoldingDuplicant.ForceThawing, smi => !smi.GetComponent<Operational>().IsOperational)
					;

				HoldingDuplicant.ThrowDupeOut
					.Exit(smi => smi.SetEnergySaver(2))
					.PlayAnim("defrost")
					.OnAnimQueueComplete(HoldingDuplicant.ThrowDupeOutPost)
					.Enter(smi => 
					{
						HasReachedTargetTemp.Set(false, smi);
						smi.master.ThrowOutDupe(); 
						HoldsDuplicant.Set(false, smi);
					})
					.Update((smi, dt) => smi.sm.defrostedDuplicant.Get(smi).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse))
					;

				HoldingDuplicant.ThrowDupeOutPost
					.PlayAnim("defrost_exit")
					.OnAnimQueueComplete(Idle)
					;
			}
		}

        private void CheckForDupe()
        {
            if (HoldingDupe())
			{
				this.smi.GoTo(smi.sm.HoldingDuplicant);
			}
        }

        #endregion
    }
}


using Database;
using HarmonyLib;
using Klei.AI;
using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ResearchTypes;

namespace BawoonFwiend
{
    internal class Bawoongiver :
  StateMachineComponent<Bawoongiver.StatesInstance>,
  IGameObjectEffectDescriptor

    {
        [Serialize]
        public Dictionary<BalloonOverrideSymbol, bool> EnabledBalloonSkins = new Dictionary<BalloonOverrideSymbol, bool>();
        //[Serialize]
        //public bool UseDefaultRedSkin = true;
        [Serialize]
        public bool ToggleAllBtnOn = false;
        [Serialize]
        public bool AllRandom = false;

        [Serialize]
        int currentIndex = -1;
        [Serialize]
        int nextIndex = -1;

        [MyCmpGet]
        Storage storage;

        [MyCmpGet]
        SymbolOverrideController symbolOverrideController;

        List<BalloonOverrideSymbol> ActiveSkinOverrides = new List<BalloonOverrideSymbol>();

        void UpdateActives()
        {
            var currentSkin = ActiveSkinOverrides.Count == 0 || currentIndex == -1 ? default(BalloonOverrideSymbol) :  ActiveSkinOverrides[currentIndex];
            ActiveSkinOverrides.Clear();

            foreach (var skin in EnabledBalloonSkins)
            {
                if(skin.Value)
                {
                    ActiveSkinOverrides.Add(skin.Key);
                }
            }
            currentIndex = ActiveSkinOverrides.Count == 0 ? -1 : currentIndex;
            nextIndex = ActiveSkinOverrides.Count == 0 ? -1 : NextOverrideSymbolInt();
            var current = ActiveSkinOverrides.Count == 0 ? -1 : ActiveSkinOverrides.FindIndex(skin => skin.animFileID == currentSkin.animFileID && skin.animFileSymbolID == currentSkin.animFileSymbolID);
            if (current != -1)
            {
                currentIndex = current;
            }
            SetBalloonSymbolOverride();
        }

        bool GetNextOverrideSymbol()
        {
            if (ActiveSkinOverrides.Count == 0)
            {
                currentIndex = -1;
                nextIndex = -1;
                return false;
            }

            currentIndex = nextIndex;
            nextIndex = NextOverrideSymbolInt();
            return true;
        }

        int NextOverrideSymbolInt()
        {
            if(ActiveSkinOverrides.Count == 0)
                return -1;

            if (AllRandom) 
            {
                return UnityEngine.Random.Range(0, ActiveSkinOverrides.Count - 1);
            }

            return (currentIndex + 1) % (ActiveSkinOverrides.Count);                 
        }

        public BalloonOverrideSymbol CurrentSkin => currentIndex == -1? default(BalloonOverrideSymbol) : ActiveSkinOverrides[currentIndex];
        public BalloonOverrideSymbol NextSkin => nextIndex == -1? default(BalloonOverrideSymbol) : ActiveSkinOverrides[nextIndex];

        public void ApplyNextSkin()
        {
            GetNextOverrideSymbol();
            SetBalloonSymbolOverride();
        }

        public void ToggleSkin(BalloonOverrideSymbol balloonSkinID)
        {
            ToggleAllBtnOn = false;
            EnabledBalloonSkins[balloonSkinID] = !EnabledBalloonSkins[balloonSkinID];
            UpdateActives();
        }
        public void ToggleAll()
        {
            //UseDefaultRedSkin = true;
            ToggleAllBtnOn = !ToggleAllBtnOn;
            var keys = new List<BalloonOverrideSymbol>(EnabledBalloonSkins.Keys);
            foreach (var bloon in keys)
            {
                EnabledBalloonSkins[bloon] = ToggleAllBtnOn;
            }
            UpdateActives();
        }

        public void ToggleFullyRandom() => AllRandom= !AllRandom;

        public void ToggleRandoms()
        {
            ToggleAllBtnOn = false;
            var numberOfOptions = (int)UnityEngine.Random.Range(3, Mathf.Min(8, EnabledBalloonSkins.Count/2));
            numberOfOptions = Mathf.Min(numberOfOptions, EnabledBalloonSkins.Count);

            var keys = new List<BalloonOverrideSymbol>(EnabledBalloonSkins.Keys);
            foreach (var bloon in keys)
            {
                EnabledBalloonSkins[bloon] = false;
            }

            List<BalloonOverrideSymbol> RandomOrder = EnabledBalloonSkins.Keys.ToList();
            RandomOrder.Shuffle();

            List<BalloonOverrideSymbol> RandomSelected = new List<BalloonOverrideSymbol>();
            for (int i = 0; i < numberOfOptions;i++)
            {
                RandomSelected.Add(RandomOrder[i]);
            }
            
            foreach (var bloon in RandomSelected)
            {
                EnabledBalloonSkins[bloon] = true;
            }
            UpdateActives();
        }



        //static Type VaricolouredBalloonsHelperType = Type.GetType("VaricolouredBalloons.VaricolouredBalloonsHelper, VaricolouredBalloons", false, false);


        public void SetBalloonSymbolOverride()
        {
            if (CurrentSkin.animFile.IsNone())
            {
                symbolOverrideController.AddSymbolOverride((HashedString)"bloon", Assets.GetAnim((HashedString)"balloon_anim_kanim").GetData().build.GetSymbol((KAnimHashedString)"body"));
            }
            else
            {
                symbolOverrideController.AddSymbolOverride((HashedString)"bloon", CurrentSkin.symbol.Unwrap());
            }

            if (NextSkin.animFile.IsNone())
            {
                symbolOverrideController.AddSymbolOverride((HashedString)"next_bloon", Assets.GetAnim((HashedString)"balloon_anim_kanim").GetData().build.GetSymbol((KAnimHashedString)"body"));
            }
            else
            {
                symbolOverrideController.AddSymbolOverride((HashedString)"next_bloon", NextSkin.symbol.Unwrap());
            }
        }


        public void UpdatePossibleBalloonSkins()
        {
            var db = Db.Get();
            foreach (var skin in BalloonArtistFacades.Infos_All)
            {
                var SkinAllowed = db.Permits.BalloonArtistFacades.Get(skin.id);
                if (SkinAllowed.IsUnlocked()) //SkinAllowed.IsUnlocked()
                {
                    var symbolOverrides = SkinAllowed.GetBalloonOverrideSymbolIDs();
                    for (int i = 0; i < symbolOverrides.Count(); ++i)
                    {
                        var BalloonSkin = SkinAllowed.GetOverrideAt(i);

                        if (!EnabledBalloonSkins.ContainsKey(BalloonSkin))
                        {
                            EnabledBalloonSkins.Add(BalloonSkin, false);
                        }
                    }
                }
            }
            UpdateActives();
        }

        private Chore.Precondition HasNoBalloon = new Chore.Precondition()
        {
            id = nameof(HasNoBalloon),
            description = "Duplicant doesn't have a balloon already",
            fn = (ref Chore.Precondition.Context context, object data) => !(context.consumerState.consumer == null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon")
        };
        public static float BloongasUsage = 5f;

        public override void OnSpawn()
        {

            base.OnSpawn();
            this.smi.StartSM();
            UpdatePossibleBalloonSkins();
            //OverwriteSymbol();
        }

        public override void OnCleanUp() => base.OnCleanUp();

        private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
        {
            string str = tag.ProperName();
            Descriptor descriptor = new Descriptor();
            descriptor.SetupDescriptor(string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, str, GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, str, GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), Descriptor.DescriptorType.Requirement);
            descs.Add(descriptor);
        }
        //private void OverwriteSymbol()
        //{
        //    if (VaricolouredBalloonsHelperType == null)
        //        return;
        //    var artist = GetComponent(BawoongiverWorkable.VaricolouredBalloonsHelperType);
        //    if (artist != null)
        //    {
        //        var symbolidx = (uint)Traverse.Create(artist).Method("get_ArtistBalloonSymbolIdx").GetValue();
        //        SgtLogger.debuglog("id: " + symbolidx);
        //        Traverse.Create(artist).Method("ApplySymbolOverrideByIdx", new[] { symbolidx }).GetValue();
        //        //Traverse.Create(artist).Method("ApplySymbolOverrideByIdx").GetValue(symbolidx);
        //    }
        //}


        List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(
          GameObject go)
        {
            List<Descriptor> descs = new List<Descriptor>();
            Descriptor descriptor = new Descriptor();
            descriptor.SetupDescriptor((string)global::STRINGS.UI.BUILDINGEFFECTS.RECREATION, (string)global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
            descs.Add(descriptor);
            //Effect.AddModifierDescriptions(this.gameObject, descs, "Balloonfriend", true);
            this.AddRequirementDesc(descs, ModAssets.Tags.BalloonGas, BloongasUsage);
            return descs;
        }

        public class States :
          GameStateMachine<States, StatesInstance, Bawoongiver>
        {
            private State unoperational;
            private OperationalStates operational;
            private ReadyStates ready;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = unoperational;
                this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational);
                this.operational.defaultState = operational.notEnoughGas;
                this.operational.notEnoughGas
                    .PlayAnim("off")
                    .TagTransition(GameTags.Operational, this.unoperational, true)
                    .Transition(operational.pumpingBloon, new Transition.ConditionCallback(this.IsReady))
                    .EventTransition(GameHashes.OnStorageChange, operational.pumpingBloon, new Transition.ConditionCallback(this.IsReady));
                this.operational.pumpingBloon
                    .PlayAnim("working", KAnim.PlayMode.Once)
                    .OnAnimQueueComplete(ready);
                this.ready.TagTransition(GameTags.Operational, this.unoperational, true)
                    .DefaultState(this.ready.idle)
                    .ToggleChore(new Func<StatesInstance, Chore>(this.CreateChore), this.operational);
                this.ready.idle
                    //.Enter((smi) => smi.master.OverwriteSymbol())
                    .PlayAnim("balloon_ready", KAnim.PlayMode.Loop)
                    .WorkableStartTransition(
                    smi => smi.master.GetComponent<BawoongiverWorkable>(), this.ready.working)
                    .Transition(this.operational,
                    Not(new Transition.ConditionCallback(this.IsReady)))
                    .EventTransition(GameHashes.OnStorageChange, this.operational,
                    Not(new Transition.ConditionCallback(this.IsReady)));
                this.ready.working.PlayAnim("giving_bloon").QueueAnim("on", true).WorkableStopTransition(
                    smi => smi.master.GetComponent<BawoongiverWorkable>(), this.ready.post);
                this.ready.post
                    .Enter( (smi) => smi.master.ApplyNextSkin())
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(operational);
            }

            private Chore CreateChore(StatesInstance smi)
            {
                Workable component = smi.master.GetComponent<BawoongiverWorkable>();
                WorkChore<BawoongiverWorkable> chore = new WorkChore<BawoongiverWorkable>(Db.Get().ChoreTypes.Relax, component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high, ignore_building_assignment: true);
                chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
                chore.AddPrecondition(smi.master.HasNoBalloon, chore);
                chore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, chore);

                return chore;
            }

            private bool IsReady(StatesInstance smi)
            {
                foreach (var item in smi.master.storage.items)
                {
                    if (item.HasTag(ModAssets.Tags.BalloonGas) && item.TryGetComponent<PrimaryElement>(out var targetElement))
                    {
                        if (targetElement.Mass >= BloongasUsage)
                            return true;
                    }
                }
                return false;
            }

            public class ReadyStates :
              State
            {
                public State idle;
                public State working;
                public State post;
            }
            public class OperationalStates :
              State
            {
                public State notEnoughGas;
                public State pumpingBloon;
            }
        }



        public class StatesInstance :
          GameStateMachine<States, StatesInstance, Bawoongiver, object>.GameInstance
        {
            public StatesInstance(Bawoongiver smi)
              : base(smi)
            {
            }
        }
    }
}

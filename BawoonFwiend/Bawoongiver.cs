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
using static HoverTextDrawer;
using static ResearchTypes;
using static STRINGS.DUPLICANTS.ROLES;

namespace BawoonFwiend
{
    internal class Bawoongiver :
  StateMachineComponent<Bawoongiver.StatesInstance>,
  IGameObjectEffectDescriptor

    {
        [Serialize]
        public Dictionary<BalloonSkinByIndex, bool> EnabledBalloonSkins = new Dictionary<BalloonSkinByIndex, bool>();


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

        [Serialize]
        List<BalloonOverrideSymbol> ActiveSkinOverrides = new List<BalloonOverrideSymbol>();
        public BalloonOverrideSymbol CurrentSkin => currentIndex == -1 ? default(BalloonOverrideSymbol) : ActiveSkinOverrides[currentIndex];
        public BalloonOverrideSymbol NextSkin => nextIndex == -1 ? default(BalloonOverrideSymbol) : ActiveSkinOverrides[nextIndex];

        public Dictionary<string, List<BalloonArtistFacadeResource>> ModdedSkinOverrideResources = new Dictionary<string, List<BalloonArtistFacadeResource>>();

        public static Type VaricolouredBalloonsHelperType = Type.GetType("VaricolouredBalloons.VaricolouredBalloonsPatches, VaricolouredBalloons", false, false);

        public struct BalloonSkinByIndex
        {
            public int animationIndex;
            public int slotIndex;
            public string modID = string.Empty;
            public BalloonSkinByIndex(int animationIndex, int slotIndex)
            {
                this.animationIndex = animationIndex;
                this.slotIndex = slotIndex;
            }
            public BalloonSkinByIndex(int animationIndex, int slotIndex, string modID)
            {
                this.animationIndex = animationIndex;
                this.slotIndex = slotIndex;
                this.modID = modID;
            }
            public override string ToString()
            {
                return modID == string.Empty
                 ? animationIndex.ToString() + " - " + slotIndex.ToString()
                 : modID + ": "+ animationIndex.ToString() + " - " + slotIndex.ToString();
            }
        }

        public void UpdateActives()
        {
            var currentSkin = ActiveSkinOverrides.Count == 0 || currentIndex == -1 ? default(BalloonOverrideSymbol) : ActiveSkinOverrides[currentIndex];
            ActiveSkinOverrides.Clear();

            foreach (var skin in EnabledBalloonSkins)
            {
                if (skin.Value)
                {
                    //if (skin.Key.modID == string.Empty)
                    //{
                        var Skin = GetOverrideViaIndex(skin.Key);
                        ActiveSkinOverrides.Add(Skin);
                    //SgtLogger.l(skin.ToString(), "ENABLED");
                    //}
                }
                else
                {
                    //SgtLogger.l(skin.ToString(), "DISABLED");
                }
            }
            //SgtLogger.l(ActiveSkinOverrides.Count.ToString(), "DISABLED");

            currentIndex = ActiveSkinOverrides.Count == 0 ? -1 : currentIndex;
            nextIndex = ActiveSkinOverrides.Count == 0 ? -1 : NextOverrideSymbolInt();
            if (currentIndex >= ActiveSkinOverrides.Count)
                currentIndex = ActiveSkinOverrides.Count - 1;

            var current = ActiveSkinOverrides.Count == 0 ? -1 : ActiveSkinOverrides.FindIndex(skin => skin.animFileID == currentSkin.animFileID && skin.animFileSymbolID == currentSkin.animFileSymbolID);
            if (current != -1)
            {
                currentIndex = current;
            }
            //SgtLogger.l(ActiveSkinOverrides.Count.ToString(), "POST");
            SetBalloonSymbolOverride();
        }

        public BalloonOverrideSymbol GetOverrideViaIndex(BalloonSkinByIndex skin)
        {
            if(skin.modID == string.Empty || skin.modID == null)
            {
                if (skin.animationIndex >= BalloonArtistFacades.Infos_All.Length)
                    return default(BalloonOverrideSymbol);
                var Anim = Db.Get().Permits.BalloonArtistFacades.Get(BalloonArtistFacades.Infos_All[skin.animationIndex].id);
                if (Anim == null)
                    return default(BalloonOverrideSymbol);
                if (skin.slotIndex >= Anim.balloonOverrideSymbolIDs.Length)
                    return Anim.GetOverrideAt(0);
                return Anim.GetOverrideAt(skin.slotIndex);
            }
            else
            {
                var id = skin.modID;
                if (skin.animationIndex >= ModdedSkinOverrideResources[id].Count)
                    return default(BalloonOverrideSymbol);
                var Anim = ModdedSkinOverrideResources[id][skin.animationIndex];
                if (Anim == null)
                    return default(BalloonOverrideSymbol);
                if (skin.slotIndex >= Anim.balloonOverrideSymbolIDs.Length)
                    return Anim.GetOverrideAt(0);
                return Anim.GetOverrideAt(skin.slotIndex);
            }

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

            if (ActiveSkinOverrides.Count == 0)
                return -1;

            if (AllRandom)
            {
                return UnityEngine.Random.Range(0, ActiveSkinOverrides.Count - 1);
            }

            return (currentIndex + 1) % (ActiveSkinOverrides.Count);
        }


        public void ApplyNextSkin()
        {
            GetNextOverrideSymbol();
            SetBalloonSymbolOverride();
        }

        public void ToggleSkin(BalloonSkinByIndex balloonSkinID)
        {
            ToggleAllBtnOn = false;
            EnabledBalloonSkins[balloonSkinID] = !EnabledBalloonSkins[balloonSkinID];
            UpdateActives();
        }
        public void ToggleAll()
        {
            //UseDefaultRedSkin = true;
            ToggleAllBtnOn = !ToggleAllBtnOn;
            var keys = new List<BalloonSkinByIndex>(EnabledBalloonSkins.Keys);
            foreach (var bloon in keys)
            {
                EnabledBalloonSkins[bloon] = ToggleAllBtnOn;
            }
            UpdateActives();
        }

        public void ToggleFullyRandom() => AllRandom = !AllRandom;

        public void SetBalloonSymbolOverride()
        {
            gameObject.TryGetComponent<SymbolOverrideController>(out var symbolOverrideController);


            symbolOverrideController.TryRemoveSymbolOverride((HashedString)"bloon");
            symbolOverrideController.TryRemoveSymbolOverride((HashedString)"next_bloon");

            if (CurrentSkin.animFile.IsNone())
            {
                symbolOverrideController.TryRemoveSymbolOverride((HashedString)"bloon");
                //symbolOverrideController.AddSymbolOverride((HashedString)"bloon", Assets.GetAnim((HashedString)"balloon_anim_kanim").GetData().build.GetSymbol((KAnimHashedString)"body"),10);
            }
            else
            {
                symbolOverrideController.AddSymbolOverride((HashedString)"bloon", CurrentSkin.symbol.Unwrap());
            }

            if (NextSkin.animFile.IsNone())
            {
                symbolOverrideController.TryRemoveSymbolOverride((HashedString)"bloon");
                //symbolOverrideController.AddSymbolOverride((HashedString)"next_bloon", Assets.GetAnim((HashedString)"balloon_anim_kanim").GetData().build.GetSymbol((KAnimHashedString)"body"), 10);
                UpdateStumpTint(new Color(159f / 255f, 54f / 255f, 54f / 255f));
            }
            else
            {
                symbolOverrideController.AddSymbolOverride((HashedString)"next_bloon", NextSkin.symbol.Unwrap());
                UpdateStumpTintAnim(NextSkin);
            }
        }

        void UpdateStumpTintAnim(BalloonOverrideSymbol SkinOverride)
        {
            var finalColor = ModAssets.GetColourFrom(SkinOverride);
            UpdateStumpTint(finalColor);
        }
        void UpdateStumpTint(Color color)
        {
            if (gameObject.TryGetComponent<KBatchedAnimController>(out var kbac))
            {
                kbac.SetSymbolTint("bloon_stump", color);
            }
        }



        public void UpdatePossibleBalloonSkins()
        {
            var db = Db.Get();

            var AllSkins = BalloonArtistFacades.Infos_All;
            for (int animIndex = 0; animIndex < AllSkins.Count(); ++animIndex)
            {
                var SkinAllowed = db.Permits.BalloonArtistFacades.Get(AllSkins[animIndex].id);
                if (SkinAllowed.IsUnlocked()) //SkinAllowed.IsUnlocked()
                {
                    var symbolOverrides = SkinAllowed.GetBalloonOverrideSymbolIDs();
                    for (int subSkinIndex = 0; subSkinIndex < symbolOverrides.Count(); ++subSkinIndex)
                    {
                        var IdentifierKey = new BalloonSkinByIndex(animIndex, subSkinIndex);
                        //var BalloonSkin = SkinAllowed.GetOverrideAt(subSkinIndex);

                        if (!EnabledBalloonSkins.ContainsKey(IdentifierKey))
                        {
                            EnabledBalloonSkins.Add(IdentifierKey, false);
                        }
                    }
                }
            }
            VaricolourBloonIntegration();
            UpdateActives();
        }


        void VaricolourBloonIntegration()
        {
            string modID = "VaricolouredBalloons";
            bool ModEnabled = VaricolouredBalloonsHelperType != null;

            if(ModEnabled== false)
            {
                var allskins = EnabledBalloonSkins.Keys.ToList();
                for (int i = EnabledBalloonSkins.Count-1; i>0; --i)
                {
                    var skin = allskins[i];
                    if (skin.modID == modID)
                    {
                        EnabledBalloonSkins.Remove(skin);
                    }
                }
            }
            else
            {
                var Resources = (IReadOnlyCollection<BalloonArtistFacadeResource>) Traverse.Create(VaricolouredBalloonsHelperType).Method("get_MyBalloons").GetValue();
                ModdedSkinOverrideResources[modID] = Resources.ToList();
                var AllSkins = ModdedSkinOverrideResources[modID];

                var unlocked = Db.Get().Permits.BalloonArtistFacades.resources.Where(facade => facade.IsUnlocked()).ToList();

                for (int animIndex = 0; animIndex < AllSkins.Count(); ++animIndex)
                {
                    var SkinToAdd = AllSkins[animIndex];
                    if (unlocked.Contains(SkinToAdd))
                        continue;

                    var symbolOverrides = SkinToAdd.GetBalloonOverrideSymbolIDs();
                    for (int subSkinIndex = 0; subSkinIndex < symbolOverrides.Count(); ++subSkinIndex)
                    {
                        var IdentifierKey = new BalloonSkinByIndex(animIndex, subSkinIndex, modID);
                        //var BalloonSkin = SkinAllowed.GetOverrideAt(subSkinIndex);

                        if (!EnabledBalloonSkins.ContainsKey(IdentifierKey))
                        {
                            EnabledBalloonSkins.Add(IdentifierKey, false);
                        }
                    }
                }
            }
        }

        //private void OverwriteSymbol()
        //{
        //    if (VaricolouredBalloonsHelperType == null)
        //        return;
        //    var artist = GetComponent(BawoongiverWorkable.VaricolouredBalloonsHelperType);
        //    if (artist != null)
        //    {
        //        var symbolidx = Traverse.Create(artist).Method("get_MyBalloons ").GetValue();
        //        SgtLogger.debuglog("id: " + symbolidx);
        //        Traverse.Create(artist).Method("ApplySymbolOverrideByIdx", new[] { symbolidx }).GetValue();
        //        //Traverse.Create(artist).Method("ApplySymbolOverrideByIdx").GetValue(symbolidx);
        //    }
        //}



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
                this.unoperational
                    .PlayAnim("off")
                    .TagTransition(GameTags.Operational, this.operational);
                this.operational.defaultState = operational.notEnoughGas;
                this.operational.notEnoughGas
                    .PlayAnim("off")
                    .TagTransition(GameTags.Operational, this.unoperational, true)
                    .Transition(operational.pumpingBloon, new Transition.ConditionCallback(this.IsReady))
                    .EventTransition(GameHashes.OnStorageChange, operational.pumpingBloon, new Transition.ConditionCallback(this.IsReady));
                this.operational.pumpingBloon
                    .PlayAnim("working", KAnim.PlayMode.Once)
                    .OnAnimQueueComplete(ready);
                this.ready
                    .TagTransition(GameTags.Operational, this.unoperational, true)
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
                    .Enter((smi) => smi.master.ApplyNextSkin())
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

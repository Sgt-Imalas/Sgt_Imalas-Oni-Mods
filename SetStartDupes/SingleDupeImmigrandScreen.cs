using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SetStartDupes
{
    internal class SingleDupeImmigrandScreen : ImmigrantScreen
    {
        public static SingleDupeImmigrandScreen instance2;
        public static void DestroyInstance() => instance2 = null;
        bool hasShown;


        public static void InitializeSingleImmigrantScreen(Personality overrideDupePersonality)
        {
            instance2.Initialize(overrideDupePersonality);
            instance2.Show();
        }
        void Initialize(Personality targetPersonality)
        {
            var containerField = AccessTools.Field(typeof(CharacterSelectionController), "containers");
            var deliverablesField = AccessTools.Field(typeof(CharacterSelectionController), "selectedDeliverables");
            var containerPrefabField = AccessTools.Field(typeof(CharacterSelectionController), "containerPrefab");
            var containerParentField = AccessTools.Field(typeof(CharacterSelectionController), "containerParent");

            var ___containerParent = (GameObject)containerPrefabField.GetValue(this);
            var ___containerPrefab = (CharacterContainer)containerParentField.GetValue(this);

            typeof(CharacterSelectionController).GetMethod("DisableProceedButton", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null);
            var __containers = (List<ITelepadDeliverableContainer>)containerField.GetValue(this);
            if (__containers != null && __containers.Count > 0)
                return;

            __containers = new List<ITelepadDeliverableContainer>();


            CharacterContainer characterContainerZZZ = Util.KInstantiateUI<CharacterContainer>(___containerPrefab.gameObject, ___containerParent);
            characterContainerZZZ.SetController(this);

            __containers.Add((ITelepadDeliverableContainer)characterContainerZZZ);
            deliverablesField.SetValue(this, new List<ITelepadDeliverable>());

            foreach (ITelepadDeliverableContainer container in __containers)
            {
                CharacterContainer characterContainer = container as CharacterContainer;
                if ((UnityEngine.Object)characterContainer != (UnityEngine.Object)null)
                    characterContainer.SetReshufflingState(false);
            }
            containerField.SetValue(this, __containers);
        }
        protected override void OnSpawn()
        {
            this.activateOnSpawn = false;
            this.ConsumeMouseScroll = false;
            base.OnSpawn();
            this.IsStarterMinion = false;
            ////this.rejectButton.onClick += new System.Action(this.OnRejectAll);
            // this.confirmRejectionBtn.onClick += new System.Action(this.OnRejectionConfirmed);
            // this.cancelRejectionBtn.onClick += new System.Action(this.OnRejectionCancelled);
            instance2 = this;
            //this.title.text = (string)UI.IMMIGRANTSCREEN.IMMIGRANTSCREENTITLE;
            //this.proceedButton.GetComponentInChildren<LocText>().text = (string)UI.IMMIGRANTSCREEN.PROCEEDBUTTON;
            //this.closeButton.onClick += (System.Action)(() => this.Show(false));
            this.Show(false);
        }
        protected override void OnShow(bool show)
        {
            if (show)
            {
                KFMOD.PlayUISound(GlobalAssets.GetSound("Dialog_Popup"));
                AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
                MusicManager.instance.PlaySong("Music_SelectDuplicant");
                this.hasShown = true;
            }
            else
            {
                AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
                if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
                    MusicManager.instance.StopSong("Music_SelectDuplicant");
                if (this.hasShown)
                    AudioMixer.instance.Start(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
            }
            base.OnShow(show);
        }
        protected override void OnProceed()
        {
            this.Show(false);
            this.containers.ForEach((Action<ITelepadDeliverableContainer>)(cc => UnityEngine.Object.Destroy((UnityEngine.Object)cc.GetGameObject())));
            this.containers.Clear();
            AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
            AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
            MusicManager.instance.PlaySong("Stinger_NewDuplicant");
        }
    }
}

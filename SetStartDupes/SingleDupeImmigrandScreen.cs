using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SetStartDupes
{
    internal class SingleDupeImmigrandScreen : CharacterSelectionController
    {
        public static SingleDupeImmigrandScreen instance2;
        public static void DestroyInstance() => instance2 = null;
        bool hasShown;


        public static void InitializeSingleImmigrantScreen(Personality overrideDupePersonality)
        {
            var window = Util.KInstantiateUI(ImmigrantScreen.instance.transform.gameObject);
            window.SetActive(true);
            window.name = "SingleDoopScreen";
            Destroy(window.GetComponent<ImmigrantScreen>());
            instance2 = (SingleDupeImmigrandScreen)window.AddComponent(typeof(SingleDupeImmigrandScreen));

            UIUtils.ListAllChildren(instance2.transform);
            instance2.proceedButton = UIUtils.TryFindComponent<KButton>(window.transform, "Layout/BottomButtons/ProceedButton");
            instance2.containerParent = instance2.transform.Find("Layout/Content").gameObject;
            instance2.Initialize(overrideDupePersonality);
            instance2.Show();
        }
        void Initialize(Personality targetPersonality)
        {
            DisableProceedButton();

            if (containers != null && containers.Count > 0)
                return;

            containers = new List<ITelepadDeliverableContainer>();

            CharacterContainer characterContainerZZZ = Util.KInstantiateUI<CharacterContainer>(ImmigrantScreen.instance.containerPrefab.gameObject, containerParent);
            characterContainerZZZ.SetController(this);

            containers.Add((ITelepadDeliverableContainer)characterContainerZZZ);
            selectedDeliverables =  new List<ITelepadDeliverable>();

            foreach (ITelepadDeliverableContainer container in containers)
            {
                CharacterContainer characterContainer = container as CharacterContainer;
                if ((UnityEngine.Object)characterContainer != (UnityEngine.Object)null)
                    characterContainer.SetReshufflingState(false);
            }
        }
        public override void InitializeContainers()
        {
            //base.InitializeContainers();
            Initialize(null);
        }
        public override void OnSpawn()
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
        public override void OnShow(bool show)
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
        public override void OnProceed()
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

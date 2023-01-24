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
            instance2 = (SingleDupeImmigrandScreen)window.AddComponent(typeof(SingleDupeImmigrandScreen));

            UIUtils.ListAllChildren(instance2.transform);
            instance2.proceedButton = UIUtils.TryFindComponent<KButton>(window.transform, "Layout/BottomButtons/ProceedButton");
            //instance2.containerParent = UIUtils.TryFindComponent<GameObject>(window.transform, "Layout/BottomButtons/ProceedButton");
            instance2.Initialize(overrideDupePersonality);
            instance2.Show();
        }
        void Initialize(Personality targetPersonality)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            var containerField =  AccessTools.Field(typeof(CharacterSelectionController), "containers");
            var deliverablesField = AccessTools.Field(typeof(CharacterSelectionController), "selectedDeliverables");
            var containerPrefabField = AccessTools.Field(typeof(CharacterSelectionController), "containerPrefab");
            var containerParentField = AccessTools.Field(typeof(CharacterSelectionController), "containerParent");

            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            var ___containerParent = (GameObject)containerPrefabField.GetValue(this);
            var ___containerPrefab = (CharacterContainer)containerParentField.GetValue(this);
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            DisableProceedButton();
           // typeof(CharacterSelectionController).GetMethod("DisableProceedButton", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null);
            var __containers = (List<ITelepadDeliverableContainer>)containerField.GetValue(this);
            if (__containers != null && __containers.Count > 0)
                return;

            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            __containers = new List<ITelepadDeliverableContainer>();

            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");

            CharacterContainer characterContainerZZZ = Util.KInstantiateUI<CharacterContainer>(___containerPrefab.gameObject,transform.Find("Layout/Content").gameObject);
            characterContainerZZZ.SetController(this);

            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            __containers.Add((ITelepadDeliverableContainer)characterContainerZZZ);
            deliverablesField.SetValue(this, new List<ITelepadDeliverable>());

            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            foreach (ITelepadDeliverableContainer container in __containers)
            {
                CharacterContainer characterContainer = container as CharacterContainer;
                if ((UnityEngine.Object)characterContainer != (UnityEngine.Object)null)
                    characterContainer.SetReshufflingState(false);
            }
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            containerField.SetValue(this, __containers);
        }
        public override void InitializeContainers()
        {
            //base.InitializeContainers();
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

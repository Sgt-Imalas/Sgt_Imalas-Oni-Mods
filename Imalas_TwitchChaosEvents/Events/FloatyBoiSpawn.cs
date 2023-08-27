using Imalas_TwitchChaosEvents.Creeper;
using Imalas_TwitchChaosEvents.OmegaSawblade;
using ONITwitchLib;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
    internal class FloatyBoiSpawn : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_CreeperEater";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.CREEPEREATINGBOI.NAME;

        public string EventDescription => STRINGS.CHAOSEVENTS.CREEPEREATINGBOI.TOASTTEXT;

        public EventWeight EventWeight => EventWeight.WEIGHT_ALMOST_NEVER;

        public Action<object> EventAction => (object data) =>
        {
            SpawnBoi();
        };

        public Func<object, bool> Condition =>
            (data) =>
            {
                return GameClock.Instance.GetCycle() > 50;
            };

        public Danger EventDanger => Danger.None;

        public void SpawnBoi()
        {
            //var spawningPosition = CameraController.Instance.baseCamera.transform.GetPosition(); ///This gives middle of the screeen
            ///Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()); this gives mouse pos
            var spawningPosition = Grid.CellToPos(ONITwitchLib.Utils.PosUtil.ClampedMouseCellWithRange(0));
            SgtLogger.l(spawningPosition.ToString(), "POS1");
            spawningPosition.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
            var blade = Util.KInstantiate(Assets.GetPrefab(CreeperConsumerPetConfig.ID), spawningPosition, Quaternion.identity);
            blade.SetActive(true);
            SgtLogger.l(spawningPosition.ToString(), "POS2");

            ToastManager.InstantiateToastWithGoTarget(
            STRINGS.CHAOSEVENTS.CREEPEREATINGBOI.TOAST,
             STRINGS.CHAOSEVENTS.CREEPEREATINGBOI.TOASTTEXT, blade
             );
        }
    }
}

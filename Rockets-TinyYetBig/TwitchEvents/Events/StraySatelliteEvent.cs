using Database;
using ONITwitchLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UnityEngine;

namespace Rockets_TinyYetBig.TwitchEvents.Events
{
    internal class StraySatelliteEvent : ITwitchEventBase
    {
        public string ID => "RTB_TwitchEvent_SatelliteCrash";
        public string EventName => "Stray Satellite";

        public Danger EventDanger => Danger.High;
        public string EventDescription => "A satellite derelict has fallen\nout of orbit on {0}";
        public EventWeight EventWeight => EventWeight.WEIGHT_NORMAL;
        public Func<object, bool> Condition =>
                (data) =>
                {
                    return (GameClock.Instance.GetCycle() > 200 && Game.Instance.savedInfo.discoveredSurface) ;
                };
        public Action<object> EventAction => 
            (data) =>
            {
                string cometId = SatelliteCometConfig.ID; 


                
                WorldContainer world = ClusterManager.Instance.activeWorld.IsModuleInterior ? ClusterManager.Instance.WorldContainers[0] : ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId);
                float x = PlayerController.GetCursorPos(KInputManager.GetMousePos()).x;
                float y = world.Height + world.WorldOffset.y - 1;
                if (!Grid.IsActiveWorld(Grid.XYToCell((int)x, (int)y)))
                    x = world.Width * UnityEngine.Random.value + world.WorldOffset.x;

                Vector3 position = new Vector3(x, y, Grid.GetLayerZ(Grid.SceneLayer.FXFront));
                GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(cometId), position, Quaternion.identity);
                gameObject.SetActive(true);

                ToastManager.InstantiateToastWithGoTarget(EventName, string.Format(EventDescription, world.GetProperName()), gameObject);
            };
    }
}

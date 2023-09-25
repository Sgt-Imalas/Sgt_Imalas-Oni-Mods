using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using Util_TwitchIntegrationLib;
using static STRINGS.BUILDINGS.PREFABS.EXTERIORWALL.FACADES;
using Util_TwitchIntegrationLib.Scripts;

namespace Imalas_TwitchChaosEvents.Events
{
    /// <summary>
    /// Retaw flood
    /// </summary>
    internal class InverseElementEvent : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_InverseElement";

        public string EventGroupID => "core.flood";

        public string EventName => STRINGS.CHAOSEVENTS.INVERSEELEMENT.NAME;

        public string EventDescription => STRINGS.CHAOSEVENTS.INVERSEELEMENT.TOASTTEXT; 

        public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;

        private static readonly CellElementEvent SpawnEvent = new(
            "TwitchSpawnedElement",
            "Spawned by Twitch",
            true
            );
        public Action<object> EventAction => (object data) =>
        {
            var cell = PosUtil.RandomCellNearMouse();

            if (SpaceCheeseChecker.HasThereBeenAttemptedSpaceCheese(cell, out var NEWcell, out var dupe, 6, 8, false, 7))
            {
                cell = NEWcell;

                GameScheduler.Instance.Schedule("creeper rain cheese protection toast", 5f, _ =>
                {
                    ToastManager.InstantiateToastWithPosTarget(
                    STRINGS.CHAOSEVENTS.SPACECHEESEDETECTED.TOAST,
                    string.Format(STRINGS.CHAOSEVENTS.SPACECHEESEDETECTED.TOASTTEXT, EventName, dupe), Grid.CellToPos(NEWcell));
                });
            }

            var nearestCell = GridUtil.FindCellWithCavityClearance(cell);



            const int maxFloodSize = 41; // A diamond with 4 cells from the middle filled in
            var cells = GridUtil.FloodCollectCells(
                nearestCell,
                c => (int)(Grid.BuildMasks[c] & (Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation)) == 0,
                maxFloodSize
            );

            var element = ElementUtil.FindElementByNameFast((string)ModElements.InverseWater.id);
            if (element == null)
            {
                Log.Warn($"Unable to find element {(string)data}");
                return;
            }

            var mass = 3000;

            foreach (var i in cells)
            {
                SimMessages.ReplaceAndDisplaceElement(
                    i,
                    element.id,
                    SpawnEvent,
                    mass,
                    element.defaultValues.temperature
                );
            }

            ToastManager.InstantiateToastWithPosTarget(
                STRINGS.CHAOSEVENTS.INVERSEELEMENT.TOAST,
                string.Format(STRINGS.CHAOSEVENTS.INVERSEELEMENT.TOASTTEXT),
                Grid.CellToPos(nearestCell)
            );
        };

        public Func<object, bool> Condition =>
            (data) =>
            { 
                return true;
            };

        public Danger EventDanger => Danger.Medium; //same as other floods

    }
}

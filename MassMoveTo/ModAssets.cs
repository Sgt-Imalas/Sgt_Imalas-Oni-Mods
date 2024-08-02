using Klei.AI;
using PeterHan.PLib.Actions;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MassMoveTo
{
    internal class ModAssets
    {
        static HashSet<Movable> cachedMovables = new();
        static PrioritySetting cachedPriority;

        public static Sprite MassMoveToolIcon;
        public static bool HasStashed => cachedMovables.Count > 0;


        internal static void ClearStashed()
        {
            cachedMovables.Clear();
        }

        internal static void MarkForMove(Movable movable, PrioritySetting priority)
        {
            cachedPriority = priority;
            if (!cachedMovables.Contains(movable))
            {
                cachedMovables.Add(movable);
            }
        }

        static Ref<Storage> AddOrGetStorageProxy(int cell)
        {
            Storage targetStorage;
            Ref<Storage> targetProxy;

            if (Grid.Objects[cell, 44] != null)
            {
                targetStorage = Grid.Objects[cell, 44].GetComponent<Storage>();
            }
            else
            {
                Vector3 position = Grid.CellToPosCBC(cell, MoveToLocationTool.Instance.visualizerLayer);
                GameObject obj = Util.KInstantiate(Assets.GetPrefab(MovePickupablePlacerConfig.ID), position);
                targetStorage = obj.GetComponent<Storage>();
                obj.SetActive(value: true);
            }

            if (targetStorage == null)
            {
                Debug.LogError("Mass Move tool: target storage was null!");
            }
            targetProxy = new Ref<Storage>(targetStorage);
            return targetProxy;
        }
        internal static void MoveAllTo(int mouseCell)
        {
            if (cachedMovables.Count > 0)
            {
                var proxy = AddOrGetStorageProxy(mouseCell);
                proxy.Get().prioritizable.SetMasterPriority(cachedPriority);

                foreach (var movable in cachedMovables)
                {
                    if (!movable.IsMarkedForMove)
                    {
                        movable.storageProxy = proxy;
                        movable.MoveToLocation(mouseCell);
                    }
                }
                ClearStashed();
            }
        }

        public static class ActionKeys
        {
            public static string ACTION_MASSMOVETOOL = "massmovetool.open";
        }
        public static class Actions
        {
            public static PAction MassMoveTool_Open { get; set; }
        }
        internal static void RegisterActions()
        {
            Actions.MassMoveTool_Open = new PActionManager().CreateAction(ActionKeys.ACTION_MASSMOVETOOL,
                STRINGS.UI.TOOLS.MOVETOSELECTTOOL.NAME, new PKeyBinding(KKeyCode.K, Modifier.Shift));
        }
    }
}

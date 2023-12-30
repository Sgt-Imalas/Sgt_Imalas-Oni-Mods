using KSerialization;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static KAnim;

namespace Rockets_TinyYetBig.Behaviours
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DockingDoor : IDockable
    {
        /// <summary>
        /// Transfer Storages
        /// </summary>

        public CellOffset porterOffset = new CellOffset(0, 0);
        [MyCmpReq] AccessControl accessControl;

        [MyCmpGet] KBatchedAnimController animController;

        public void EnableAccessAll()
        {
            for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
            {
                RefreshAccessStatus(Components.LiveMinionIdentities[idx], false);
            }
        }
        public void RefreshAccessStatus(MinionIdentity minion, bool restrictToOwnWorld)
        {

            if (restrictToOwnWorld)
            {
                if (DockingManagerSingleton.Instance.TryGetAssignmentController(GUID, out var Controller))
                {
                    SgtLogger.l("refreshing access status");
                    if (Game.Instance.assignmentManager.assignment_groups[Controller.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                    {
                        accessControl.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Neither);
                    }
                    else
                    {
                        accessControl.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
                    }
                }
                else
                    SgtLogger.warning("couldnt refresh access status - no controller found");
            }
            else
            {
                accessControl.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
            }
        }

        public CellOffset GetRotatedTeleportOffset()
        {
            var offset = porterOffset;
            if (TryGetComponent<Rotatable>(out var rotatable))
            {
                offset = rotatable.GetRotatedCellOffset(porterOffset);
            }
            return offset;
        }
        public int GetPorterCell()
        {
            return Grid.OffsetCell(Grid.PosToCell(this), GetRotatedTeleportOffset());
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
            if (DockingManagerSingleton.Instance.IsDocked(GUID, out _))
            {
                animController.Play("extended");
            }
            else
                animController.Play("retracted");

        }

        public override bool UpdateDockingConnection(bool initORCleanupCall)
        {

            bool connected = base.UpdateDockingConnection();

            if (cleaningUp)
                return connected;
            if (spacecraftHandler.clustercraft.status == Clustercraft.CraftStatus.InFlight)
            {
                if (DockingManagerSingleton.Instance.TryGetDockableIfDocked(GUID, out var currentDocked)
                    && this.HasDupeTeleporter && currentDocked.HasDupeTeleporter)
                {
                    Teleporter.SetTarget(currentDocked.Teleporter);
                    SgtLogger.l("enabling teleporter");
                    EnableAccessAll();
                }
                else
                {
                    SgtLogger.l("disabling teleporter");
                    Teleporter.SetTarget(null);
                }
            }

            if (connected)
            {
                if (animController != null)
                {
                    if (!initORCleanupCall)
                    {
                        animController.Play("extending");
                        animController.Queue("extended");
                    }
                    else
                    {
                        animController.Queue("extended");
                    }
                }
            }
            else
            {
                if (animController != null)
                {
                    if (!initORCleanupCall)
                    {
                        animController.Play("retracting");
                        animController.Queue("retracted");
                    }
                    else
                    {
                        animController.Queue("retracted");
                    }
                }
            }

            return connected;
        }
    }
}

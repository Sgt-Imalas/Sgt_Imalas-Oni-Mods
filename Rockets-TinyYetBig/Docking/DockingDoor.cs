using KSerialization;
using Rockets_TinyYetBig.Docking;
using UtilLibs;

namespace Rockets_TinyYetBig.Behaviours
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class DockingDoor : IDockable
	{
		/// <summary>
		/// Transfer Storages
		/// </summary>

		public CellOffset porterOffset = new CellOffset(0, 0);
		[MyCmpGet] AccessControl accessControl;

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
			SgtLogger.l("refreshing access status for " + minion.GetProperName() + " in " + spacecraftHandler.GetProperName() + ", restrict: " + restrictToOwnWorld);

			if (restrictToOwnWorld)
			{
				if (DockingManagerSingleton.Instance.TryGetMinionAssignment(minion, out var worldID))
				{
					if (this.WorldId == worldID)
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
			var world = this.GetMyWorld();
			if (Teleporter == null && accessControl == null)
			{
				foreach (ClustercraftInteriorDoor craftInteriorDoor in Components.ClusterCraftInteriorDoors)
				{
					if (craftInteriorDoor.GetMyWorldId() == world.id)
					{
						craftInteriorDoor.TryGetComponent<NavTeleporter>(out Teleporter);
						craftInteriorDoor.TryGetComponent<AccessControl>(out accessControl);
						SgtLogger.l("docking door attached to interior door");
						break;
					}
				}
			}
			SgtLogger.Assert("Teleporter was null", Teleporter);
			SgtLogger.Assert("accessControl was null", accessControl);


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
					//SgtLogger.l("enabling teleporter");
				}
				else
				{
					//SgtLogger.l("disabling teleporter");
					Teleporter.SetTarget(null);
				}
				EnableAccessAll();
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

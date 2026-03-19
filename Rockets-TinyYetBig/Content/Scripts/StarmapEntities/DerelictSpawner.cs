using KSerialization;
using Rockets_TinyYetBig.Content.Scripts.UI;
using Rockets_TinyYetBig.Derelicts;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Content.Scripts.StarmapEntities
{
	internal class DerelictSpawner : KMonoBehaviour
	{
		[MyCmpReq] ArtifactPOIClusterGridEntity artifactEntity;
		[Serialize] bool derelictSpawned = false;
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (!Config.Derelicts)
				return;
			if (!derelictSpawned)
			{
				TrySpawnDerelict(true);
				handle = Game.Instance.Subscribe((int)GameHashes.ClusterFogOfWarRevealed, OnStarmapHexRevealed);
			}
		}
		int handle = -1;
		public override void OnCleanUp()
		{
			if (handle != -1)
				Game.Instance.Unsubscribe(handle);

			base.OnCleanUp();
		}
		void OnStarmapHexRevealed(object _) => TrySpawnDerelict();
		void TrySpawnDerelict(bool skipNotification = false)
		{
			if (derelictSpawned)
				return;
			bool visible = ClusterGrid.Instance.IsVisible(artifactEntity);
			if (visible)
			{
				if (!SpaceStationManager.IsSpaceStationAt(artifactEntity.Location) && DerelictStation.SpawnNewDerelictStation(artifactEntity, out var entity))
				{
					ClusterMapSelectTool.Instance.SelectNextFrame(entity.GetComponent<KSelectable>());
					ClusterManager.Instance.Trigger((int)GameHashes.WorldRenamed, "derelict created revealed");
					derelictSpawned = true;
					if (!skipNotification)
					{
						Messenger.Instance.QueueMessage(new DerelictDiscoveredMessage(entity));
						MusicManager.instance.PlaySong("Stinger_WorldDetected");
					}
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.Research
{
	class DeepSpaceAnalyzer : ComplexFabricator
	{
		
		public static readonly Operational.Flag InCorrectRoom = new Operational.Flag("RTB_CorrectRoom", Operational.Flag.Type.Requirement);

		[MyCmpReq] RoomTracker roomTracker;
		SkyVisibilityMonitor.Instance skyVisibilityMonitor;

		public float ScanEfficiencyMultiplier = 1f;
		public float TearBonus = 0f;
		public float SkyVisibility = 1f;
		
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			choreType = Db.Get().ChoreTypes.Research;
			fetchChoreTypeIdHash = Db.Get().ChoreTypes.ResearchFetch.IdHash;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			skyVisibilityMonitor = this.GetSMI<SkyVisibilityMonitor.Instance>();

			CheckRoomType(null);
			RecalculateSpaceEfficiency(null);

			Subscribe(ModAssets.Hashes.OnStationMove, RecalculateSpaceEfficiency);
			Subscribe((int)GameHashes.UpdateRoom, CheckRoomType);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.UpdateRoom, CheckRoomType);
			Unsubscribe(ModAssets.Hashes.OnStationMove, RecalculateSpaceEfficiency);
		}

		void CheckRoomType(object data)
		{
			bool correctRoom = roomTracker.IsInCorrectRoom();
			operational.SetFlag(InCorrectRoom, correctRoom);
		}

		void RecalculateSpaceEfficiency(object data)
		{
			var ownPos = this.GetMyWorldLocation();
			if (!GetClosestOtherAsteroidToLocation(ownPos, out ClusterGridEntity closestAsteroid))
			{				
				ScanEfficiencyMultiplier = 1f;
				TearBonus = 0f;
				return;
			}
			var distance = ClusterGrid.Instance.GetHexDistance(ownPos, closestAsteroid.Location);

			//0.8 in orbit due to "interference", the further away from planets the better the scan efficiency, up to 2.5x

			ScanEfficiencyMultiplier = Mathf.Clamp(0.8f, 0.6f + distance * 0.2f, 2.5f);

			var tear = ClusterManager.Instance.GetClusterPOIManager().GetTemporalTear();
			if (tear != null)
			{
				var TearDistance = ClusterGrid.Instance.GetHexDistance(ownPos, tear.Location);
				TearBonus = Mathf.Clamp(0.0f, 0.99f - TearDistance * 0.33f, 1f);
			}
			else
				TearBonus = 0;
		}

		public static bool GetClosestOtherAsteroidToLocation(AxialI location, out ClusterGridEntity asteroid)
		{
			asteroid = null;
			foreach (AxialI item in AxialUtil.SpiralOut(location, ClusterGrid.Instance.numRings))
			{
				if (item == location)
					continue;

				if (ClusterGrid.Instance.IsValidCell(item))
				{
					asteroid = ClusterGrid.Instance.GetAsteroidAtCell(item);
					if (asteroid != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal float GetWorkMultiplier()
		{
			float baseMultiplier = 1f;
			baseMultiplier *= ScanEfficiencyMultiplier;
			baseMultiplier *= (1f + TearBonus);
			baseMultiplier *= Mathf.Clamp01(skyVisibilityMonitor.PercentClearSky); 
			return baseMultiplier;
		}
	}
}

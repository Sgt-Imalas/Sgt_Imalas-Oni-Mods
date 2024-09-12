using ONITwitchLib;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Fire
{
	internal class BurningCursorLogic : KMonoBehaviour, ISim33ms
	{
		[MyCmpGet]
		ParticleSystem particleSystem;

		public float energyPerSecondKJStart = 10000f;
		public float energyPerSecondKJPutOut = 3000f;
		public float distanceToPutOut = 400f;
		public float timeToPutOut = 60f;
		public float ignitionEnergy = 80f;

		public List<Tuple<CellOffset, float>> HeatZones = new List<Tuple<CellOffset, float>>()
		{
			new Tuple<CellOffset, float>( new CellOffset(0,0),1f ),
			new Tuple<CellOffset, float>( new CellOffset(-1,0),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(1,0),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(0,1),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(0,-1),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(-1,-1),0.66f ),
			new Tuple<CellOffset, float>( new CellOffset(1,-1),0.66f ),
			new Tuple<CellOffset, float>( new CellOffset(-1,-1),0.66f ),
			new Tuple<CellOffset, float>( new CellOffset(1,1),0.66f ),
			new Tuple<CellOffset, float>( new CellOffset(-1,1),0.66f )

		};
		public void Sim33ms(float dt)
		{
			Burn(dt);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			distanceRemaining = distanceToPutOut;
			timeRemaining = timeToPutOut;
		}

		private float distanceRemaining;
		private float timeRemaining;
		public float particlesStartOverTime = 240;
		public float particlesEndOverTime = 40;

		private bool scheduledForDestruction = false;

		private static readonly CellElementEvent SpawnEvent = new(
	"TwitchSpawnedElement",
	"Spawned by Twitch",
	true
	);

		void FixedUpdate()
		{
			var updatedPosition = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
			updatedPosition.z = -50f; //Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
			distanceRemaining -= (transform.position - updatedPosition).magnitude;
			transform.SetPosition(updatedPosition);
		}

		public void Burn(float dt)
		{
			if (scheduledForDestruction) return;

			timeRemaining -= dt;


			float DistanceLerp = Mathf.InverseLerp(0, distanceToPutOut, distanceRemaining);
			float TimeLerp = Mathf.InverseLerp(0, timeToPutOut, timeRemaining);


			float SmallerLerpVal = Mathf.Min(DistanceLerp, TimeLerp);

			//SgtLogger.l(DistanceLerp + " -> " + distanceRemaining +" from "+distanceToPutOut, "Lerps");

			float energy = Mathf.Lerp(energyPerSecondKJPutOut, energyPerSecondKJStart, SmallerLerpVal);

			int particles = Mathf.RoundToInt(Mathf.Lerp(particlesEndOverTime, particlesStartOverTime, SmallerLerpVal));

			var emission = particleSystem.emission;
			emission.rateOverTime = particles;

			//SgtLogger.l(particles.ToString(), "PARTICLES");

			int originCell = Grid.PosToCell(this);

			if (!Grid.IsSolidCell(Grid.CellAbove(originCell)))
			{
				SimMessages.ReplaceAndDisplaceElement(Grid.CellAbove(originCell), SimHashes.CarbonDioxide, SpawnEvent, 0.01f * dt, UtilMethods.GetKelvinFromC(600));
			}
			foreach (var HeatLocation in HeatZones)
			{
				int cell = Grid.OffsetCell(originCell, HeatLocation.first);
				if (Grid.IsValidCellInWorld(cell, ClusterManager.Instance.activeWorldId))
				{
					SimMessages.ModifyEnergy(cell, HeatLocation.second * energy * dt, UtilMethods.GetKelvinFromC(666), SimMessages.EnergySourceID.Burner);
					FireManager.Instance.ApplyIgnitionHeatToCell(cell, HeatLocation.second * ignitionEnergy * dt);
				}
			}


			if (distanceRemaining <= 0 || timeRemaining <= 0)
			{
				emission.rateOverTime = 0;
				scheduledForDestruction = true;
				ToastManager.InstantiateToast(
				STRINGS.CHAOSEVENTS.BURNINGCURSOR.TOAST,
				 STRINGS.CHAOSEVENTS.BURNINGCURSOR.TOASTTEXTENDING
				 );
				GameScheduler.Instance.Schedule("DestroyBurningCmp ", 15, (s) => Destroy(gameObject));
			}
		}
	}
}

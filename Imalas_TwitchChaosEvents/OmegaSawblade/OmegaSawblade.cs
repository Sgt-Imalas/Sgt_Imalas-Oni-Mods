using Imalas_TwitchChaosEvents.Meteors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.OmegaSawblade
{
	internal class OmegaSawblade : KMonoBehaviour, ISim33ms
	{
		private Rigidbody2D rigidBody;

		[MyCmpGet]
		private LoopingSounds sounds;
		[MyCmpGet]
		KBatchedAnimController kbac;

		public Vector3 targetPos;
		bool attracted = false;

		public List<Tuple<CellOffset, float>> DamageZones = new List<Tuple<CellOffset, float>>()
		{
			new Tuple<CellOffset, float>( new CellOffset(0,0),1f ),
			new Tuple<CellOffset, float>( new CellOffset(-1,0),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(1,0),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(0,1),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(0,-1),0.75f ),
			new Tuple<CellOffset, float>( new CellOffset(-1,-1),0.66f ),
			new Tuple<CellOffset, float>( new CellOffset(1,-1),0.66f ),
			new Tuple<CellOffset, float>( new CellOffset(1,1),0.66f ),
			new Tuple<CellOffset, float>( new CellOffset(-1,1),0.66f )

		};



		public float WorldDamagePerSecond = 4f / 5f;
		public float BuildingDamagePerSecond = 100f / 2f;
		public float BunkerDamagePerSecond = 120f / 3f;
		public float EntityDamagePerSecond = 100f / 2f;
		public float DupeDamagePerSecond = 100f / 1f;
		public float lifeTime = 20f;
		public float speed = 240f;
		public float maxSpeed = 45;
		public float sliding = 0.010f;

		public float ChanceDistanceThreshold = 14f;
		public float GuaranteedDistanceThresholdPerSec = 7f;

		public bool homeOnlyDupes = false;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			UtilMethods.ListAllComponents(gameObject);

			rigidBody = gameObject.AddOrGet<Rigidbody2D>();
			SgtLogger.Assert("Rigidbody", rigidBody);

			rigidBody.bodyType = RigidbodyType2D.Dynamic;
			rigidBody.simulated = true;
			rigidBody.useAutoMass = false;
			rigidBody.mass = 1.0f;
			rigidBody.angularDrag = 0.05f;
			rigidBody.drag = 0;
			rigidBody.gravityScale = 0;
			rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
			rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			sounds.StartSound(GlobalAssets.GetSound("IceCooledFan_fan_LP"));
			GameScheduler.Instance.Schedule("StartHomingOmegaSawblade", 8, (obj) => attracted = true);
			SgtLogger.Assert("Rigidbody", rigidBody);

		}

		public void Sim33ms(float dt)
		{
			CursorDistance(dt);
			Homing(dt);
			Damage(dt);
		}
		float distancePassed = 0;
		float timePassed = 0;
		Vector3 MousePos, OldMousePos;
		List<float> lastDistances = new List<float>();

		void CursorDistance(float dt)
		{
			if (MousePos == null)
			{
				OldMousePos = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
			}
			else
			{
				OldMousePos = MousePos;
			}
			MousePos = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
			distancePassed += (MousePos - OldMousePos).magnitude;

			timePassed += dt;
			if (timePassed > 0.33f)
			{
				SgtLogger.l(timePassed.ToString(), "distance");

				lastDistances.Add(distancePassed);
				if (lastDistances.Count > 3)
					lastDistances.RemoveAt(0);
				timePassed -= 0.33f;
				distancePassed = 0;
			}
		}

		bool BelowDistanceThreshold()
		{
			var avg = lastDistances.Sum() / (float)lastDistances.Count;

			SgtLogger.l("avg: " + avg);

			if (avg < GuaranteedDistanceThresholdPerSec)
				return true;
			if (avg < ChanceDistanceThreshold)
			{
				float random = new System.Random().Next();
				var chance = 1f - (avg - GuaranteedDistanceThresholdPerSec / ChanceDistanceThreshold - GuaranteedDistanceThresholdPerSec);
				return chance > random;
			}
			return false;

		}

		public void Damage(float dt)
		{
			if (!attracted)
				return;

			int cell = Grid.PosToCell(this);
			if (!Grid.IsValidCell(cell))
				return;

			foreach (var Zone in DamageZones)
			{
				int toDamageCell = Grid.OffsetCell(cell, Zone.first);
				if (!Grid.IsValidCell(toDamageCell))
					continue;
				TileDamage(new(toDamageCell, Zone.second), cell, dt);
				EntityDamage(new(toDamageCell, Zone.second), dt);
			}
		}

		public void TileDamage(Tuple<int, float> cellAndEnergy, int centerCell, float dt)
		{
			int cell = cellAndEnergy.first;

			if (!Grid.IsValidCell(cell))
				return;

			GameObject tile_go = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];

			float dmgMultiplier = 1f;
			bool buildingInTile = false;
			if (tile_go != null)
			{
				SimCellOccupier component = tile_go.GetComponent<SimCellOccupier>();
				if (component != null && !component.doReplaceElement)
					buildingInTile = true;
			}
			Element element = !buildingInTile ? Grid.Element[cell] : tile_go.GetComponent<PrimaryElement>().Element;

			if (element.strength == 0.0 || element.hardness == byte.MaxValue)
				return;

			float totalMultiplier = cellAndEnergy.second * dmgMultiplier / element.strength;

			if ((double)totalMultiplier == 0.0)
				return;

			if (buildingInTile)
			{
				BuildingHP component = tile_go.GetComponent<BuildingHP>();

				bool bunker = (tile_go.GetComponent<KPrefabID>().HasTag(GameTags.Bunker));

				float f = totalMultiplier * (bunker ? BunkerDamagePerSecond : BuildingDamagePerSecond) * dt;
				component.gameObject.BoxingTrigger(-794517298, new BuildingHP.DamageSourceInfo()
				{
					damage = Mathf.Max(Mathf.RoundToInt(f), 1)
				});

			}
			else
			{
				WorldDamage.Instance.ApplyDamage(cell, totalMultiplier * WorldDamagePerSecond * dt, centerCell);
			}
		}
		public void EntityDamage(Tuple<int, float> cellAndEnergy, float dt)
		{
			List<GameObject> damagedEntities = new List<GameObject>();
			float damageMultiplier = cellAndEnergy.second;
			int cell = cellAndEnergy.first;

			if (!Grid.IsValidCell(cell))
				return;

			var cellPos = Grid.CellToXY(cell);


			GameObject building_go = Grid.Objects[cell, 1];
			if (building_go != null && building_go.TryGetComponent<BuildingHP>(out var buildingHP))
			{
				if (!damagedEntities.Contains(building_go))
				{
					float f = building_go.GetComponent<KPrefabID>().HasTag(GameTags.Bunker) ? damageMultiplier * BunkerDamagePerSecond * dt : damageMultiplier * BuildingDamagePerSecond * dt;

					buildingHP.gameObject.BoxingTrigger(-794517298, new BuildingHP.DamageSourceInfo()
					{
						damage = Mathf.Max(Mathf.RoundToInt(f), 1)
					});
					damagedEntities.Add(building_go);
				}
			}
			ListPool<ScenePartitionerEntry, Comet>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, Comet>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(cellPos.X, cellPos.Y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, gathered_entries);
			foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>)gathered_entries)
			{
				Pickupable pickupable = partitionerEntry.obj as Pickupable;
				if (pickupable.TryGetComponent<Health>(out var component) && !damagedEntities.Contains(pickupable.gameObject))
				{
					if (pickupable.TryGetComponent<KPrefabID>(out var id) && id.HasTag(GameTags.DupeBrain))
						component.Damage(damageMultiplier * DupeDamagePerSecond * dt);
					else
						component.Damage(damageMultiplier * EntityDamagePerSecond * dt);

					damagedEntities.Add(pickupable.gameObject);
				}
			}
			gathered_entries.Recycle();
		}

		Vector3 GetHomingTarget(float dt)
		{
			if (!BelowDistanceThreshold())
			{
				randoPosDuration = 0;
			}

			if (randoPosDuration > 0)
			{
				randoPosDuration -= dt;
				return randoPos;
			}
			else
			{
				//float random = new System.Random().Next(1000);
				if (
					 //random < 6
					 BelowDistanceThreshold()
				   || homeOnlyDupes
					)
				{
					var ActiveWorldDupes = Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId);
					if (ActiveWorldDupes != null && ActiveWorldDupes.Count > 0)
					{
						randoPosDuration = Mathf.Min(Mathf.Max(0.33f, 2f), 3f);
						randoPos = ActiveWorldDupes.GetRandom().gameObject.transform.position;
						//SgtLogger.l(randoPos.ToString()+ ", duration: "+ randoPosDuration, "RANDOPOS");
						return randoPos;
					}
				}
				return Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
			}

		}
		float randoPosDuration = 0;
		Vector3 randoPos = Vector3.zero;
		public void Homing(float dt)
		{
			if (attracted)
			{
				lifeTime -= dt;
				targetPos = GetHomingTarget(dt);
				Vector2 force = targetPos - transform.position;
				//SgtLogger.l($"{transform.position} => {targetPos}");
				force.Normalize();
				force *= speed;
				rigidBody.AddForce(force);
				rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxSpeed);
				transform.SetPosition(transform.position += (Vector3)force * sliding * dt);
			}
			if (lifeTime <= 0)
			{
				attracted = false;
				rigidBody.velocity = Vector2.zero;
				rigidBody.freezeRotation = true;
				kbac.Stop();
				kbac.Play("idle");
				sounds.StopAllSounds();
				GameComps.Fallers.Add(gameObject, new Vector2(0, 0));
				gameObject.AddComponent<GhostFade>().DefaultFade = 0.99f;
			}
		}

	}
}

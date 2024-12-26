using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisSnowThings.Content.Scripts.Buildings
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class SnowMachine : StateMachineComponent<SnowMachine.SMInstance>
	{
		public static Dictionary<int, List<SnowMachine>> AllSnowInfos = new Dictionary<int, List<SnowMachine>>();
		public static Dictionary<int, float> AllDecors = new Dictionary<int, float>();
		public static void ResetDictionaries()
		{
			AllDecors.Clear();
			AllSnowInfos.Clear();
		}


		[Serialize]
		[SerializeField]
		public float speed;

		[Serialize]
		[SerializeField]
		public float density;

		[Serialize]
		[SerializeField]
		public float turbulence;

		[Serialize]
		[SerializeField]
		public float lifeTime;

		[MyCmpReq]
		private OccupyArea occupyArea;

		[Serialize]
		public bool active;

		public int terminatorCellOffset;

		public void SetDecorRange(int range)
		{
			if (terminatorCellOffset == range)
			{
				return;
			}

			terminatorCellOffset = range;
			AddDecor();
		}

		private ParticleSystem particles;
		private Transform colliderPlane;
		private const float Y_OFFSET = 0.73f;

		private EffectorValues decor;

		private float[] decors;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

			decors = new float[Config.Instance.SnowMachineDecor.Range];
			Subscribe((int)GameHashes.CopySettings, OnCopySettings);
		}

		private void OnActiveWorldChanged(object obj)
		{
			var main = particles.main;
			main.prewarm = true;

			if (obj is Tuple<int, int> data) //first is new, second is old
			{
				//Log.Debuglog("Tuple<int, int>");
				var worldId = this.GetMyWorldId();
				particles.gameObject.SetActive(data.first == worldId);
			}
		}

		public override void OnSpawn()
		{
			decor = new EffectorValues(Config.Instance.SnowMachineDecor.Amount, 0); // Config.Instance.SnowMachineDecor.Range);

			particles = Instantiate(ModAssets.Prefabs.snowParticlesPrefab).GetComponent<ParticleSystem>();
			particles.gameObject.SetActive(true);

			var pos = transform.position + new Vector3(0, Y_OFFSET, Grid.GetLayerZ(Grid.SceneLayer.TileFront) - 0.5f);
			particles.transform.position = pos;
			particles.transform.SetParent(transform);

			if (!gameObject.GetComponent<Operational>().IsOperational)
			{
				particles.Stop();
			}

			colliderPlane = particles.transform.Find("Plane");

			var main = particles.main;
			main.maxParticles = Config.Instance.SnowMachineMaxParticles;

			UpdateValues();

			smi.StartSM();
		}

		public float GetDecorForCell(int cell)
		{
			Grid.CellToXY(cell, out _, out int y);
			Grid.PosToXY(transform.position, out _, out int thisY);

			var yDiff = thisY - y;

			if (yDiff < 0 || yDiff >= decors.Length)
			{
				return 0f;
			}

			return decors[yDiff];
		}

		private void OnCopySettings(object obj)
		{
			if (((GameObject)obj).TryGetComponent(out SnowMachine snowMachine))
			{
				speed = snowMachine.speed;
				turbulence = snowMachine.turbulence;
				lifeTime = snowMachine.lifeTime;
				density = snowMachine.density;

				UpdateValues();
			}
		}

		public override void OnCleanUp()
		{
			RemoveDecor();
			//Game.Instance.Unsubscribe((int)GameHashes.ActiveWorldChanged, OnActiveWorldChanged);
			base.OnCleanUp();
		}

		public void UpdateValues()
		{
			var main = particles.main;
			main.startLifetime = lifeTime;

			var emission = particles.emission;
			emission.rateOverTime = density;

			var noise = particles.noise;
			noise.strength = turbulence;

			main.simulationSpeed = speed;
		}

		public void AddDecor()
		{
			RemoveDecor();

			var cell = this.NaturalBuildingCell();
			var max = Config.Instance.SnowMachineDecor.Range;
			var start = Config.Instance.SnowMachineDecor.Amount;

			var length = Mathf.Min(max, terminatorCellOffset);

			for (int i = 0; i < length; i++)
			{
				var c = Grid.OffsetCell(cell, 0, -i);
				decors[i] = Mathf.Max(0, start - (i * 2.5f));

				if (!AllSnowInfos.ContainsKey(c))
				{
					AllSnowInfos.Add(c, new List<SnowMachine>());

					var cells = new CellOffset[length];

					for (int j = 0; j < length; j++)
					{
						cells[j] = new CellOffset(0, j);
					}

					occupyArea.SetCellOffsets(cells);
				}

				if (!AllSnowInfos[c].Contains(this))
				{
					AllSnowInfos[c].Add(this);
					Grid.Decor[c] += decors[i];
				}
			}
		}

		private void RemoveDecor()
		{
			var cell = this.NaturalBuildingCell();

			for (int i = 0; i < decors.Length; i++)
			{
				var c = Grid.OffsetCell(cell, 0, -i);
				if (AllSnowInfos.ContainsKey(c))
				{
					AllSnowInfos[c].Remove(this);
					Grid.Decor[c] -= decors[i];
				}

				decors[i] = 0;
			}
		}

		public class States : GameStateMachine<States, SMInstance, SnowMachine>
		{
			public State off;
			public State on;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = off;

				off
					.Enter(StopSnowing)
					.EventTransition(GameHashes.OperationalChanged, on, smi => smi.operational.IsOperational);

				on
					.Enter(StartSnowing)
					//.ScheduleActionNextFrame("", CheckWorld)
					.ToggleTag(GameTags.Decoration)
					.Update(RefreshCollider, UpdateRate.SIM_1000ms)
					.EventTransition(GameHashes.OperationalChanged, off, smi => !smi.operational.IsOperational);
			}

			private void CheckWorld(SMInstance smi)
			{
				if (ClusterManager.Instance.activeWorldId != smi.GetMyWorldId())
				{
					smi.master.particles.gameObject.SetActive(false);
				}
			}

			private void StartSnowing(SMInstance smi)
			{
				//smi.decorProvider.SetValues(smi.master.decor);
				// smi.decorProvider.Refresh();

				smi.master.AddDecor();
				smi.master.particles.gameObject.SetActive(true);

				smi.master.particles.Play();
			}

			private void StopSnowing(SMInstance smi)
			{
				//smi.decorProvider.SetValues(DECOR.NONE);
				//smi.decorProvider.Refresh();

				smi.master.RemoveDecor();

				smi.master.particles.Stop();
				smi.master.particles.gameObject.SetActive(false);
			}

			private void RefreshCollider(SMInstance smi, float _)
			{
				var y = -FindSurfaceDistance(smi.transform.position);
				smi.master.SetDecorRange(Mathf.CeilToInt(Mathf.Abs(y)));
				smi.master.colliderPlane.localPosition = new Vector3(0, y, 0);
			}

			private bool CanPassCell(int cell)
			{
				return Grid.IsGas(cell) || Mathf.Approximately(Grid.Mass[cell], 0);
			}

			private float FindSurfaceDistance(Vector3 position)
			{
				var cell = Grid.PosToCell(position);
				if (!CanPassCell(cell))
				{
					return 0;
				}

				Grid.PosToXY(position, out var x, out var y);
				var yo = 0;

				while (CanPassCell(cell))
				{
					yo++;

					if (yo > 16)
					{
						return 16;
					}

					cell = Grid.XYToCell(x, y - yo);
				}

				var offset = (yo + Y_OFFSET) - 1f;

				if (Grid.IsLiquid(cell))
				{
					var partialHeight = Grid.Mass[cell] / Grid.Element[cell].maxMass;
					partialHeight = Mathf.Clamp01(partialHeight);

					return offset + partialHeight;
				}
				else
				{
					return offset;
				}
			}
		}

		public class SMInstance : GameStateMachine<States, SMInstance, SnowMachine, object>.GameInstance
		{
			public DecorProvider decorProvider;
			public Operational operational;

			public SMInstance(SnowMachine master) : base(master)
			{
				decorProvider = master.GetComponent<DecorProvider>();
				operational = master.GetComponent<Operational>();

			}
		}
	}
}

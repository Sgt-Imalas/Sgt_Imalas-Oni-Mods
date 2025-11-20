using ForceFieldWallTile.Content.Defs.Buildings;
using ForceFieldWallTile.Content.ModDb;
using ForceFieldWallTile.Content.Scripts.MeshGen;
using HarmonyLib;
using KSerialization;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.ELEMENTS;
using static STRINGS.INPUT_BINDINGS;
using static STRINGS.UI.UNITSUFFIXES;

namespace ForceFieldWallTile.Content.Scripts
{
	public class ForceFieldTile : StateMachineComponent<ForceFieldTile.StatesInstance>
	{
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] KPrefabID kpref;
		[MyCmpReq] Operational operational;
		[MyCmpReq] EnergyConsumer consumer;

		[SerializeField]
		public float RegenerationRate = 100f;
		[SerializeField]
		public float MaxStrenght = 1000f;
		[SerializeField]
		public float EnergySaverWattage = Config.Instance.SteadyWattage();
		[SerializeField]
		public float NormalWattage = Config.Instance.NormalWattage;

		[SerializeField]
		public float DamageMultiplier_LiquidPressure = 10f;//at default mass of 1000kg, the gas pressure overwhelms the barrier at 10000kg, more if there are adjacent shields

		Node GridNode;
		int cell;

		int top, bottom, left, right;

		[Serialize]
		float _shieldStrength = 0f;
		[Serialize]
		float _overloadCooldown = 0f;

		public float ShieldStrengthPercentage => ShieldStrength > 0f ? ShieldStrength / MaxStrenght : 0f;
		public float ShieldStrength => _shieldStrength;
		public bool ShieldActive => _shieldStrength > 0.001;
		public float OverloadCooldown => _overloadCooldown;

		public static void ClearAll()
		{
			ShieldProjectors.Clear();
			ShieldGrid.ClearAll();
		}
		static Dictionary<int, ForceFieldTile> ShieldProjectors = new Dictionary<int, ForceFieldTile>();

		List<string> Tintables = ["tintable_bloom", "tintable_fx", "tintable_off"];

		public bool IsAltVariant = false;

		public static bool ForceFieldAt(int cell, out ForceFieldTile tile) => ShieldProjectors.TryGetValue(cell, out tile);

		public override void OnSpawn()
		{
			NormalWattage = Mathf.Max(NormalWattage, EnergySaverWattage);

			cell = Grid.PosToCell(this);

			top = Grid.CellAbove(cell);
			bottom = Grid.CellBelow(cell);
			left = Grid.CellLeft(cell);
			right = Grid.CellRight(cell);

			ShieldProjectors[cell] = this;
			var pos = Grid.CellToXY(cell);
			IsAltVariant = pos.x % 2 == 0 ^ pos.y % 2 == 0;

			if (IsAltVariant)
				kbac.flipX = true;


			GridNode = new Node(cell);
			GridNode.Strenght = _shieldStrength;
			GridNode.MaxStrenght = MaxStrenght;

			SetTints();
			base.OnSpawn();
			smi.StartSM();
		}

		public static void HandleCometAt(Comet comet, int cell)
		{
			if (!ShieldProjectors.TryGetValue(cell, out ForceFieldTile tile))
				return;
			tile.OnCometImpact(comet);
		}

		void OnCometImpact(Comet comet)
		{
			if (!ShieldActive) return;

			float cometMass = comet.GetComponent<PrimaryElement>().Mass;
			float cometDamage = comet.totalTileDamage * 10000; //caps at 5k for big metal meteors
			float damage = Mathf.Max(cometMass, cometDamage);
			//SgtLogger.l("Comet Damage: " + damage);
			DropMeteorDebris(comet);
			ReceiveDamage(damage);
			Util.KDestroyGameObject(comet.gameObject);

		}
		#region ExplodeMeteor
		public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
		{
			var angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
			var PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
			var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
			return targetDirection * V;
		}
		public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
		{
			return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
		}
		static void DropMeteorDebris(Comet CometToDropMats)
		{
			if (CometToDropMats.TryGetComponent<PrimaryElement>(out var primElement))
			{
				var pos = CometToDropMats.transform.GetPosition();
				pos.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);

				if (CometToDropMats.explosionEffectHash != SpawnFXHashes.None)
					Game.Instance.SpawnFX(CometToDropMats.explosionEffectHash, pos, 0.0f);

				CometToDropMats.PlayImpactSound(pos);

				int numberOfSplinters = UnityEngine.Random.Range(CometToDropMats.explosionOreCount.x, CometToDropMats.explosionOreCount.y + 1) * 2;
				float TotalMeteorMass = CometToDropMats.explosionMass + CometToDropMats.addTileMass;
				float temperature = CometToDropMats.GetRandomTemperatureForOres();

				if (CometToDropMats is GassyMooComet)
				{
					numberOfSplinters = 0;
				}

				if (numberOfSplinters == 0)
					numberOfSplinters = Mathf.Min(Mathf.RoundToInt(TotalMeteorMass / 0.2f), 14);

				TotalMeteorMass *= (Config.Instance.MeteorMassPercentage / 100f);

				float SplinterMass = TotalMeteorMass / numberOfSplinters;


				float speed = CometToDropMats.velocity.magnitude;
				for (int splinterIndex = 0; splinterIndex < numberOfSplinters; splinterIndex++)
				{
					Vector3 randomizedDirection = GetPointOnUnitSphereCap((Vector3)CometToDropMats.velocity, 45f) * speed;
					GameObject splinter = null;

					if (primElement.ElementID != SimHashes.Creature)
					{
						splinter = primElement.Element.substance.SpawnResource(CometToDropMats.previousPosition, SplinterMass, temperature, primElement.DiseaseIdx, disease_count: Mathf.RoundToInt(primElement.DiseaseCount / SplinterMass));
					}
					if (splinter != null)
					{
						if (GameComps.Fallers.Has(splinter))
						{
							GameComps.Fallers.Remove(splinter);
						}
						GameComps.Fallers.Add(splinter, randomizedDirection);
					}
				}
				if (CometToDropMats.lootOnDestroyedByMissile != null)
				{
					var cometCell = Grid.PosToCell(CometToDropMats.previousPosition);
					foreach (var extraLoot in CometToDropMats.lootOnDestroyedByMissile)
					{
						var lootItem = Scenario.SpawnPrefab(cometCell, 0, 0, extraLoot);
						if (lootItem != null)
						{
							Vector3 randomizedDirection = GetPointOnUnitSphereCap((Vector3)CometToDropMats.velocity, 45f) * speed;
							lootItem.SetActive(true);
							if (GameComps.Fallers.Has(lootItem))
							{
								GameComps.Fallers.Remove(lootItem);
							}
							GameComps.Fallers.Add(lootItem, randomizedDirection);
						}
					}
				}
			}
		}

		static float Distance(int sourceCell, int targetCell)
		{
			var pos1 = Grid.CellToPos2D(targetCell);
			var pos2 = Grid.CellToPos2D(sourceCell);
			return Vector2.Distance(pos1, pos2);
		}

		public void Bresenhams(HashSet<Vector2I> retList, int x0, int y0, int x1, int y1)
		{
			int xDist = Math.Abs(x1 - x0);
			int yDist = -Math.Abs(y1 - y0);
			int xStep = (x0 < x1 ? +1 : -1);
			int yStep = (y0 < y1 ? +1 : -1);
			int error = xDist + yDist;
			while (x0 != x1 || y0 != y1)
			{
				if (2 * error - yDist > xDist - 2 * error)
				{
					// horizontal step
					error += yDist;
					x0 += xStep;
				}
				else
				{
					// vertical step
					error += xDist;
					y0 += yStep;
				}
				int cell = Grid.XYToCell(x0, y0);
				if (!ShieldProjectors.TryGetValue(cell, out var projector) || !projector.ShieldActive)
				{
					return;
				}
				else
				{
					retList.Add(new Vector2I(x0, y0));
				}
			}
		}
		#endregion
		public void RecievePercentageDamage(float percentage)
		{
			percentage = Mathf.Clamp01(percentage);
			var dmg = MaxStrenght * percentage;
			ReceiveDamage(dmg);
		}

		public void ReceiveDamage(float damage)
		{
			if (damage < 0.25f || !ShieldActive) return;

			float radius = Mathf.Clamp(damage / 500f, 4, 10);

			var outerCircle = ProcGen.Util.GetCircle(transform.position, Mathf.CeilToInt(radius));
			var center = Grid.PosToXY(transform.position);

			var affectedLocations = new HashSet<Vector2I>();
			foreach (var borderTile in outerCircle)
			{
				Bresenhams(affectedLocations, center.X, center.Y, (int)borderTile.x, (int)borderTile.y);
			}

			float GetStrengthWeight(ForceFieldTile tile, float strenght)
			{
				return strenght;
				float strengthweight = tile.ShieldStrengthPercentage * strenght;
				float twoThirds = strenght * 1f;
				return (strengthweight + twoThirds) / 2f;
			}

			//var affectedLocations = ProcGen.Util.GetFilledCircle(transform.position, radius);
			var cells = affectedLocations.Select(pos => Grid.PosToCell(pos)).ToHashSet();
			Dictionary<ForceFieldTile, float> AffectedTiles = [];
			float totalWeight = GetStrengthWeight(this,1);
			AffectedTiles[this] = 1;
			foreach (var cell in cells)
			{
				if (ShieldProjectors.TryGetValue(cell, out var projector) && projector.ShieldActive)
				{
					float distance = Distance(this.cell, cell);
					float weight = distance > 0 ? 1 - distance / radius : 1;

					float weightedWeight = GetStrengthWeight(projector, weight);
					AffectedTiles.Add(projector, weightedWeight);
					totalWeight += weightedWeight;
				}
			}

			foreach (var affectedShield in AffectedTiles)
			{
				float normalizedWeight = affectedShield.Value / totalWeight;
				affectedShield.Key.HandleDamageReceived(normalizedWeight * damage);
			}
		}

		void HandleDamageReceived(float damage)
		{
			if (damage < 0.25f || !ShieldActive) return;

			if (damage < _shieldStrength)
			{
				_shieldStrength -= damage;
				GridNode.Strenght = _shieldStrength;
				ShieldGrid.RedrawColors(cell);
				return;
			}
			//float overloadDamage = damage - _shieldStrength;
			//overload damage kills the shield for at least 6 seconds, at most 18 for recharge;
			//float overloadTime = Mathf.Clamp(overloadDamage, 6, 18);
			//overloadTime = 8;
			_overloadCooldown = 8;
			ResetBarrier();
		}
		void ResetBarrier()
		{
			_shieldStrength = 0;
			GridNode.Strenght = _shieldStrength;
		}
		void CooldownOverload(float dt)
		{
			_overloadCooldown -= dt;
		}

		public override void OnCleanUp()
		{
			ShieldProjectors.Remove(cell);
			SetForceFieldEnabled(false);
			base.OnCleanUp();
		}
		public bool IsOverloaded() => _overloadCooldown > 0;

		public bool RequiresRecharge()
		{
			return _shieldStrength < MaxStrenght;
		}

		void RechargeBarrier(float dt)
		{
			_shieldStrength += dt * RegenerationRate;
			_shieldStrength = Mathf.Clamp(_shieldStrength, 0, MaxStrenght);
			GridNode.Strenght = _shieldStrength;
			Redraw();
		}
		void Redraw()
		{
			if (_shieldStrength > 0)
			{
				ShieldGrid.RedrawColors(cell);
				SetTints();
			}
		}
		void SetTints()
		{
			var color = ModAssets.ColorGradientTint.Evaluate(ShieldStrengthPercentage);
			foreach (var symbol in Tintables)
			{
				kbac.SetSymbolTint(symbol, color);
			}
		}

		private void SetLightSymbolsEnabled(bool on)
		{
			kbac.SetSymbolVisiblity("tintable_bloom", on);
			kbac.SetSymbolVisiblity("tintable_fx", on);
		}
		void SetPartialLightBlocking(bool on)
		{
			if(on)
				SimMessages.SetCellProperties(cell, PartialLightBlocking.PartialLightBlockingProperties);
			else
				SimMessages.ClearCellProperties(cell, PartialLightBlocking.PartialLightBlockingProperties);
		}
		void SetForceFieldEnabled(bool enabled)
		{
			operational.SetActive(enabled);
			Sim.Cell.Properties simCellProperties = this.GetSimCellProperties();
			if (enabled)
			{
				ShieldGrid.AddNode(cell, GridNode);
				SimMessages.SetCellProperties(cell, ((byte)simCellProperties));
				SimMessages.ReplaceAndDisplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0);
			}
			else
			{	
				ShieldGrid.RemoveNode(cell, GridNode);
				SimMessages.ClearCellProperties(cell, (byte)simCellProperties);
				ResetBarrier();
			}
			SetPartialLightBlocking(enabled);
		}

		private Sim.Cell.Properties GetSimCellProperties()
		{
			Sim.Cell.Properties simCellProperties = Sim.Cell.Properties.GasImpermeable;
			simCellProperties |= Sim.Cell.Properties.LiquidImpermeable;

			return simCellProperties;
		}
		public void SetEnergySaver(bool energySaving)
		{
			if (energySaving)
				consumer.BaseWattageRating = EnergySaverWattage;
			else
				consumer.BaseWattageRating = NormalWattage;
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, ForceFieldTile, object>.GameInstance
		{
			public StatesInstance(ForceFieldTile master) : base(master)
			{
			}
			//public string Anim(string animName) => animName + master.AnimSuffix;
		}

		public class States : GameStateMachine<States, StatesInstance, ForceFieldTile>
		{
			public class OnStates : State
			{
				public State recharging;
				public State full_strength;
			}
			public class OverloadState : State
			{
				public State blink_on;
				public State blink_off;
			}
			public OnStates on;
			public OnStates on_pre;
			public OnStates on_pst;
			public State off;
			public OverloadState overloaded;


			public override void InitializeStates(out BaseState default_state)
			{
				default_state = root;

				root
					.EnterTransition(on, (smi => smi.master.ShieldActive))
					.EnterTransition(off, (smi => !smi.master.ShieldActive));

				overloaded
					.Enter(smi => smi.master.SetTints())
					.ToggleStatusItem(ModStatusItems.FFT_ShieldOverloaded, smi => smi.master)
					.PlayAnim("overloaded")
					.Update((smi, dt) => smi.master.CooldownOverload(dt))
					.UpdateTransition(off, (smi, dt) => !smi.master.IsOverloaded());
				overloaded.defaultState = overloaded.blink_off;
				overloaded.blink_off
					.Enter(smi => smi.master.SetLightSymbolsEnabled(false))
					.Exit(smi => smi.master.SetLightSymbolsEnabled(true))
					.ScheduleGoTo(0.75f, overloaded.blink_on);
				overloaded.blink_on
					.ScheduleGoTo(0.75f, overloaded.blink_off);


				off.PlayAnim("off")
					.EventTransition(GameHashes.OperationalChanged, on, (smi => smi.master.IsOperational()))
					.EnterTransition(on_pre, (smi => smi.master.IsOperational()));

				on_pre
					.PlayAnim("working_pre")
					.OnAnimQueueComplete(on);

				on.defaultState = on.recharging;
				on
					.Enter(smi => smi.master.SetForceFieldEnabled(true))
					.Exit(smi => smi.master.SetForceFieldEnabled(false))
					.Update((smi, dt) => smi.master.HandlePressure(dt), UpdateRate.SIM_1000ms)
					.UpdateTransition(overloaded, (smi, dt) => smi.master.IsOverloaded())
					.EventTransition(GameHashes.OperationalChanged, on_pst, smi => !smi.master.IsOperational())
					.PlayAnim("working_loop", KAnim.PlayMode.Loop);

				on.recharging
					.ToggleStatusItem(ModStatusItems.FFT_ShieldCharging, smi => smi.master)
					.Update((smi, dt) => smi.master.RechargeBarrier(dt))
					.Transition(on.full_strength, smi => !smi.master.RequiresRecharge());

				on.full_strength
					.ToggleStatusItem(ModStatusItems.FFT_ShieldFullyCharged, smi => smi.master)
					.Enter(smi => smi.master.SetEnergySaver(true))
					.Enter(smi => smi.master.Redraw())
					.Exit(smi => smi.master.SetEnergySaver(false))
					.Transition(on.recharging, smi => smi.master.RequiresRecharge());

				on_pst
					.PlayAnim("working_pst")
					.OnAnimQueueComplete(off);
			}
		}

		private void HandlePressure(float dt)
		{
			if (!Config.Instance.PressureDamage)
				return;
			
			HandlePressureInCell(top, dt);
			HandlePressureInCell(bottom, dt);
			HandlePressureInCell(left, dt);
			HandlePressureInCell(right, dt);
		}
		private void HandlePressureInCell(int cell, float dt)
		{
			if (ShieldProjectors.ContainsKey(cell) || Grid.IsSolidCell(cell))
				return;

			bool isLiquid = Grid.IsLiquid(cell);

			if (isLiquid)
			{
				float maxMass = Grid.Element[cell].maxMass * 1.01f;

				float currentMass = Grid.Mass[cell];
				if (currentMass < maxMass)
					return;


				float overpressureMultiplier = (currentMass / maxMass) - 1;

				float damage = dt * DamageMultiplier_LiquidPressure * overpressureMultiplier;
				ReceiveDamage(damage);
			}
		}


		private bool IsOperational()
		{
			//Prevent the on load power outage from disabling the shield
			if (Time.timeSinceLevelLoad < 0.01f)
				return ShieldActive;

			return operational.IsOperational;
		}
	}

}

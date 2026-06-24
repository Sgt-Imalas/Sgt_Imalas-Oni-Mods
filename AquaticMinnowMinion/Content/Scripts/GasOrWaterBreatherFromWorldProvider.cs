using AquaticMinnowMinion.Content.ModDb;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using static AquaticMinnowMinion.ModAssets;
using static GasBreatherFromWorldProvider;

namespace AquaticMinnowMinion.Content.Scripts
{
	internal class GasOrWaterBreatherFromWorldProvider : OxygenBreather.IGasProvider
	{
		private OxygenBreather oxygenBreather;
		private Navigator nav;

		public SimHashes LastConsumedElement { get; private set; } = SimHashes.Oxygen;

		private static Action<Sim.MassConsumedCallback, object> OnSimConsumeCallbackAction = new Action<Sim.MassConsumedCallback, object>(OnSimConsumeCallback);

		private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
		{
			SimHashes id = ElementLoader.elements[(int)mass_cb_info.elemIdx].id;
			OxygenBreather.BreathableGasConsumed(data as OxygenBreather, id, mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
		}

		private static float GetBreathableCellMass(int cell, out SimHashes elementID)
		{
			elementID = SimHashes.Vacuum;
			if (!Grid.IsValidCell(cell))
				return 0f;

			Element element = Grid.Element[cell];
			///Breathable Gases
			if (!element.HasTag(GameTags.Breathable) && !element.HasTag(Tags.BreathableWater))
				return 0f;

			elementID = element.id;
			return Grid.Mass[cell];
		}
		public BreathableCellData GetBestBreathableCellAtCurrentLocation()
		{
			return GetBestBreathableCellAroundSpecificCell(Grid.PosToCell(this.oxygenBreather), DEFAULT_BREATHABLE_OFFSETS, this.oxygenBreather);
		}

		public static BreathableCellData GetBestBreathableCellAroundSpecificCell(
		  int theSpecificCell,
		  CellOffset[] breathRange,
		  OxygenBreather breather)
		{
			return GetBestBreathableCellAroundSpecificCell(theSpecificCell, breathRange, breather, out float _);
		}
		public static BreathableCellData GetBestBreathableCellAroundSpecificCell(
			int theSpecificCell,
			CellOffset[] breathRange,
			OxygenBreather breather,
			out float totalBreathableMassAroundCell)
		{
			if (breathRange == null)
				breathRange = DEFAULT_BREATHABLE_OFFSETS;
			float breathableMass = 0.0f;
			int mostSuitableCell = theSpecificCell;
			SimHashes foundBreathableElement = SimHashes.Vacuum;
			totalBreathableMassAroundCell = 0.0f;
			foreach (CellOffset offset in breathRange)
			{
				int offsetCell = Grid.OffsetCell(theSpecificCell, offset);
				float breathableCellMass = GetBreathableCellMass(offsetCell, out SimHashes elementID);
				totalBreathableMassAroundCell += breathableCellMass;
				if (breathableCellMass > breathableMass && breathableCellMass > breather.noOxygenThreshold)
				{
					breathableMass = breathableCellMass;
					mostSuitableCell = offsetCell;
					foundBreathableElement = elementID;
				}
			}
			return new BreathableCellData()
			{
				Cell = mostSuitableCell,
				ElementID = foundBreathableElement,
				Mass = breathableMass,
				IsBreathable = foundBreathableElement != SimHashes.Vacuum
			};
		}

		public bool ConsumeGas(OxygenBreather oxygen_breather, float mass_to_consume)
		{
			if (this.nav.CurrentNavType != NavType.Tube)
			{
				BreathableCellData atCurrentLocation = this.GetBestBreathableCellAtCurrentLocation();
				if (!atCurrentLocation.IsBreathable)
					return false;
				SimHashes elementId = atCurrentLocation.ElementID;
				LastConsumedElement = elementId;
				ToggleBreathingLiquid(ElementLoader.FindElementByHash(elementId).IsLiquid);
				var handle = Game.Instance.massConsumedCallbackManager.Add(OnSimConsumeCallbackAction, (object)oxygen_breather, nameof(GasOrWaterBreatherFromWorldProvider));
				SimMessages.ConsumeMass(atCurrentLocation.Cell, elementId, mass_to_consume, 3, handle.index);
			}
			return true;
		}
		bool lastWasLiquid = false;
		void ToggleBreathingLiquid(bool breathingLiquid)
		{
			if(lastWasLiquid == breathingLiquid) return;
			lastWasLiquid = breathingLiquid;

			if (breathingLiquid)
				oxygenBreather.gameObject.Trigger(AqHashes.StartedBreathingLiquid);
			else
				oxygenBreather.gameObject.Trigger(AqHashes.StoppedBreathingLiquid);
		}

		public bool HasOxygen()
		{
			return this.oxygenBreather.prefabID.HasTag(GameTags.RecoveringBreath)
				|| this.oxygenBreather.prefabID.HasTag(GameTags.InTransitTube) 
				|| this.GetBestBreathableCellAtCurrentLocation().IsBreathable;
		}

		public bool IsBlocked() => this.oxygenBreather.HasTag(GameTags.HasSuitTank);

		public bool IsLowOxygen()
		{
			BreathableCellData atCurrentLocation = this.GetBestBreathableCellAtCurrentLocation();
			return atCurrentLocation.IsBreathable && atCurrentLocation.Mass < this.oxygenBreather.lowOxygenThreshold;
		}
		public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
		{
			ToggleBreathingLiquid(false);
		}

		public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
		{
			this.oxygenBreather = oxygen_breather;
			this.nav = this.oxygenBreather.GetComponent<Navigator>();
		}

		public bool ShouldEmitCO2()
		{
			if(this.nav.CurrentNavType == NavType.Tube || this.nav.CurrentNavType == NavType.Swim)
				return false;
			var bodyCell = Grid.PosToCell(nav);
			var headCell = Grid.CellAbove(bodyCell);

			if (Grid.IsLiquid(headCell) && Grid.IsLiquid(bodyCell))
				return false;

			///Todo: breathing out in vacuum yes/no?
			if (Grid.Element[headCell].IsVacuum && Grid.Element[bodyCell].IsVacuum)
				return false;

			return true;
		}

		public bool ShouldStoreCO2() => false;
	}
}

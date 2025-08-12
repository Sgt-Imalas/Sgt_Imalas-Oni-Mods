using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
    class Chemical_FueledFabricatorAddon : StateMachineComponent<Chemical_FueledFabricatorAddon.StatesInstance>
	{
		[SerializeField]
		public float START_FUEL_MASS = 5f;
		[SerializeField]
		public Tag fuelTag;
		[MyCmpReq]
		ComplexFabricator fabricator;
		[MyCmpReq]
		Operational operational;

		private static readonly Operational.Flag fuelRequirementFlag = new Operational.Flag("chemical_fuelRequirement", Operational.Flag.Type.Requirement);

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			if(fuelTag != null && fuelTag != Tag.Invalid)
				fabricator.keepAdditionalTag = this.fuelTag;
		}

		public override void OnSpawn()
		{
			if (fuelTag != null && fuelTag != Tag.Invalid)
				fabricator.keepAdditionalTag = this.fuelTag;
			base.OnSpawn();
			smi.StartSM();
		}
		public void SetQueueDirty() => fabricator.SetQueueDirty();

		public float GetAvailableFuel()
		{
			return fabricator.inStorage.GetAmountAvailable(this.fuelTag);
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, Chemical_FueledFabricatorAddon, object>.GameInstance
		{
			public StatesInstance(Chemical_FueledFabricatorAddon smi) : base(smi)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, Chemical_FueledFabricatorAddon>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				if (waitingForFuelStatus == null)
				{
					waitingForFuelStatus = new StatusItem("waitingForFuelStatus",global::STRINGS.BUILDING.STATUSITEMS.ENOUGH_FUEL.NAME, global::STRINGS.BUILDING.STATUSITEMS.ENOUGH_FUEL.TOOLTIP, "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022, true, null);
					waitingForFuelStatus.resolveStringCallback = delegate (string str, object obj)
					{
						Chemical_FueledFabricatorAddon fueledFabricator = (Chemical_FueledFabricatorAddon)obj;
						return string.Format(str, fueledFabricator.fuelTag.ProperName(), GameUtil.GetFormattedMass(fueledFabricator.START_FUEL_MASS, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
					};
				}
				default_state = this.waitingForFuel;
				this.waitingForFuel.Enter(delegate (StatesInstance smi)
				{
					smi.master.operational.SetFlag(fuelRequirementFlag, false);
				}).ToggleStatusItem(States.waitingForFuelStatus, (StatesInstance smi) => smi.master).EventTransition(GameHashes.OnStorageChange, this.ready, (StatesInstance smi) => smi.master.GetAvailableFuel() >= smi.master.START_FUEL_MASS);
				this.ready.Enter(delegate (StatesInstance smi)
				{
					smi.master.SetQueueDirty();
					smi.master.operational.SetFlag(fuelRequirementFlag, true);
				}).EventTransition(GameHashes.OnStorageChange, this.waitingForFuel, (StatesInstance smi) => smi.master.GetAvailableFuel() <= 0);
			}

			public static StatusItem waitingForFuelStatus;
			public State waitingForFuel;
			public State ready;
		}
	}
}

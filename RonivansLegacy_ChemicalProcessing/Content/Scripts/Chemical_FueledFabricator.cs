using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
    class Chemical_FueledFabricator : ComplexFabricator, IGameObjectEffectDescriptor
	{
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.keepAdditionalTag = this.fuelTag;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.smi = new StatesInstance(this);
			this.smi.StartSM();
		}

		public float GetAvailableFuel()
		{
			return this.inStorage.GetAmountAvailable(this.fuelTag);
		}

		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			List<GameObject> list = base.SpawnOrderProduct(recipe);
			foreach (GameObject gameObject in list)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				component.ModifyDiseaseCount(-component.DiseaseCount, "CompleteOrder");
			}
			base.GetComponent<Operational>().SetActive(false, false);
			return list;
		}

		public override List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> descriptors = base.GetDescriptors(go);
			descriptors.Add(new Descriptor(global::STRINGS.UI.BUILDINGEFFECTS.REMOVES_DISEASE, global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.REMOVES_DISEASE, Descriptor.DescriptorType.Effect, false));
			return descriptors;
		}

		private static readonly Operational.Flag fuelRequirementFlag = new Operational.Flag("chemical_fuelRequirement", Operational.Flag.Type.Requirement);
		public float GAS_CONSUMPTION_RATE;
		public float GAS_CONVERSION_RATIO = 0.1f;
		public const float START_FUEL_MASS = 5f;
		public Tag fuelTag;

		private StatesInstance smi;

		public class StatesInstance : GameStateMachine<States, StatesInstance, Chemical_FueledFabricator, object>.GameInstance
		{
			public StatesInstance(Chemical_FueledFabricator smi) : base(smi)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, Chemical_FueledFabricator>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				bool flag = States.waitingForFuelStatus == null;
				if (flag)
				{
					States.waitingForFuelStatus = new StatusItem("waitingForFuelStatus",global::STRINGS.BUILDING.STATUSITEMS.ENOUGH_FUEL.NAME, global::STRINGS.BUILDING.STATUSITEMS.ENOUGH_FUEL.TOOLTIP, "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022, true, null);
					States.waitingForFuelStatus.resolveStringCallback = delegate (string str, object obj)
					{
						Chemical_FueledFabricator fueledFabricator = (Chemical_FueledFabricator)obj;
						return string.Format(str, fueledFabricator.fuelTag.ProperName(), GameUtil.GetFormattedMass(5f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
					};
				}
				default_state = this.waitingForFuel;
				this.waitingForFuel.Enter(delegate (StatesInstance smi)
				{
					smi.master.operational.SetFlag(fuelRequirementFlag, false);
				}).ToggleStatusItem(States.waitingForFuelStatus, (StatesInstance smi) => smi.master).EventTransition(GameHashes.OnStorageChange, this.ready, (StatesInstance smi) => smi.master.GetAvailableFuel() >= 5f);
				this.ready.Enter(delegate (StatesInstance smi)
				{
					smi.master.SetQueueDirty();
					smi.master.operational.SetFlag(fuelRequirementFlag, true);
				}).EventTransition(GameHashes.OnStorageChange, this.waitingForFuel, (StatesInstance smi) => smi.master.GetAvailableFuel() <= 0f);
			}

			public static StatusItem waitingForFuelStatus;
			public State waitingForFuel;
			public State ready;
		}
	}
}

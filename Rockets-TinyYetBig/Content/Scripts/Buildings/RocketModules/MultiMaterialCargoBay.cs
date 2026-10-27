using KSerialization;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class MultiMaterialCargoBay : KMonoBehaviour, IMultiSliderControl
	{
		[SerializeField] private float solidCapacity, liquidCapacity, gasCapacity;

		[MyCmpReq] public TreeFilterable Filterable;
		[MyCmpReq] public KBatchedAnimController kbac;
		[MyCmpReq] public Storage Storage;
		[Serialize] protected float userCapacitySolid = -1, userCapacityLiquid = -1, userCapacityGas = -1;
		MeterController mc_solid, mc_liquid, mc_gas;
		int meterHandle = -1;
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (userCapacitySolid < 0 || userCapacitySolid > solidCapacity)
				userCapacitySolid = solidCapacity;
			if (userCapacityLiquid < 0 || userCapacityLiquid > liquidCapacity)
				userCapacityLiquid = liquidCapacity;
			if (userCapacityGas < 0 || userCapacityGas > gasCapacity)
				userCapacityGas = gasCapacity;
			InitMeters();
		}
		public override void OnCleanUp()
		{
			Unsubscribe(meterHandle);
			base.OnCleanUp();
		}
		void InitMeters()
		{
			mc_solid = new MeterController(kbac, "mt_solid", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
			mc_liquid = new MeterController(kbac, "mt_liquid", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
			mc_gas = new MeterController(kbac, "mt_gas", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);

			meterHandle = Subscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
			OnStorageChanged(null);
		}
		void OnStorageChanged(object _)
		{
			mc_solid.SetPositionPercent(HasSolidStorage ? GetMassFor(CargoBay.CargoType.Solids) / userCapacitySolid : 0);
			mc_liquid.SetPositionPercent(HasLiquidStorage ? GetMassFor(CargoBay.CargoType.Liquids) / userCapacityLiquid : 0);
			mc_gas.SetPositionPercent(HasGasStorage ? GetMassFor(CargoBay.CargoType.Gasses) / userCapacityGas : 0);
		}

		public void Configure(float solid = 0f, float liquid = 0f, float gas = 0f)
		{
			solidCapacity = solid;
			liquidCapacity = liquid;
			gasCapacity = gas;
		}

		internal float RemainingCapacityFor(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent<PrimaryElement>(out var primaryElement))
			{
				SgtLogger.warning("MultiMaterialCargoBay: Tried to get remaining capacity for a game object without a PrimaryElement component, returning 0");
				return 0f;
			}
			if (HasSolidStorage && primaryElement.Element.IsSolid)
			{
				return Mathf.Max(0, userCapacitySolid - Storage.GetMassAvailable(GameTags.Solid));
			}
			else if (HasLiquidStorage && primaryElement.Element.IsLiquid)
			{
				return Mathf.Max(0, userCapacityLiquid - Storage.GetMassAvailable(GameTags.Liquid));
			}
			else if (HasGasStorage && primaryElement.Element.IsGas)
			{
				return Mathf.Max(0, userCapacityGas - Storage.GetMassAvailable(GameTags.Gas));
			}
			return 0f;
		}
		internal float RemainingCapacityFor(PrimaryElement primaryElement)
		{
			if (HasSolidStorage && primaryElement.Element.IsSolid)
			{
				return Mathf.Max(0, userCapacitySolid - Storage.GetMassAvailable(GameTags.Solid));
			}
			else if (HasLiquidStorage && primaryElement.Element.IsLiquid)
			{
				return Mathf.Max(0, userCapacityLiquid - Storage.GetMassAvailable(GameTags.Liquid));
			}
			else if (HasGasStorage && primaryElement.Element.IsGas)
			{
				return Mathf.Max(0, userCapacityGas - Storage.GetMassAvailable(GameTags.Gas));
			}
			return 0f;
		}

		internal float RemainingCapacityFor(CargoBay.CargoType cargoType)
		{
			return cargoType switch
			{
				CargoBay.CargoType.Solids => HasSolidStorage ? userCapacitySolid - Storage.GetMassAvailable(GameTags.Solid) : 0f,
				CargoBay.CargoType.Liquids => HasLiquidStorage ? userCapacityLiquid - Storage.GetMassAvailable(GameTags.Liquid) : 0f,
				CargoBay.CargoType.Gasses => HasGasStorage ? userCapacityGas - Storage.GetMassAvailable(GameTags.Gas) : 0f,
				_ => 0f,
			};
		}
		internal float GetMassFor(CargoBay.CargoType cargoType)
		{
			return cargoType switch
			{
				CargoBay.CargoType.Solids => HasSolidStorage ? Storage.GetMassAvailable(GameTags.Solid) : 0f,
				CargoBay.CargoType.Liquids => HasLiquidStorage ? Storage.GetMassAvailable(GameTags.Liquid) : 0f,
				CargoBay.CargoType.Gasses => HasGasStorage ? Storage.GetMassAvailable(GameTags.Gas) : 0f,
				_ => 0f,
			};
		}

		public bool SidescreenEnabled() => true;

		public bool HasSolidStorage => solidCapacity > 0f;
		public bool HasLiquidStorage => liquidCapacity > 0f;
		public bool HasGasStorage => gasCapacity > 0f;

		public string SidescreenTitleKey => "STRINGS.UI.UISIDESCREENS.CAPACITY_CONTROL_SIDE_SCREEN.TITLE";

		public ISliderControl[] sliderControls
		{
			get
			{
				if (_sliderControls == null)
					_sliderControls = [
						new ElementTypeSliderControl(this,Element.State.Gas),
						new ElementTypeSliderControl(this,Element.State.Liquid),
						new ElementTypeSliderControl(this,Element.State.Solid)
					];
				return _sliderControls;
			}
		}

		private ISliderControl[] _sliderControls = null;

		class ElementTypeSliderControl : ISingleSliderControl
		{
			public ElementTypeSliderControl(MultiMaterialCargoBay parent, Element.State state)
			{
				this.parent = parent;
				this.state = state;
			}

			public MultiMaterialCargoBay parent;
			public Element.State state;
			public string SliderTitleKey
			{
				get
				{
					switch (state)
					{
						case Element.State.Solid:
							return "STRINGS.MISC.TAGS.SOLID";
						case Element.State.Liquid:
							return "STRINGS.MISC.TAGS.LIQUID";
						case Element.State.Gas:
							return "STRINGS.MISC.TAGS.GAS";
					}
					return "???";
				}
				//"global::STRINGS.UI.UISIDESCREENS.CAPACITY_CONTROL_SIDE_SCREEN.MAX_LABEL";

			}

			public string SliderUnits => GameUtil.GetCurrentMassUnit();

			public float GetSliderMax(int index)
			{
				return state switch
				{
					Element.State.Solid => parent.solidCapacity,
					Element.State.Liquid => parent.liquidCapacity,
					Element.State.Gas => parent.gasCapacity,
					_ => 0,
				};
			}

			public float GetSliderMin(int index) => 0;

			public string GetSliderTooltip(int index) => global::STRINGS.UI.UISIDESCREENS.CAPACITY_SIDE_SCREEN.TOOLTIP;

			public string GetSliderTooltipKey(int index) => "";

			public float GetSliderValue(int index)
			{
				return state switch
				{
					Element.State.Solid => parent.userCapacitySolid,
					Element.State.Liquid => parent.userCapacityLiquid,
					Element.State.Gas => parent.userCapacityGas,
					_ => 0,
				};
			}

			public void SetSliderValue(float percent, int index)
			{
				switch (state)
				{
					case Element.State.Solid:
						parent.userCapacitySolid = percent; break;
					case Element.State.Liquid:
						parent.userCapacityLiquid = percent; break;
					case Element.State.Gas:
						parent.userCapacityGas = percent; break;
				}
			}

			public int SliderDecimalPlaces(int index) => 0;
		}
	}
}

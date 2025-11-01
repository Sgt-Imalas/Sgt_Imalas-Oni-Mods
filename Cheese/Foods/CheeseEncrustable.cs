using Cheese.ModElements;
using KSerialization;
using UnityEngine;
using UtilLibs;

namespace Cheese.Foods
{
	internal class CheeseEncrustable : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet]
		PrimaryElement element;
		[MyCmpGet]
		KPrefabID kPrefab;
		[MyCmpGet]
		KSelectable selectable;

		[MyCmpGet]
		Edible edible;

		public static readonly float EncrustingMass = 5f;
		public static readonly float ConversionCheeseMass = 10f;

		[Serialize]
		bool cheeseEncrusted = false;
		public bool CheeseEncrusted => cheeseEncrusted;

		public static StatusItem EncrustedStatusItem
		{
			get
			{
				if (_encrustedStatusItem == null)
					RegisterStatusItems();
				return _encrustedStatusItem;
			}
		}

		static StatusItem _encrustedStatusItem;



		public static void RegisterStatusItems()
		{
			_encrustedStatusItem = new StatusItem(
					  "CHEESE_ENCRUSTEDFOOD",
					  "ITEMS",
					  "",
					  StatusItem.IconType.Info,
					  NotificationType.Good,
					  false,
					  OverlayModes.None.ID,
					  false
					  );
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (cheeseEncrusted)
				selectable.AddStatusItem(EncrustedStatusItem);
		}

		public void Sim1000ms(float dt)
		{
			if (HasCheeseConversionRecipe(out var targetSpawnTag)
			  && CanTakeCheeseFromWorld(ConversionCheeseMass * element.Mass))
			{
				ConsumeCheeseFromWorld(ConversionCheeseMass * element.Mass, (data) => ConvertToTargetItem(targetSpawnTag, data));
				return;
			}

			if (cheeseEncrusted) { return; }

			if (CanTakeCheeseFromWorld(EncrustingMass * element.Mass))
			{
				ConsumeCheeseFromWorld(EncrustingMass * element.Mass, (data) => EncrustWithCheese(data));
			}
		}

		private void ConvertToTargetItem(Tag targetSpawnTag, Sim.MassConsumedCallback data)
		{
			if (this.transform == null) return;

			var pos = this.transform.position;
			var targetPrefab = Assets.GetPrefab(targetSpawnTag);
			if (targetPrefab == null)
			{
				SgtLogger.warning("missing prefab on cheese conversion: " + targetSpawnTag);
				return;
			}
			GameObject targetFood = Util.KInstantiate(targetPrefab);
			targetFood.transform.SetPosition(pos);
			var primElement = targetFood.GetComponent<PrimaryElement>();
			var targetEdible = targetFood.GetComponent<Edible>();
			primElement.Temperature = element.Temperature;
			primElement.Units = element.Units;
			primElement.diseaseCount = element.DiseaseCount;
			primElement.diseaseID = element.diseaseID;
			targetEdible.spices = new(edible.spices);
			targetFood.SetActive(true);
			Destroy(this.gameObject);
		}

		private bool HasCheeseConversionRecipe(out Tag TargetItem)
		{
			TargetItem = null;
			if (ModAssets.Foods.CheeseConversions.ContainsKey(kPrefab.PrefabTag))
			{
				TargetItem = ModAssets.Foods.CheeseConversions[kPrefab.PrefabTag];
				return true;
			}

			return false;
		}

		public void EncrustWithCheese(Sim.MassConsumedCallback data)
		{
			cheeseEncrusted = true;
			selectable.AddStatusItem(EncrustedStatusItem);
			edible.ToggleGenericSpicedTag(true);
		}
		public void ConsumeCheeseFromWorld(float mass, System.Action<Sim.MassConsumedCallback> OnConsumption)
		{
			HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add((mass_cb_info, data) =>
			{
				if ((double)mass_cb_info.mass <= 0.0)
					return;
				OnConsumption.Invoke(mass_cb_info);
			}, null, "Cheese encrusted");

			var currentPos = (Grid.PosToCell(this));
			SimMessages.ConsumeMass(currentPos, ModElementRegistration.CheeseMolten.SimHash, mass, (byte)1, handle.index);
		}

		public bool CanTakeCheeseFromWorld(float mass)
		{
			var currentPos = (Grid.PosToCell(this));
			return Grid.IsLiquid(currentPos)
				&& Grid.Element[currentPos].id == ModElementRegistration.CheeseMolten.SimHash
				&& Grid.Mass[currentPos] >= mass;
		}
	}
}

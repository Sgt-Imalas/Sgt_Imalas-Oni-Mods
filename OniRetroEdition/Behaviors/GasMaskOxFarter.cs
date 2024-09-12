using KSerialization;
using UnityEngine;

namespace OniRetroEdition.Behaviors
{
	internal class GasMaskOxFarter : KMonoBehaviour, ISim200ms
	{
		[MyCmpGet] SuitTank tank;
		[MyCmpGet] PrimaryElement primaryElement;
		Storage gasStorage;
		[Serialize] public bool IsFarting = false;
		public float MassLostPerSecond = 0.2f;


		public override void OnSpawn()
		{
			base.OnSpawn();
			gasStorage = tank.storage;
		}

		public void Sim200ms(float dt)
		{
			if (!IsFarting)
				return;

			if (IsVentingOxygen(dt))
				return;

			DestroyOnEmpty();
		}
		bool IsVentingOxygen(float dt)
		{
			float amountToLose = MassLostPerSecond * dt;

			var pos = transform.GetPosition();

			for (int i = gasStorage.items.Count - 1; i >= 0; i--)
			{
				var item = gasStorage.items[i];
				if (item.TryGetComponent<PrimaryElement>(out var element))
				{
					float toVent = Mathf.Min(element.Mass, amountToLose);
					amountToLose -= toVent;
					SimMessages.AddRemoveSubstance(Grid.PosToCell(pos), element.ElementID, CellEventLogger.Instance.Dumpable, toVent, element.Temperature, element.DiseaseIdx, element.DiseaseCount);
					element.Mass -= toVent;

					if (amountToLose <= 0)
						return true;
				}
			}
			return false;



		}

		void DestroyOnEmpty()
		{
			var pos = transform.GetPosition();
			primaryElement.Element.substance.SpawnResource(pos, 15f, primaryElement.Temperature, byte.MaxValue, 0, false);
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}
}

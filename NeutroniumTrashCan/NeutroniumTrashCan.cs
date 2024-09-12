using UnityEngine;

namespace NeutroniumTrashCan
{
	internal class NeutroniumTrashCan : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet]
		Storage storage;

		public Color tint = Color.white;


		[MyCmpGet] KBatchedAnimController animController;

		public override void OnSpawn()
		{
			base.OnSpawn();
			animController.TintColour = tint;
		}

		public void Sim1000ms(float dt)
		{
			if (storage.MassStored() > 0)
			{
				foreach (var item in storage.items)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
		}
	}
}

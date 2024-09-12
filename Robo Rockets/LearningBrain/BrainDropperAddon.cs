using KSerialization;
using UnityEngine;

namespace RoboRockets.LearningBrain
{
	internal class BrainDropperAddon : KMonoBehaviour
	{
		[Serialize]
		public bool BrainDropsResources = false;

		public override void OnCleanUp()
		{
			if (gameObject.GetComponent("ObjectCanMove"))
				return;


			GameObject go = Util.KInstantiate(Assets.GetPrefab(BrainConfig.ID));
			if (go.TryGetComponent<DemolishableDroppable>(out var dropper))
			{
				dropper.ShouldDrop = BrainDropsResources;
			}

			go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore));
			go.SetActive(true);
			base.OnCleanUp();
		}
	}
}

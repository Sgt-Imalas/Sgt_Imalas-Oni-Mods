using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class VentTintable : KMonoBehaviour
	{
		[MyCmpReq] Storage ContentStorage;
		[MyCmpGet] KBatchedAnimController kbac;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.Sub((int)GameHashes.VentAnimatingChanged, UpdateTint);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.UnsubAll();
		}

		void UpdateTint(object data)
		{
			if (data == null || data is not Color32 tintColor)
				return;

			kbac.SetSymbolTint("tint", tintColor);
		}
	}
}

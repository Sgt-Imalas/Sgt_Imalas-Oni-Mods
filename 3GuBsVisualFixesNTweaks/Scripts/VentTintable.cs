namespace _3GuBsVisualFixesNTweaks.Scripts
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
		public override void OnSpawn()
		{
			kbac.SetSymbolVisiblity("leak_ceiling_bloom", false);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.UnsubAll();
		}
		void UpdateTint(object data)
		{
			if (data == null || data is not Element element)
				return;

			GameUtil.TintLiquidSymbolOnBuilding("leak_ceiling", kbac, element);
		}
	}
}

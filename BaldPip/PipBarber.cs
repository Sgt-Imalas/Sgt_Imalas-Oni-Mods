namespace BaldPip
{
	internal class PipBarber : KMonoBehaviour
	{
		[MyCmpReq] KBatchedAnimController kbac;

		int effectAdded, effectRemoved;
		public override void OnSpawn()
		{
			base.OnSpawn();
			Shave();
			effectAdded = Subscribe((int)GameHashes.EffectAdded, Shave);
			effectRemoved = Subscribe((int)GameHashes.EffectRemoved, Shave);

		}
		public override void OnCleanUp()
		{
			Unsubscribe(effectAdded);
			Unsubscribe(effectRemoved);
			base.OnCleanUp();
		}
		void Shave(object data = null)
		{
			kbac.SetSymbolVisiblity("sq_leaf", false);
			kbac.SetSymbolVisiblity("snapto_tag", false);
		}
	}
}

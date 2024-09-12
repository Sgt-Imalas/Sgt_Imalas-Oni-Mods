using KSerialization;
using UnityEngine;

namespace OniRetroEdition.Behaviors
{
	internal class ItemLoopedAnimHandler : KMonoBehaviour
	{
		[MyCmpReq] KBatchedAnimController kbac;

		[Serialize][SerializeField] public bool LoopBaseAnim = false;
		[Serialize][SerializeField] public string BaseAnim = "object";

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (LoopBaseAnim)
			{
				kbac.Play(BaseAnim, KAnim.PlayMode.Loop);
			}
		}

	}
}

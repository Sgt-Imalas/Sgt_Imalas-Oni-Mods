using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static BaseMinionConfig;

namespace WeebPacu
{
	internal class WeebPacuSymbolHandler : KMonoBehaviour
	{
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] SymbolOverrideController soc;


		GameObject blush, hand;
		public override void OnSpawn()
		{
			base.OnSpawn();
			SpawnTrackers_Blush();
			SpawnTrackers_Hand();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			if (blush != null)
				UnityEngine.Object.Destroy(blush);
			if (hand != null)
				UnityEngine.Object.Destroy(hand);
		}
		void SpawnTrackers_Blush()
		{
			GameObject blushGO = new GameObject("PacuwuBlush");
			blushGO.transform.SetParent(gameObject.transform, false);
			blushGO.SetActive(false);
			KBatchedAnimTracker kbatchedAnimTracker = blushGO.AddOrGet<KBatchedAnimTracker>();
			kbatchedAnimTracker.controller = kbac;
			kbatchedAnimTracker.symbol = new HashedString("body");
			//kbatchedAnimTracker.offset = new Vector3(180f, -300f, 0.0f);
			//kbatchedAnimTracker.offset = new Vector3(0, 0, 0.0f);
			//kbatchedAnimTracker.offset = new Vector3(180f, -300f, 0.0f);
			//kbatchedAnimTracker.useTargetPoint = true;
			KBatchedAnimController kbatchedAnimController = blushGO.AddOrGet<KBatchedAnimController>();
			kbatchedAnimController.AnimFiles = new KAnimFile[1]
			{
				Assets.GetAnim("weeb_pacu_trackers_kanim")
			};
			blushGO.SetActive(true);
			kbatchedAnimController.Play("blush");
			blush = blushGO;
		}

		void SpawnTrackers_Hand()
		{
			GameObject handGo = new GameObject("PacuwuHand");
			handGo.transform.SetParent(gameObject.transform, false);
			handGo.SetActive(false);
			KBatchedAnimTracker kbatchedAnimTracker = handGo.AddOrGet<KBatchedAnimTracker>();
			kbatchedAnimTracker.controller = kbac;
			kbatchedAnimTracker.symbol = new HashedString("mouth");
			//kbatchedAnimTracker.offset = new Vector3(10, 0, 0.0f);
			//kbatchedAnimTracker.useTargetPoint = true;
			KBatchedAnimController kbatchedAnimController = handGo.AddOrGet<KBatchedAnimController>();
			kbatchedAnimController.AnimFiles = new KAnimFile[1]
			{
				Assets.GetAnim("weeb_pacu_trackers_kanim")
			};
			handGo.SetActive(true);
			kbatchedAnimController.Play("hands", KAnim.PlayMode.Loop);
			hand = handGo;
		}

	}
}

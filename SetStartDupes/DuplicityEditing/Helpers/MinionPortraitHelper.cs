using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using static DetailsScreen;

namespace SetStartDupes.DuplicityEditing.Helpers
{
	public class MinionPortraitHelper : MonoBehaviour
	{
		private static GameObject crewPortraitPrefab;
		public static GameObject GetCrewPortraitPrefab()
		{
			//if (true)
			//{
				///grabbin crew face from retiredcolony info screen
				if (crewPortraitPrefab == null)
				{
					var source = ScreenPrefabs.Instance.RetiredColonyInfoScreen;

					var clone = Util.KInstantiateUI(source.duplicantPrefab);
					if (clone != null)
					{
						crewPortraitPrefab = Util.KInstantiateUI(clone);//.transform.Find("PortraitImage").gameObject);
						var tr = crewPortraitPrefab.transform;
						tr.Find("Labels").gameObject.SetActive(false);
						tr.Find("BG").gameObject.SetActive(false);
						UIUtils.FindAndRemove<KButton>(crewPortraitPrefab.transform, "");

						crewPortraitPrefab.AddOrGet<MinionPortraitHelper>();

						Destroy(clone);
					}
					// ERROR!
					else
						Debug.Log("[DSS] Error creating search prefab!  The searchbar will not function!");

				}
				return crewPortraitPrefab;
			//}
			//else
			//{
				///other option for a portrait, but kinda breaks that sourcescreen so its not used
				//if (crewPortraitPrefab == null)
				//{
				//	foreach (SideScreenRef screen in Instance.sideScreens)
				//	{
				//		if (screen.screenPrefab.TryGetComponent<AccessControlSideScreen>(out var accessControl))
				//		{
				//			var rowPrefab = Util.KInstantiateUI<AccessControlSideScreenRow>(accessControl.rowPrefab.gameObject);
				//			crewPortraitPrefab = Util.KInstantiateUI(rowPrefab.crewPortraitPrefab.gameObject);
				//			crewPortraitPrefab.SetActive(false);

				//			return crewPortraitPrefab;
				//		}
				//	}
				//}
				//return crewPortraitPrefab;
			//}
		}

		SymbolOverrideController soc;
		KBatchedAnimController kbac;

		public void ApplyMinionAccessories(List<KeyValuePair<string, string>> accessories)
		{
			if (soc == null)
				soc = gameObject.GetComponentInChildren<SymbolOverrideController>();

			if (kbac == null)
				kbac = gameObject.GetComponentInChildren<KBatchedAnimController>();

			MinionAnimUtils.ApplyNewAccessories(kbac, soc, accessories);			
			StartCoroutine(ActivatePortraitsWhenReady());
		}
		private IEnumerator ActivatePortraitsWhenReady()
		{
			yield return 0;
			kbac.transform.localScale = Vector3.one;
		}
	}
}

using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AkisDecorPackB.Content.ModDb
{
	public class DPPermitDioaramaVis_Museum : KMonoBehaviour, IKleiPermitDioramaVisTarget
	{
		[SerializeField] public KBatchedAnimController buildingKAnim;
		[SerializeField] public Image background;

		public void ConfigureSetup()
		{
			SymbolOverrideControllerUtil.AddToPrefab(buildingKAnim.gameObject);
			background.sprite = Assets.GetSprite(ModAssets.Sprites.FOSSIL_BG);
		}

		public void ConfigureWith(PermitResource permit)
		{
			KleiPermitVisUtil.ConfigureToRenderBuilding(buildingKAnim, (ArtableStage)permit);
		}

		public GameObject GetGameObject() => gameObject;
	}
	internal class ModPermitDioramas
	{
		public static DPPermitDioaramaVis_Museum museum;

		public static void Init(KleiPermitDioramaVis kleiPermitDioramaVis)
		{
			if (museum != null)
				return;

			// copying the closest laid out diorama to start out with
			var buildingOnFloor = UnityEngine.Object.Instantiate(kleiPermitDioramaVis.buildingOnFloorVis, kleiPermitDioramaVis.transform.parent, true);

			var go = buildingOnFloor.gameObject;

			// set inactive or kbac will freak out
			go.SetActive(false);

			buildingOnFloor.name = "DecorPackB_FossilStage";
			buildingOnFloor.transform.SetParent(kleiPermitDioramaVis.buildingOnFloorVis.transform.parent);

			museum = go.AddComponent<DPPermitDioaramaVis_Museum>();
			museum.buildingKAnim = go.GetComponentInChildren<KBatchedAnimController>();
			museum.background = go.transform.Find("BG").GetComponent<Image>();

			// reapply every transform because for some reason it really wants it
			museum.transform.position = kleiPermitDioramaVis.buildingOnFloorVis.transform.position;
			museum.transform.localScale = kleiPermitDioramaVis.buildingOnFloorVis.transform.localScale;

			if (museum.TryGetComponent(out RectTransform rect2) && buildingOnFloor.TryGetComponent(out RectTransform rect1))
			{
				rect2.position = rect1.position;
				rect2.anchoredPosition = rect1.anchoredPosition;
				rect2.anchorMin = rect1.anchorMin;
				rect2.anchorMax = rect1.anchorMax;
				rect2.localPosition = rect1.localPosition;
			}

			// remove original component from the copy
			UnityEngine.Object.DestroyImmediate(go.GetComponent<KleiPermitDioramaVis_BuildingOnFloor>());

			museum.ConfigureSetup();
			go.SetActive(true);

			if (kleiPermitDioramaVis.allVisList is List<IKleiPermitDioramaVisTarget> list)
				list.Add(museum);
		}
	}
}

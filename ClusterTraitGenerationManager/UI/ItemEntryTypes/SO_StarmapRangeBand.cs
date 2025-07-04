using ClusterTraitGenerationManager.ClusterData;
using ClusterTraitGenerationManager.UI.Screens;
using ClusterTraitGenerationManager.UI.SecondaryDisplayTypes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;

namespace ClusterTraitGenerationManager.UI.ItemEntryTypes
{
	class SO_StarmapRangeBand : KMonoBehaviour
	{
		string id;
		List<Tuple<string, GameObject>> ActivePOIsInBand = new List<Tuple<string, GameObject>>();
		GameObject POIContainer, POIPrefab;
		LocText HeaderInfo;
		[MyCmpGet]
		GridLayoutGroup group;
		StarmapItem _item;
		public void Init(StarmapItem item)
		{
			//SgtLogger.l(item.id, "initializing poi");
			this.id = item.id;
			this.ActivePOIsInBand.Clear();
			POIContainer = gameObject;
			HeaderInfo = transform.Find("Header/Label").gameObject.GetComponent<LocText>();
			var headerBtn = transform.Find("Header").gameObject.AddOrGet<FButton>();
			headerBtn.OnClick += () =>
			{
				CGM_MainScreen_UnityScreen.Instance.SetSelectedItem(new SelectedGalleryStarmapItem(item));
			};

			POIPrefab = transform.Find("VanillaWorldPrefab").gameObject;
			POIPrefab.SetActive(false);
			_item = item;
			RefreshHeader();
		}
		public void RefreshHeader()
		{
			if (CustomCluster.HasStarmapItem(id, out var item))
			{
				if (!item.IsPOI)
					return;
				HeaderInfo.SetText(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER.VANILLAPOI_RESOURCES.DISTANCELABEL_DLC, item.id.Substring(0, 8), item.InstancesToSpawn.ToString("0.0"), item.minRing, item.maxRing));
			}
		}
		public bool RemovePoiUI(string id)
		{
			for (int i = 0; i < ActivePOIsInBand.Count; ++i)
			{
				var poi = ActivePOIsInBand[i];
				if (poi.first == id)
				{
					UnityEngine.Object.Destroy(poi.second);
					ActivePOIsInBand.RemoveAt(i);
					return true;
				}
			}
			return false;
		}
		public GameObject AddPoiUI(string id)
		{
			if (!ModAssets.SO_POIs.ContainsKey(id))
				return null;

			GameObject poiEntry = Util.KInstantiateUI(POIPrefab, POIContainer, true);
			poiEntry.SetActive(true);

			var poi = ModAssets.SO_POIs[id];
			if (poi != null)
			{
				var image = poiEntry.transform.Find("Image").gameObject.GetComponent<Image>();

				image.sprite = poi.Sprite;
				UnityEngine.Rect rect = image.sprite.rect;
				if (rect.width > rect.height)
				{
					var size = (rect.height / rect.width) * 60;
					image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				}
				else
				{
					var size = (rect.width / rect.height) * 60;
					image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				}
				//poiEntry.transform.Find("Label").gameObject.GetComponent<LocText>().text = poi.Name;

				poiEntry.AddOrGet<FButton>().OnClick += () =>
				{
					CGM_MainScreen_UnityScreen.Instance.SetSelectedItem(new SelectedSinglePOI_SO(id, _item.id));
				};
			}
			else
				SgtLogger.warning("POI in gallery was null!");

			ActivePOIsInBand.Add(new(id, poiEntry));
			return poiEntry;
		}
	}
}

using ClusterTraitGenerationManager.ClusterData;
using ClusterTraitGenerationManager.UI.Screens;
using ClusterTraitGenerationManager.UI.SecondaryDisplayTypes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace ClusterTraitGenerationManager.UI.ItemEntryTypes
{
	class VanillaStarmapRangeBand : KMonoBehaviour
	{
		int range;
		List<Tuple<string, GameObject>> ActivePOIsInBand = new List<Tuple<string, GameObject>>();
		GameObject POIContainer, POIPrefab;
		FButton AddNewPOI;
		public bool IsLatestEntry
		{
			get
			{
				return _isLatestEntry;
			}
			set
			{
				_isLatestEntry = value;
				Wormhole.SetActive(value);
			}
		}
		private bool _isLatestEntry;
		GameObject Wormhole;


		public void Init(int range)
		{
			this.range = range;
			this.ActivePOIsInBand.Clear();
			POIContainer = transform.Find("MiningWorldsContainer/ScrollArea/Content").gameObject;
			transform.Find("DistanceHeader/Label").gameObject.GetComponent<LocText>().text = ((range + 1) * 10000).ToString() + global::STRINGS.UI.UNITSUFFIXES.DISTANCE.KILOMETER;

			POIPrefab = transform.Find("MiningWorldsContainer/ScrollArea/Content/VanillaWorldPrefab").gameObject;
			POIPrefab.SetActive(false);
			AddNewPOI = transform.Find("MiningWorldsContainer/ScrollArea/Content/AddPOI").gameObject.AddOrGet<FButton>();
			AddNewPOI.gameObject.SetActive(true);
			AddNewPOI.OnClick += () =>
				VanillaPOISelectorScreen.InitializeView(range, (id) =>
				{
					AddPoi(id);
					CGSMClusterManager.CustomCluster.AddVanillaPoi(id, range);
					CGM_MainScreen_UnityScreen.Instance.SetSelectedItem(new SelectedSinglePOI_Vanilla(id, range));
				});

			Wormhole = AddPoi("Wormhole");
			IsLatestEntry = false;
		}
		public bool RemovePoi(string id)
		{
			for (int i = 0; i < ActivePOIsInBand.Count; ++i)
			{
				var poi = ActivePOIsInBand[i];
				if (poi.first == id)
				{
					UnityEngine.Object.Destroy(poi.second);
					ActivePOIsInBand.RemoveAt(i);
					CGSMClusterManager.CustomCluster.RemoveVanillaPoi(id, range);
					return true;
				}
			}
			return false;
		}
		public GameObject AddPoi(string id)
		{
			GameObject poiEntry = Util.KInstantiateUI(POIPrefab, POIContainer, true);
			poiEntry.SetActive(true);
			var poi = Db.Get().SpaceDestinationTypes.TryGet(id);
			if (poi != null)
			{
				poiEntry.transform.Find("Image").gameObject.GetComponent<Image>().sprite = Assets.GetSprite(poi.spriteName);
				poiEntry.transform.Find("Label").gameObject.GetComponent<LocText>().text = poi.Name;

				poiEntry.AddOrGet<FButton>().OnClick += () =>
				{
					CGM_MainScreen_UnityScreen.Instance.SetSelectedItem(new SelectedSinglePOI_Vanilla(id, range));
				};
			}
			else
				SgtLogger.warning("POI in gallery was null!");

			ActivePOIsInBand.Add(new(id, poiEntry));
			AddNewPOI.transform.SetAsLastSibling();
			return poiEntry;
		}
	}
}

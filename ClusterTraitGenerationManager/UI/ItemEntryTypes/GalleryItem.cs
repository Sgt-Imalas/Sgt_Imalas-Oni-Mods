using ClusterTraitGenerationManager.ClusterData;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;

namespace ClusterTraitGenerationManager.UI.ItemEntryTypes
{
	public class GalleryItem : KMonoBehaviour
	{
		public LocText ItemNumber, PlanetName;
		public FToggleButton ActiveToggle;
		public GameObject DisabledOverlay;
		public string StarmapItemId;
		public Image MixingImage;
		public GameObject MixingImageBG;
		bool _wasMixed = false;
		ToolTip desc;
		public Image DLC_Banner;
		public FToggle Checkbox;

		public void Initialize(StarmapItem planet)
		{
			if (planet == null)
			{
				SgtLogger.error("gallery item planet was null!");
			}
			StarmapItemId = planet.id;

			Image itemIconImage = transform.Find("Image").GetComponent<Image>();
			MixingImage = transform.Find("MixingImage").gameObject.GetComponent<Image>();
			DLC_Banner = transform.Find("DLC_Banner").gameObject.GetComponent<Image>();
			MixingImageBG = transform.Find("MixingImageBG").gameObject;
			ItemNumber = transform.Find("AmountLabel").GetComponent<LocText>();
			PlanetName = transform.Find("Label").GetComponent<LocText>();
			DisabledOverlay = transform.Find("DisabledOverlay").gameObject;

			Checkbox = transform.Find("Checkbox").gameObject.AddOrGet<FToggle>();
			Checkbox.SetCheckmark("Checkmark");

			ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
			itemIconImage.sprite = planet.planetSprite;

			UnityEngine.Rect rect = itemIconImage.sprite.rect;
			
			if (ModAssets.GetBannerColor(planet, out var color))
				DLC_Banner.color = color;
			else
				DLC_Banner.gameObject.SetActive(false);


			desc = UIUtils.AddSimpleTooltipToObject(this.transform,
				//"("+ planet.id+")\n"+ 
				planet.DisplayName + "\n\n" + planet.DisplayDescription, true, 300, true);
			Refresh(planet, true, false, false);

		}
		public void Refresh(StarmapItem planet, bool inCluster, bool currentlySelected, bool isMixed)
		{
			float number = planet.InstancesToSpawn;
			bool planetActive = inCluster;//CGSMClusterManager.CustomCluster.HasStarmapItem(planet.Id)

			Checkbox.SetOnFromCode(inCluster);
			ActiveToggle.SetIsSelected(currentlySelected);
			DisabledOverlay.SetActive(!planetActive);
			bool showPlanetNumber = planetActive && !Mathf.Approximately(number, 1);

			ItemNumber.gameObject.SetActive(showPlanetNumber);
			if (showPlanetNumber)
				ItemNumber.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", number.ToString("0.0"));

			_wasMixed = isMixed;
			MixingImage.gameObject.SetActive(isMixed);
			MixingImageBG.SetActive(isMixed);
			string tt = planet.DisplayName + "\n\n" + planet.DisplayDescription;
			desc.SetSimpleTooltip(tt);
			PlanetName.SetText(planet.DisplayName);

			if (isMixed)
			{
				MixingImage.sprite = planet.planetMixingSprite;
			}
		}
	}
}

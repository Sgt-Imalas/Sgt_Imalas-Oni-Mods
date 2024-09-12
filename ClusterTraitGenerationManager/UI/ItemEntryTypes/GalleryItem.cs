using ClusterTraitGenerationManager.ClusterData;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;

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

		public void Initialize(StarmapItem planet)
		{
			if (planet == null)
			{
				SgtLogger.error("gallery item planet was null!");
			}
			StarmapItemId = planet.id;

			Image itemIconImage = transform.Find("Image").GetComponent<Image>();
			MixingImage = transform.Find("MixingImage").gameObject.GetComponent<Image>();
			MixingImageBG = transform.Find("MixingImageBG").gameObject;
			ItemNumber = transform.Find("AmountLabel").GetComponent<LocText>();
			PlanetName = transform.Find("Label").GetComponent<LocText>();
			DisabledOverlay = transform.Find("DisabledOverlay").gameObject;
			ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
			itemIconImage.sprite = planet.planetSprite;

			UnityEngine.Rect rect = itemIconImage.sprite.rect;
			if (rect.width > rect.height)
			{
				var size = (rect.height / rect.width) * 80f;
				itemIconImage.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (5 + (80 - size) / 2), size);
			}
			else
			{
				var size = (rect.width / rect.height) * 80f;
				itemIconImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}



			desc = UIUtils.AddSimpleTooltipToObject(this.transform,
				//"("+ planet.id+")\n"+ 
				planet.DisplayName + "\n\n" + planet.DisplayDescription, true, 300, true);
			Refresh(planet, true, false, false);

		}
		public void Refresh(StarmapItem planet, bool inCluster, bool currentlySelected, bool isMixed)
		{
			float number = planet.InstancesToSpawn;
			bool planetActive = inCluster;//CGSMClusterManager.CustomCluster.HasStarmapItem(planet.Id)

			ActiveToggle.ChangeSelection(currentlySelected);
			DisabledOverlay.SetActive(!planetActive);
			ItemNumber.gameObject.SetActive(planetActive);
			if (planetActive)
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

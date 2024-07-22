using UnityEngine;
using UtilLibs.UI.FUI;
using UtilLibs;
using UnityEngine.UI;
using ClusterTraitGenerationManager.ClusterData;

namespace ClusterTraitGenerationManager.UI.ItemEntryTypes
{
    public class GalleryItem : KMonoBehaviour
    {
        public LocText ItemNumber;
        public FToggleButton ActiveToggle;
        public GameObject DisabledOverlay;

        public void Initialize(StarmapItem planet)
        {
            if(planet == null)
            {
                SgtLogger.error("gallery item planet was null!");
            }

            Image itemIconImage = transform.Find("Image").GetComponent<Image>();
            ItemNumber = transform.Find("AmountLabel").GetComponent<LocText>();
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



            UIUtils.AddSimpleTooltipToObject(this.transform,"("+ planet.id+")\n"+ planet.DisplayName + "\n\n" + planet.DisplayDescription, true, 300, true);
            Refresh(planet, true);
        }
        public void Refresh(StarmapItem planet, bool inCluster, bool currentlySelected = false)
        {
            float number = planet.InstancesToSpawn;
            bool planetActive = inCluster;//CGSMClusterManager.CustomCluster.HasStarmapItem(planet.Id)

            ActiveToggle.ChangeSelection(currentlySelected);
            DisabledOverlay.SetActive(!planetActive);
            ItemNumber.gameObject.SetActive(planetActive);
            if (planetActive)
                ItemNumber.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", number.ToString("0.0"));
        }
    }
}

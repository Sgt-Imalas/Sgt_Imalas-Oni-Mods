using UnityEngine;
using UnityEngine.UI;
using UtilLibs.UI.FUI;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;

namespace ClusterTraitGenerationManager.UI.ItemEntryTypes
{
	public class CategoryItem : KMonoBehaviour
	{
		public Image CategoryIcon;
		public FToggleButton ActiveToggle;
		public StarmapItemCategory Category;

		public void Initialize(StarmapItemCategory category, Sprite newSprite)
		{
			if (newSprite != null)
			{
				CategoryIcon = transform.Find("Image").GetComponent<Image>();
			}
			Category = category;
			ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
			Refresh(StarmapItemCategory.Starter, newSprite);
		}
		public void Refresh(StarmapItemCategory category, Sprite newSprite)
		{
			ActiveToggle.ChangeSelection(this.Category == category);
			if (newSprite != null)
			{
				CategoryIcon.sprite = newSprite;
			}
		}
	}
}

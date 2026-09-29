using System;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;

namespace ClusterTraitGenerationManager.UI.ItemEntryTypes
{
	public class CategoryItem : KMonoBehaviour
	{
		public Image CategoryIcon;
		public FToggleButton ActiveToggle;
		public StarmapItemCategory Category;
		Image WarningNotification;

		public void Initialize(StarmapItemCategory category, Sprite newSprite)
		{
			if (newSprite != null)
			{
				CategoryIcon = transform.Find("Image").GetComponent<Image>();
			}
			Category = category;
			ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
			ActiveToggle.OnClick += () => ToggleWarning(false);
			Refresh(StarmapItemCategory.Starter, newSprite);
		}
		public void Refresh(StarmapItemCategory category, Sprite newSprite)
		{
			ActiveToggle.SetIsSelected(this.Category == category);
			if (newSprite != null)
			{
				CategoryIcon.sprite = newSprite;
			}
		}

		public void ToggleWarning(bool on)
		{
			if (WarningNotification == null)
				return;
			WarningNotification.gameObject.SetActive(on);
		}

		internal void InitWarning(string tt)
		{
			if(WarningNotification == null)
			{
				WarningNotification = transform.Find("InfoIcon").GetComponent<Image>();
			}
			UIUtils.AddSimpleTooltipToObject(WarningNotification.gameObject, tt);
		}
	}
}

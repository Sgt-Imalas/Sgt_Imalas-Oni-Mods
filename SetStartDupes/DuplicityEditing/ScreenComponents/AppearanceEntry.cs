using UnityEngine;
using UnityEngine.UI;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
	internal class AppearanceEntry : KMonoBehaviour
	{
		public string CategoryText = "SlotName", CurrentItemText = "unfilled";
		LocText ItemName, CategoryName;
		public System.Action OnEntryClicked;
		FButton CardButton;
		Image CurrentItemImage;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			CategoryName = transform.Find("Header").gameObject.GetComponent<LocText>();
			CategoryName.SetText(CategoryText);

			CardButton = transform.Find("Card").gameObject.AddOrGet<FButton>();
			CardButton.OnClick += OnEntryClicked;

			ItemName = transform.Find("Card/Label").gameObject.GetComponent<LocText>();

			CurrentItemImage = transform.Find("Card/Image").gameObject.GetComponent<Image>();
		}
		public void SetItemName(string itemName)
		{
			CategoryName.SetText(CategoryText);
			ItemName?.SetText(itemName);
		}
		public void SetItemIcon(Sprite icon)
		{
			CurrentItemImage.sprite = icon;
			UnityEngine.Rect rect = icon.rect;
			if (rect.width > rect.height)
			{
				var size = (rect.height / rect.width) * 80;
				CurrentItemImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			}
			else
			{
				var size = (rect.width / rect.height) * 80;
				CurrentItemImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
		}
	}
}

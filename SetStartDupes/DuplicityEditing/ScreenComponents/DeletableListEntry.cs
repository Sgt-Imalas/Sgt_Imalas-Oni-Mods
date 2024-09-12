using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
	internal class DeletableListEntry : KMonoBehaviour
	{
		public string Text, Tooltip;
		LocText label;
		public System.Action OnDeleteClicked;
		FButton deleteButton;
		public UnityEngine.Color? backgroundColor = null;
		public bool HideDelete = false;

		public override void OnPrefabInit()
		{
			base.OnSpawn();
			label = transform.Find("Label").GetComponent<LocText>();
			label.SetText(Text);

			if (!HideDelete)
			{
				deleteButton = transform.Find("DeleteButton").gameObject.AddOrGet<FButton>();
				deleteButton.OnClick += OnDeleteClicked;
			}
			else
			{
				transform.Find("DeleteButton").gameObject.SetActive(false);
			}


			if (Tooltip != null && Tooltip.Length > 0)
			{
				UIUtils.AddSimpleTooltipToObject(this.gameObject, Tooltip);
			}

			if (backgroundColor.HasValue)
			{
				transform.Find("Background").gameObject.GetComponent<Image>().color = backgroundColor.Value;
			}
		}
	}
}

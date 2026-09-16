using UnityEngine.UI;
using UtilLibs.UIcmp;

namespace UtilLibs.UI.FUI
{
	public class FExpandToggle : KMonoBehaviour
	{
		Image IconOpen, IconClose;
		FButton button;
		public bool Expanded = true;
		public System.Action<bool> OnChange;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			button = gameObject.AddOrGet<FButton>();
			IconOpen = transform.Find("IconOpen").gameObject.GetComponent<Image>();
			IconClose = transform.Find("IconClose").gameObject.GetComponent<Image>();
			button.OnClick += OnClicked;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			Refresh();
		}
		public void Refresh()
		{
			IconOpen?.gameObject.SetActive(!Expanded);
			IconClose?.gameObject.SetActive(Expanded);
		}
		public void OnClicked()
		{
			Expanded = !Expanded;
			Refresh();
			OnChange?.Invoke(Expanded);
		}
	}
}

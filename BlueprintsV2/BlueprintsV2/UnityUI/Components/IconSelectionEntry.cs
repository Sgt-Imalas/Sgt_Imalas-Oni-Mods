using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs.UIcmp;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
	internal class IconSelectionEntry : KMonoBehaviour
	{
		[SerializeField] public LocText IconName;
		[SerializeField] public Image Icon;
		[SerializeField] public FButton Button;

		public void CollectReferences()
		{
			IconName = transform.Find("Label").gameObject.GetComponent<LocText>();
			Icon = transform.Find("Image").gameObject.GetComponent<Image>();
			Button = gameObject.AddComponent<FButton>();
		}

		public void Init(string name, Sprite icon, System.Action OnSelect, Color tint)
		{
			Button.OnClick += OnSelect;
			IconName.SetText(name);
			Icon.sprite = icon;
			Icon.color = tint;
		}
	}
}

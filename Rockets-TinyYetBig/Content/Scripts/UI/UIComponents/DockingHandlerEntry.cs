using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UtilLibs.UIcmp;
using YamlDotNet.Core.Tokens;

namespace Rockets_TinyYetBig.Content.Scripts.UI.UIComponents
{
	internal class DockingHandlerEntry : KMonoBehaviour
	{
		public DockingSpacecraftHandler Target;

		LocText Name;
		Image Icon;
		public FButton Dock, Undock, Transfer, ViewOther;
		public System.Action DockClicked, UndockClicked, TransferClicked, ViewOtherClicked;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();


			Name = transform.Find("Row1/TitleText").gameObject.GetComponent<LocText>();
			Icon = transform.Find("Row1/SpaceCraftIcon/Image").GetComponent<Image>();//.sprite = referencedManager.GetDockingIcon();
			Icon.type = Image.Type.Simple;
			Icon.preserveAspect = true;


			Dock = transform.Find("Row1/Dock").gameObject.AddComponent<FButton>();
			Undock = transform.Find("Row1/Undock").gameObject.AddComponent<FButton>();
			Transfer = transform.Find("Row2/TransferButton").gameObject.AddComponent<FButton>();
			ViewOther = transform.Find("Row2/ViewDockedButton").gameObject.AddComponent<FButton>();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if(Target != null)
			{
				Name.SetText(Target.GetProperName());
				Icon.sprite = Target.GetDockingIcon();
				Dock.OnClick += DockClicked;
				Undock.OnClick += UndockClicked;
				Transfer.OnClick += TransferClicked;
				ViewOther.OnClick += ViewOtherClicked;
			}
		}
	}
}

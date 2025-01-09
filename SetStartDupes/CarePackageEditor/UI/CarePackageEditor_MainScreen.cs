using SetStartDupes.DuplicityEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes.CarePackageEditor.UI
{
	public class CarePackageEditor_MainScreen : FScreen
	{
		static CarePackageEditor_MainScreen Instance;
		public static void ShowCarePackageEditor(object obj)
		{
			SgtLogger.l("Opening Care Package Editor");
			ShowWindow();
		}
		public static void ShowWindow()
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.CarePackageEditorWindowPrefab, FrontEndManager.Instance.gameObject, true);
				Instance = screen.AddOrGet<CarePackageEditor_MainScreen>();
				Instance.Init();
				Instance.name = "DSS_CarePackageEditor_MainScreen";
			}
			//Instance.SetOpenedType(currentGroup, currentTrait, DupeTraitManager, openedFrom);
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
		}

		private bool initialized = false;
		public void Init()
		{
			if (initialized)
				return;
			initialized = true;


			var closeButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").gameObject.AddOrGet<FButton>();
			closeButton.OnClick += ()=>Show(false);
		}
		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				this.Show(false);
			}
			if (e.TryConsume(Action.Escape))
			{
				this.Show(false);
			}
			base.OnKeyDown(e);
		}

	}
}

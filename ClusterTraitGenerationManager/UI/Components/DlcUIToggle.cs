using ClusterTraitGenerationManager.ClusterData;
using ClusterTraitGenerationManager.UI.Screens;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UtilLibs.UIcmp;
using static SandboxSettings;

namespace ClusterTraitGenerationManager.UI.Components
{
	public class DlcUIToggle : KMonoBehaviour
	{
		internal string DlcID;

		DlcMixingSettingConfig setting;

		LocText DlcTitle;
		FToggle DlcToggle;
		Image DlcIcon, DlcBanner;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}

		internal void SetDlc(DlcMixingSettingConfig dlc)
		{
			if (dlc == null)
				return;

			setting = dlc;
			DlcID = dlc.dlcIdFrom;
			DlcTitle.SetText(DlcManager.GetDlcTitleNoFormatting(DlcID));
			DlcBanner.color = DlcManager.GetDlcBannerColor(DlcID);
			string iconName = DlcManager.GetDlcSmallLogo(DlcID);
			DlcIcon.sprite = Assets.GetSprite(iconName);
			DlcToggle.OnClick += (enabled) =>
			{
				CGSMClusterManager.ToggleWorldgenAffectingDlc(enabled, setting);
			};
			SetInteractable(true);
			RefreshOnState();
		}

		internal void Refresh()
		{
			RefreshOnState();
			bool isDlcRequired = CGSMClusterManager.CustomCluster?.HasDlcRequiringContentActive(DlcID) ?? false;
			SetInteractable(!isDlcRequired && DlcManager.IsContentSubscribed(DlcID));
		}
		public void RefreshOnState()
		{
			if (setting == null)
				return;

			bool dlcActive = CustomGameSettings.Instance.GetCurrentMixingSettingLevel(setting).id == setting.on_level.id;
			DlcToggle.SetOnFromCode(dlcActive);
		}

		internal void SetInteractable(bool isInteractable)
		{
			if (string.IsNullOrEmpty(DlcID))
				return;
			DlcToggle.SetInteractable(DlcManager.IsContentSubscribed(DlcID) && isInteractable);
		}

		private bool init = false;
		void Init()
		{
			if(init)
				return;
			init = true;
			DlcBanner = transform.Find("DlcBanner").gameObject.GetComponent<Image>();
			DlcIcon = transform.Find("Image").gameObject.GetComponent<Image>();
			DlcTitle = transform.Find("Label").gameObject.GetComponent<LocText>();
			DlcToggle = transform.Find("Checkbox").gameObject.AddComponent<FToggle>();
			DlcToggle.SetCheckmark("Checkmark");
		}

	}
}

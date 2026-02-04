using ClusterTraitGenerationManager.ClusterData;
using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using static SandboxSettings;

namespace ClusterTraitGenerationManager.UI.Components
{
	internal class MixingTargetChangeButton : KMonoBehaviour
	{
		FButton button;
		LocText Label;
		Image Image;
		public WorldMixingSettings TargetWorldMixing;
		public string MixingID;
		public Action<string> OnMixingSelected;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			button = gameObject.AddOrGet<FButton>();
			Label = transform.Find("Label").GetComponent<LocText>();
			Image = transform.Find("Image").GetComponent<Image>();
			button.OnClick += OnClick;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			Refresh();
		}
		void OnClick()
		{
			OnMixingSelected?.Invoke(TargetWorldMixing.world);
		}

		public void Refresh()
		{

			if(TargetWorldMixing != null 
				&& CustomGameSettings.Instance.MixingSettings.TryGetValue(MixingID, out var settingVal)
				&& CustomGameSettings.Instance.GetCurrentMixingSettingLevel(settingVal).id != WorldMixingSettingConfig.DisabledLevelId
				&& CGSMClusterManager.TryGetCurrentMixingTarget(TargetWorldMixing.world,out var item))
			{
				Label.SetText(item.OriginalDisplayName);
				Image.sprite = item.planetSprite;
				button.SetInteractable(true);
			}
			else
			{
				Label.SetText(VANILLAPOI_RESOURCES.NONESELECTED);
				button.SetInteractable(false);
				Image.sprite = Assets.GetSprite("unknown_far");
			}
		}

	}
}

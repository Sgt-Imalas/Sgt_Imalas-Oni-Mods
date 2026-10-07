using ClusterTraitGenerationManager.ClusterData;
using Delaunay.Geo;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs.UIcmp;

namespace ClusterTraitGenerationManager.UI.Components
{
	internal class BiomeMixingTarget : KMonoBehaviour
	{
		LocText Label;
		Image Image, DlcBanner;
		FToggle Checkbox;
		public Action<bool> OnMixingTargetSelected;
		string _labelText = string.Empty;

		public void Init(Action<bool> onMixingTargetSelected, string labelText, Sprite icon, string dlcIdFrom)
		{
			InitBase();
			OnMixingTargetSelected = onMixingTargetSelected;
			Label.SetText(labelText);
			Image.sprite = icon;
			_labelText = labelText;
			Checkbox.OnChange += onMixingTargetSelected;

			if (dlcIdFrom.IsNullOrWhiteSpace())
				DlcBanner.gameObject.SetActive(false);
			else
				DlcBanner.color = DlcManager.GetDlcBannerColor(dlcIdFrom);
		}
		public void SetChecked(bool check)
		{
			if (Checkbox == null)
				return;
			Checkbox.SetOnFromCode(check);
		}
		public void SetInteractable(bool interactable)
		{
			if (Checkbox == null)
				return;
			Checkbox.SetInteractable(interactable);
		}
		private void InitBase()
		{
			Label = transform.Find("Label").GetComponent<LocText>();
			Image = transform.Find("Image").GetComponent<Image>();
			DlcBanner = transform.Find("DLC_Banner").GetComponent<Image>();
			Checkbox = gameObject.AddOrGet<FToggle>();
			Checkbox.SetCheckmark("Checkbox/Checkmark");
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			if(!_labelText.IsNullOrWhiteSpace())
				Label?.SetText(_labelText);
		}

	}
}

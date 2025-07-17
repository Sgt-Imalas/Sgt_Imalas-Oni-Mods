using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;
using UtilLibs;
using UnityEngine.UI;

namespace SetStartDupes.CarePackageEditor.UI
{
	public class VanillaCarePackageOutlineEntry : KMonoBehaviour
	{
		public CarePackageOutline TargetOutline;

		Image DisplayImage;
		LocText Label;
		FToggle Toggle;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			UpdateUI();
		}

		private bool init = false;
		private void InitUi()
		{
			if (init)
				return;
			init = true;

			DisplayImage = transform.Find("DisplayImage")?.GetComponent<Image>();
			Label = transform.Find("Label")?.GetComponent<LocText>();
			Toggle = transform.Find("Checkbox").gameObject.AddComponent<FToggle>();
			Toggle.SetCheckmark("Checkmark");
			Toggle.OnChange += SetVanillaCarePackageEnabled;
		}
		void SetVanillaCarePackageEnabled(bool enabled)
		{
			if (TargetOutline == null)
				return;
			CarePackageOutlineManager.ToggleVanillaOutlineEnabled(TargetOutline);
		}

		public void UpdateOutline(CarePackageOutline newOutline)
		{
			TargetOutline = newOutline;
			UpdateUI();
		}
		public void UpdateUI()
		{
			if (TargetOutline == null)
			{
				//SgtLogger.l("aborting ui update, target was null");
				return;
			}
			if (!init)
			{
				InitUi();
			}
			Label?.SetText(TargetOutline.GetDescriptionString());
			Toggle.SetOnFromCode(CarePackageOutlineManager.IsVanillaCarePackageEnabled(TargetOutline));
			var TargetItem = Assets.GetPrefab(TargetOutline.ItemId);
			if (TargetItem != null)
			{
				//SgtLogger.l(TargetItem.GetProperName());
				var image = Def.GetUISprite(TargetItem);
				if (image != null)
				{
					DisplayImage.sprite = image.first;
					DisplayImage.color = image.second;
				}
			}
			else
			{
				DisplayImage.sprite = Assets.GetSprite("unknown");
				DisplayImage.color = Color.white;
			}
		}		
	}
}

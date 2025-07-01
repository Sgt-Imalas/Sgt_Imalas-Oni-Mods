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
		}

		void SelectOutline()
		{
			if (TargetOutline == null)
				return;
			CarePackageOutlineManager.TrySelectOutline(TargetOutline);
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

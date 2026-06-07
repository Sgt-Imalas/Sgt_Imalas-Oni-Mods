using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
	internal class BuildingInfoEntry : KMonoBehaviour
	{
		public string BuildingName;
		public ToolTip BuildingNameText;
		public Image BuildingIcon;
		public LocText BuildingCount;

		public void SetBuildingCount(int count) => BuildingCount.SetText("x"+count.ToString());

		public void SetBuilding(string id)
		{
			BuildingName = id;
			var def = Assets.GetBuildingDef(id);
			if(def == null)
			{
				BuildingIcon.sprite = Assets.GetSprite("unknown");
				BuildingNameText.SetSimpleTooltip(id.ToString());
				return;
			}

			BuildingIcon.sprite = def.GetUISprite();
			BuildingNameText.SetSimpleTooltip(def.Name);
			BuildingName = def.Name;
		}

		static Material OutLinedFontMAt = null;
		void InitLabelMat()
		{
			if (OutLinedFontMAt == null)
			{
				OutLinedFontMAt = new(BuildingCount.fontMaterial);
				// Enable underlay
				OutLinedFontMAt.EnableKeyword("UNDERLAY_ON");
				OutLinedFontMAt.SetColor("_UnderlayColor", new Color(0, 0, 0, 1));
				OutLinedFontMAt.SetFloat("_UnderlayDilate", 1f);
				OutLinedFontMAt.SetFloat("_UnderlaySoftness", 1f);
			}
			BuildingCount.fontMaterial = OutLinedFontMAt;
		}


		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			BuildingNameText = UIUtils.AddSimpleTooltipToObject(gameObject.transform,string.Empty);
			BuildingCount = transform.Find("Amount").gameObject.GetComponent<LocText>();
			InitLabelMat();
			BuildingIcon = transform.Find("BuildingIcon").gameObject.GetComponent<Image>();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if(!BuildingName.IsNullOrWhiteSpace())
				BuildingNameText.SetSimpleTooltip(BuildingName);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			BuildingNameText = UIUtils.AddSimpleTooltipToObject(gameObject.transform,string.Empty);
			BuildingCount = transform.Find("Amount").gameObject.GetComponent<LocText>();
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

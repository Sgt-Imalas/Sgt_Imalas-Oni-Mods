using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
	internal class BuildingInfoEntry : KMonoBehaviour
	{
		public string BuildingName;
		public LocText BuildingNameLocText;
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
				BuildingNameLocText.SetText(id.ToString());
				return;
			}

			BuildingIcon.sprite = def.GetUISprite();
			BuildingNameLocText.SetText(def.Name);
			BuildingName = def.Name;
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			BuildingNameLocText = transform.Find("Descriptor/Label").gameObject.GetComponent<LocText>();
			BuildingCount = transform.Find("Descriptor/Output").gameObject.GetComponent<LocText>();
			BuildingIcon = transform.Find("BuildingIcon").gameObject.GetComponent<Image>();
		}
	}
}

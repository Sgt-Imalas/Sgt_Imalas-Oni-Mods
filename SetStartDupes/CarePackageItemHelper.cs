using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SetStartDupes
{
	internal class CarePackageItemHelper
	{
		public static string GetSpawnableName(string CarePackageID)
		{
			GameObject prefab = Assets.GetPrefab((Tag)CarePackageID);
			if (prefab == null)
			{
				Element elementByName = ElementLoader.FindElementByName(CarePackageID);
				return elementByName != null ? elementByName.substance.name : "";
			}
			return prefab.GetProperName();
		}
		public static string GetSpawnableQuantity(string CarePackageID, float CarePackageQuantity)
		{
			if (ElementLoader.GetElement(CarePackageID.ToTag()) != null)
				return string.Format((string)global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT, Assets.GetPrefab((Tag)CarePackageID).GetProperName(), GameUtil.GetFormattedMass(CarePackageQuantity));

			var info = EdiblesManager.GetFoodInfo(CarePackageID);

			return info != null && info.CaloriesPerUnit > 0
				? string.Format((string)global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT, Assets.GetPrefab((Tag)CarePackageID).GetProperName(), GameUtil.GetFormattedCaloriesForItem((Tag)CarePackageID, CarePackageQuantity))
				: string.Format((string)global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT, Assets.GetPrefab((Tag)CarePackageID).GetProperName(), CarePackageQuantity.ToString());
		}

		public static string GetSpawnableDescription(string CarePackageID)
		{
			Element element = ElementLoader.GetElement(CarePackageID.ToTag());
			if (element != null)
				return element.Description();
			GameObject prefab = Assets.GetPrefab((Tag)CarePackageID);
			if (prefab == null)
				return "";
			InfoDescription component = prefab.GetComponent<InfoDescription>();
			return component != null ? component.description : prefab.GetProperName();
		}
	}
}

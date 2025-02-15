using Klei.AI;
using SettingsSyncGroups.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SettingsSyncGroups
{
	internal class ModAssets
	{
		public static GameObject GroupSelectionSidescreenGO; //This is required to keep the GO of GroupSelectionSidescreen from getting GCed!
		public static KScreen GroupSelectionSidescreen;
		public static GameObject CurrentGroupSidescreenGO; 
		public static void LoadAssets()
		{
			SgtLogger.l("Loading UI from asset bundle");
			AssetBundle bundle = AssetUtils.LoadAssetBundle("buildingsettingsync_assets", platformSpecific: true);
			//AssetBundle bundle = AssetUtils.LoadAssetBundle("rocketryexpanded_ui_assets", platformSpecific: true);


			//var DupeTransferSecondarySideScreenWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/DockingTransferScreen.prefab");


			//SgtLogger.Assert("DupeTransferSecondarySideScreenWindowPrefab", DupeTransferSecondarySideScreenWindowPrefab);

			//GroupSelectionSidescreen = DupeTransferSecondarySideScreenWindowPrefab.AddComponent<KScreen>();
			//return;


			GroupSelectionSidescreenGO = bundle.LoadAsset<GameObject>("Assets/UIs/GroupAssignment_SecondarySidescreen.prefab");
			CurrentGroupSidescreenGO = bundle.LoadAsset<GameObject>("Assets/UIs/BuildingGroupSideScreen.prefab");
			SgtLogger.Assert("GroupSelectionSidescreenGO", GroupSelectionSidescreenGO);
			SgtLogger.Assert("CurrentGroupSidescreenGO", CurrentGroupSidescreenGO);

			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(GroupSelectionSidescreenGO);
			TMPConverter.ReplaceAllText(CurrentGroupSidescreenGO);

			GroupSelectionSidescreen = GroupSelectionSidescreenGO.AddComponent<SyncGroupCarrier_SecondarySidescreen>();			
		}
	}
}

using BlueprintsV2.BlueprintsV2.BlueprintData;
using BlueprintsV2.ModAPI;
using KSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BlueprintsV2.BlueprintData
{
	public class UnderConstructionDataTransfer : KMonoBehaviour
	, ISidescreenButtonControl
	{
		public static Dictionary<Tuple<int, ObjectLayer>, UnderConstructionDataTransfer> RegisteredTransferPlans = new();


		[Serialize]
		[SerializeField]
		public Dictionary<string, string> ToApplyData = new();

		[MyCmpGet]
		public BuildingUnderConstruction building;

		Tuple<int, ObjectLayer> currentPos;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			currentPos = new(Grid.PosToCell(this), building.Def.ObjectLayer);
			RegisteredTransferPlans[currentPos] = this;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			//Subscribe((int)GameHashes.SelectObject, OnSelectObject);
		}

		public override void OnCleanUp()
		{
			RegisteredTransferPlans.Remove(currentPos);
			//Unsubscribe((int)GameHashes.SelectObject, OnSelectObject);
			base.OnCleanUp();
		}

		public void SetDataToApply(string id, JObject value)
		{
			//SgtLogger.l("registering stored data for " + id);
			if (id.IsNullOrWhiteSpace() || value == null)
			{
				//SgtLogger.l("data was null");
				return;
			}
			//SgtLogger.l("registering stored data for " + id);
			ToApplyData[id] = JsonConvert.SerializeObject(value);
		}

		public Dictionary<string, string> GetStoredData()
		{
			return new(ToApplyData);
		}

		public static void TransferDataTo(GameObject targetBuilding, Dictionary<string, string> toApply)
		{
			foreach (var data in toApply)
			{
				API_Methods.TryApplyingStoredData(targetBuilding, data.Key, JObject.Parse(data.Value));
			}
		}
		#region DataSetter
		public string SidescreenButtonText => STRINGS.UI.PRECONFIGURE_UNDERCONSTRUCTION.TITLE;
		public string SidescreenButtonTooltip => STRINGS.UI.PRECONFIGURE_UNDERCONSTRUCTION.TOOLTIP;
		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
		}

		public bool SidescreenEnabled() => UnderConstructionDataSettingHelper.HasDataTransferComponents(building);

		public bool SidescreenButtonInteractable() => true;

		public void OnSidescreenButtonPressed()
		{
			UnderConstructionDataSettingHelper.StartEditingUnderConstructionData(this);
		}

		public int HorizontalGroupID() => -1;

		public int ButtonSideScreenSortOrder() => 22;
		#endregion
	}
}

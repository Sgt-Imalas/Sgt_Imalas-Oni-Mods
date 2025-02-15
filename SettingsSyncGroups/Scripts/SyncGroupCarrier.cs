using KSerialization;
using Newtonsoft.Json.Linq;
using SettingsSyncGroups.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ModInfo;
using static SettingsSyncGroups.STRINGS.UI;

namespace SettingsSyncGroups.Scripts
{
	internal class SyncGroupCarrier : KMonoBehaviour
	{

		#region blueprints_integration
		public static void Blueprints_SetData(GameObject source, JObject data)
		{
			if (source.TryGetComponent<SyncGroupCarrier>(out var behavior))
			{
				var t1 = data.GetValue("GroupName");
				if (t1 == null)
					return;
				var GroupName = t1.Value<string>();
				behavior.SetAndSyncFromGroup(GroupName);

			}
		}
		public static JObject Blueprints_GetData(GameObject source)
		{
			if (source.TryGetComponent<SyncGroupCarrier>(out var behavior))
			{
				behavior.TryGetGroupName(out string groupName);	
				return new JObject()
				{
					{ "GroupName", groupName},
				};
			}
			return null;
		}
		#endregion

		private int subscriptionHandle;
		public static Dictionary<Tag, List<SyncGroupCarrier>> AllCarriersByTag = new();

		[MyCmpReq]
		KPrefabID kPrefabID;
		[MyCmpGet]
		CopyBuildingSettings copyBuildingSettings;
		[MyCmpGet]
		BuildingComplete buildingComplete;
		[MyCmpGet]
		KSelectable selectable;

		[Serialize]
		private string _assignedGroupName;


		public Tag BatchTag => copyBuildingSettings.copyGroupTag.IsValid ? copyBuildingSettings.copyGroupTag : kPrefabID.PrefabTag;

		public bool IsValid => copyBuildingSettings != null && !copyBuildingSettings.IsNullOrDestroyed() && buildingComplete != null && !buildingComplete.IsNullOrDestroyed();

		internal static int GetNumberOfEntries(string groupId)
		{
			return 0;
		}

		public List<SyncGroupCarrier> GetAllOtherCarriersOfGroup(string groupName)
		{
			var allCarriersOfType = AllCarriersByTag[BatchTag];
			if (allCarriersOfType == null || !TryGetGroupName(out string myGroup))
				return new();

			var result = new List<SyncGroupCarrier>();

			for (int i = 0; i < allCarriersOfType.Count; i++)
			{
				var carrier = allCarriersOfType[i];
				if (carrier == null || carrier == this)
					continue;
				if (!carrier.TryGetGroupName(out var carrierGroup) || myGroup != carrierGroup)
					continue;

				result.Add(carrier);
			}
			return result;
		}

		public void SetAndSyncFromGroup(string newGroup)
		{
			_assignedGroupName = newGroup;
			if (TryGetGroupName(out var groupName))
			{
				var otherCarriers = GetAllOtherCarriersOfGroup(groupName);

				if (otherCarriers.Any())  //if there is a group already, sync settings from that group
				{
					var source = otherCarriers.First();
					SgtLogger.l("Applying from existing group: " + groupName);
					_assignedGroupName = newGroup;
					this.Trigger((int)GameHashes.CopySettings, source.gameObject);
					SelectTool.Instance.Select(null);
					SelectTool.Instance.Select(selectable);
				}
				else
					SgtLogger.l("no existing carrier of group " + groupName + " found");

			}
		}
		public bool TryGetGroupName(out string name)
		{
			name = string.Empty;
			if (_assignedGroupName.IsNullOrWhiteSpace())
			{
				return false;
			}
			name = _assignedGroupName;
			return true;
		}
		public bool TryGetFullGroupId(out string groupId)
		{
			groupId = string.Empty;
			if (!IsValid || _assignedGroupName.IsNullOrWhiteSpace())
				return false;
			groupId = BatchTag + _assignedGroupName;

			return true;
		}
		public string GetDescriptionText()
		{
			if (TryGetFullGroupId(out string groupId))
			{
				int count = GetNumberOfEntries(groupId);
				return _assignedGroupName;
			}
			else
				return STRINGS.UI.GROUPASSIGNMENT_SECONDARYSIDESCREEN.NO_GROUP_ASSIGNED;
		}

		public static int GroupCarrierCount(Tag BatchTag, string groupNameToCount)
		{
			if (AllCarriersByTag.TryGetValue(BatchTag, out var syncGroupCarriers))
			{
				int count = 0;
				foreach (var entry in syncGroupCarriers)
				{
					if (entry.TryGetFullGroupId(out string group) && group == groupNameToCount)
						++count;
				}
				return count;
			}
			return 0;
		}

		internal Dictionary<string, int> GetCurrentlyExistingGroups()
		{
			var allCarriersOfType = AllCarriersByTag[BatchTag];
			if (allCarriersOfType == null)
				return new();

			var result = new Dictionary<string, int>();

			for (int i = 0; i < allCarriersOfType.Count; i++)
			{
				var carrier = allCarriersOfType[i];
				if (carrier == null) continue;
				if (carrier.TryGetGroupName(out string group))
				{
					if (result.ContainsKey(group))
						result[group]++;
					else
						result[group] = 1;
				}

			}
			return result;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (AllCarriersByTag.TryGetValue(BatchTag, out var carrierList))
				carrierList.Add(this);
			else
				AllCarriersByTag[BatchTag] = [this];

			subscriptionHandle = Subscribe((int)GameHashes.CopySettings, OnCopySettings);
		}
		public override void OnCleanUp()
		{
			Unsubscribe(subscriptionHandle);
			if (AllCarriersByTag.TryGetValue(BatchTag, out var carrierList))
				carrierList.Remove(this);
			base.OnCleanUp();
		}
		private void OnCopySettings(object obj)
		{
			if (obj is GameObject building && building.TryGetComponent<SyncGroupCarrier>(out var sourceCarrier) && sourceCarrier != this)
			{
				//no need to do group transfer because thats happening in parallel already due to the normal copySettings
				sourceCarrier.TryGetGroupName(out var newName); 
				this._assignedGroupName = newName;
			}
		}

		internal static void SynchronizeAll(SyncGroupCarrier source)
		{
			if (source == null || !source.IsValid)
				return;

			if (!source.TryGetGroupName(out string SourceGroup))
				return;

			var batch = source.BatchTag;

			SgtLogger.l("Syncing all entities of tag: " + batch + " inside the group \"" + SourceGroup+"\"");

			if (AllCarriersByTag.TryGetValue(batch, out var carrierList))
			{
				foreach (var targetCarrier in carrierList)
				{
					if (targetCarrier == source || targetCarrier == null || targetCarrier.gameObject == null || targetCarrier.gameObject.IsNullOrDestroyed())
						continue;

					if (!targetCarrier.TryGetGroupName(out string targetGroup) || targetGroup != SourceGroup)
						continue;

					targetCarrier.Trigger((int)GameHashes.CopySettings, source.gameObject);
				}
			}
		}
	}
}

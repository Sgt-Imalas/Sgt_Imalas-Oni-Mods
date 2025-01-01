using SetStartDupes.CarePackageEditor.UnlockConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SetStartDupes.CarePackageEditor
{
	public class CarePackageOutline
	{
		public string ItemId;
		public string Name;
		public List<string> RequiredDlcs = null;
		public int Amount;
		public List<List<ICarePackageUnlockCondition>> UnlockConditions = null;

		public static bool IsValidCarePackageId(string id, out string failReason)
		{
			failReason = null;

			if (Assets.GetPrefab(id) == null)
			{
				failReason = ("Could not find a valid item with the Id: " + id);
				return false;
			}
			return true;
		}
		public CarePackageOutline(string itemId, int amount, List<ICarePackageUnlockCondition> unlockConditions)
		{
			ItemId = itemId;
			Name = Assets.GetPrefab(itemId)?.GetProperName() ?? null;
			Amount = amount;
			UnlockConditions = new() { unlockConditions };
		}
		public static CarePackageOutline ElementCarePackage(SimHashes elementHash, int amount = 1)
		{
			var element = ElementLoader.FindElementByHash(elementHash);
			if (element == null)
				return new CarePackageOutline(elementHash.ToString(), amount);
			else
				return new CarePackageOutline(element.tag.ToString(), amount);
		}
		public CarePackageOutline(string itemId, int amount = 1)
		{
			ItemId = itemId;
			SgtLogger.l(ItemId);
			Name = Assets.GetPrefab(itemId)?.GetProperName() ?? null;
			SgtLogger.l(Name);
			Amount = amount;
			UnlockConditions = null;
		}
		public CarePackageOutline DiscoverCondition(string targetId = null)
		{
			SgtLogger.l("DiscoverCondition");
			if (targetId == null)
				targetId = ItemId;

			if (UnlockConditions == null)
				UnlockConditions = [new(8)];

			UnlockConditions.Last().Add(new ItemDiscoveredCondition(targetId));
			return this;
		}
		public CarePackageOutline DiscoverElementCondition(SimHashes elementId)
		{
			if (UnlockConditions == null)
				UnlockConditions = [new(8)];
			var element = ElementLoader.FindElementByHash(elementId);
			if (element == null)
				UnlockConditions.Last().Add(new ItemDiscoveredCondition(elementId.ToString()));
			else
				UnlockConditions.Last().Add(new ItemDiscoveredCondition(element.tag.ToString()));
			return this;
		}
		public CarePackageOutline CycleCondition(int cycle)
		{
			SgtLogger.l("CycleCondition");
			if (UnlockConditions == null)
				UnlockConditions = [new()];
			UnlockConditions.Last().Add(new CycleUnlockCondition(cycle));
			return this;
		}
		public CarePackageOutline DlcRequired(string dlc)
		{
			if (RequiredDlcs == null)
				RequiredDlcs = new(8);
			RequiredDlcs.Add(dlc);
			return this;
		}
		public CarePackageOutline OR()
		{
			if (UnlockConditions == null)
				UnlockConditions = [new()];
			else
				UnlockConditions.Add(new(8));
			return this;
		}

		public CarePackageOutline(CarePackageInfo sourcePackage)
		{
			SgtLogger.l(sourcePackage.id);
			ItemId = sourcePackage.id;
			Amount = Mathf.RoundToInt(sourcePackage.quantity);
			Name = Assets.GetPrefab(ItemId)?.GetProperName() ?? null;

			UnlockConditions = null;

		}


		public static bool TryCreateCarePackageOutline(string outlineId, out CarePackageOutline carePackageOutline, out string failed)
		{
			carePackageOutline = null;
			failed = null;
			if (!IsValidCarePackageId(outlineId, out failed))
			{
				return false;
			}
			carePackageOutline = new(outlineId);
			return true;
		}

		public bool IsValidAsCarePackage(out string failReason)
		{
			failReason = null;

			if (!IsValidCarePackageId(ItemId, out var failed))
			{
				failReason = failed;
				return false;
			}
			if (RequiredDlcs != null)
			{
				if (!DlcManager.IsAllContentSubscribed(RequiredDlcs))
				{
					failReason = "missing owned dlc: " + string.Join(", ", RequiredDlcs);
					return false;
				}

				var saveLoader = SaveLoader.Instance;
				if (saveLoader != null && saveLoader.GameInfo.dlcIds != null)
				{
					var activeDLCIDs = saveLoader.GameInfo.dlcIds;

					foreach (var dlc in RequiredDlcs)
					{
						if (!activeDLCIDs.Contains(dlc))
						{
							failReason = "missing active dlc: " + string.Join(", ", RequiredDlcs);
							return false;
						}
					}
				}
			}
			if (Amount <= 0)
			{
				failReason = "item amount needs to be larger than 0";
				return false;
			}

			return true;
		}
		public bool TryMakeCarePackageInfo(out CarePackageInfo carePackageInfo)
		{
			carePackageInfo = null;
			bool hasUnlockConditions = false;
			Func<bool> UnlockCondition = null;
			if (UnlockConditions != null && UnlockConditions.Count > 0)
			{
				hasUnlockConditions = true;
			}
			if (!IsValidAsCarePackage(out var failReason))
			{
				SgtLogger.log("skipping care package for " + ItemId + ": " + failReason);
				return false;
			}
			if (hasUnlockConditions)
			{
				UnlockCondition = () => Immigration.CycleCondition(500) || UnlockConditions.Any(conditionList => conditionList.All(condition => condition.UnlockConditionFulfilled()));
			}

			carePackageInfo = new CarePackageInfo(ItemId, Amount, UnlockCondition);
			return true;
		}
	}
}

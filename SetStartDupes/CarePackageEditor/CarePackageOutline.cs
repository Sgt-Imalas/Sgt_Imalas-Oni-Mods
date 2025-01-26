using SetStartDupes.CarePackageEditor.UnlockConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS;

namespace SetStartDupes.CarePackageEditor
{
	public class CarePackageOutline
	{
		public string ItemId;
		public string Name;
		public List<string> RequiredDlcs = null;
		public int Amount;
		public List<List<ICarePackageUnlockCondition>> UnlockConditions = null;


		internal string GetConditionsTooltip()
		{
			if (UnlockConditions == null || !UnlockConditions.Any())
				return STRINGS.UI.CAREPACKAGEEDITOR.UNLOCKCONDITIONTOOLTIPS.ALWAYSAVAILABLE;

			var sb = new StringBuilder();
			sb.AppendLine(STRINGS.UI.CAREPACKAGEEDITOR.UNLOCKCONDITIONTOOLTIPS.START);
			sb.AppendLine();

			for (int i = 0; i < UnlockConditions.Count; i++)
			{
				if (i > 0)
				{
					sb.AppendLine();
					sb.AppendLine(STRINGS.UI.CAREPACKAGEEDITOR.UNLOCKCONDITIONTOOLTIPS.OR);
					sb.AppendLine();
				}

				for (int j = 0; j < UnlockConditions[i].Count; j++)
				{
					if (j > 0)
					{
						sb.AppendLine(STRINGS.UI.CAREPACKAGEEDITOR.UNLOCKCONDITIONTOOLTIPS.AND);
					}

					var condition = UnlockConditions[i][j];
					if (condition is ItemDiscoveredCondition idc)
					{
						sb.AppendLine(string.Format(STRINGS.UI.CAREPACKAGEEDITOR.UNLOCKCONDITIONTOOLTIPS.DISCOVERY, CarePackageItemHelper.GetSpawnableName(idc.PrefabId)));
					}
					else if (condition is CycleUnlockCondition cu)
					{
						sb.AppendLine(string.Format(STRINGS.UI.CAREPACKAGEEDITOR.UNLOCKCONDITIONTOOLTIPS.CYCLETHRESHOLD, cu.CycleUnlock));
					}
				}
			}
			return sb.ToString();
		}

		public bool HasUIDiscoveredCondition() => GetUIConditions()?.Any(cond => cond is ItemDiscoveredCondition) ?? false;
		public bool HasUICycleCondition(out CycleUnlockCondition condition)
		{
			condition = GetUIConditions()?.FirstOrDefault(cond => cond is CycleUnlockCondition) as CycleUnlockCondition;
			return condition != null;
		}

		public void AddOrUpdateUIDiscoveredCondition()
		{
			var conditions = GetUIConditions();
			if (conditions == null || !conditions.Any(cond => cond is ItemDiscoveredCondition))
				DiscoverCondition(ItemId);
		}
		public void RemoveUIDiscoveredCondition()
		{
			var conditions = GetUIConditions();
			if (conditions == null)
				return;
			conditions.RemoveAll(condition => condition is ItemDiscoveredCondition);
		}


		public void AddOrUpdateUICycleCondition(int cycle)
		{
			var conditions = GetUIConditions();
			if (conditions == null || !conditions.Any(cond => cond is CycleUnlockCondition))
				CycleCondition(cycle);
			else
			{
				(conditions.First(item => item is CycleUnlockCondition) as CycleUnlockCondition).CycleUnlock = cycle;
			}
		}
		public void RemoveUICycleCondition()
		{
			var conditions = GetUIConditions();
			if (conditions == null)
				return;
			conditions.RemoveAll(condition => condition is CycleUnlockCondition);
		}


		public List<ICarePackageUnlockCondition> GetUIConditions() => UnlockConditions?.FirstOrDefault();
		public bool HasConditions() => UnlockConditions != null && UnlockConditions.Any() && UnlockConditions[0] != null && UnlockConditions[0].Any();

		public Tuple<Sprite, Color> GetImageWithColor()
		{
			var TargetItem = Assets.GetPrefab(ItemId);
			if (TargetItem != null)
			{
				SgtLogger.l(TargetItem.GetProperName());
				var image = Def.GetUISprite(TargetItem);
				if (image != null)
				{
					return image;
				}
			}
			return new(Assets.GetSprite("unknown"), Color.white);
		}
		public string GetDescriptionString()
		{
			var item = Assets.GetPrefab(ItemId);
			if (item == null)
			{
				//spaced out package in base game, using stored name backup or id;
				string displayName = Name ?? ItemId;

				return string.Format((string)global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT, displayName, Amount);
			}
			return CarePackageItemHelper.GetSpawnableQuantity(ItemId, Amount);
		}
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
		public CarePackageOutline()
		{

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
			var item = Assets.GetPrefab(sourcePackage.id);
			ItemId = sourcePackage.id;
			Amount = Mathf.RoundToInt(sourcePackage.quantity);
			Name = item?.GetProperName() ?? null;
			UnlockConditions = null;

			if(item!=null && item.TryGetComponent<KPrefabID>(out var prefabID) && prefabID.requiredDlcIds != null)
			{
				RequiredDlcs = new(prefabID.requiredDlcIds);
			}
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

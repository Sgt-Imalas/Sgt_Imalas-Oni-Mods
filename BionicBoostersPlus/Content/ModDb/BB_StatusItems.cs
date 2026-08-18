using BionicBoostersPlus.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using static BionicBoostersPlus.STRINGS.DUPLICANTS.STATUSITEMS;
using static BionicBoostersPlus.STRINGS.MISC.STATUSITEMS;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_StatusItems
	{
		public static StatusItem DreamBooster_Idle;
		public static StatusItem DreamBooster_Dreaming;

		public static StatusItem DreamBoosterJournalStorage;
		public static StatusItem DreamBoosterJournalStorageFull;

		public static StatusItem MedBooster_Repairing;

		public static void InitStatusitems(Db db)
		{
			var dsi = Db.Get().DuplicantStatusItems;
			var msi = Db.Get().MiscStatusItems;

			DreamBooster_Idle = dsi.CreateStatusItem("DreamBooster_Idle", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
			DreamBooster_Dreaming = dsi.CreateStatusItem("DreamBooster_Dreaming", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
			DreamBooster_Dreaming.resolveTooltipCallback = (string str, object obj) =>
			{
				if (obj is BionicUpgrade_DreamerBoosterMonitor.Instance smi)
				{
					return str.Replace("{time}", GameUtil.GetFormattedTime(smi.TimeToNextJournal));
				}
				return str;
			};

			DreamBoosterJournalStorage = msi.CreateStatusItem("DreamBoosterJournalStorage", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
			DreamBoosterJournalStorage.resolveStringCallback = (string str, object obj) =>
			{
				if (obj is BionicUpgrade_DreamerBooster.Instance smi)
				{
					string.Format(str, GameUtil.GetFormattedPercent(smi.Progress));
				}
				return str;
			};

			DreamBoosterJournalStorageFull = msi.CreateStatusItem("DreamBoosterJournalStorageFull", "MISC", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);

			MedBooster_Repairing = msi.CreateStatusItem("MedBooster_Repairing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);

		}
	}
}

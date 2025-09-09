using ForceFieldWallTile.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceFieldWallTile.Content.ModDb
{
	internal class ModStatusItems
	{

		public static StatusItem FFT_ShieldFullyCharged;
		public static StatusItem FFT_ShieldCharging;
		public static StatusItem FFT_ShieldOverloaded;

		public static void CreateStatusItems()
		{
			var bsi = Db.Get().BuildingStatusItems;


			FFT_ShieldFullyCharged = bsi.CreateStatusItem("FFT_ShieldFullyCharged", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			
			FFT_ShieldCharging = bsi.CreateStatusItem("FFT_ShieldCharging", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			FFT_ShieldCharging.resolveStringCallback = delegate (string str, object data)
			{
				ForceFieldTile shield = (ForceFieldTile)data;
				return string.Format(str, GameUtil.GetFormattedPercent(100*shield.ShieldStrength/shield.MaxStrenght));
			};
			FFT_ShieldOverloaded = bsi.CreateStatusItem("FFT_ShieldOverloaded", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			FFT_ShieldOverloaded.resolveStringCallback = delegate (string str, object data)
			{
				ForceFieldTile shield = (ForceFieldTile)data;
				return string.Format(str, GameUtil.GetFormattedTime(shield.OverloadCooldown));
			};
		}

	}
}

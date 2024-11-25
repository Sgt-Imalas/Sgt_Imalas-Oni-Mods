using AkisSnowThings.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Content.Scripts
{
	public class SnowStatusItems
	{
		public static StatusItem sealedStatus;

		public static void CreateStatusItems()
		{
			sealedStatus = new StatusItem(
				"SnowSculptures_SealedStatusItem",
				"BUILDINGS",
				"",
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID);

			sealedStatus.SetResolveStringCallback((str, obj) =>
			{
				if (obj is GlassCaseSealable sealable)
				{
					var text = (sealable.glassCase != null && sealable.glassCase.broken) ?
						(string)STRINGS.BUILDINGS.STATUSITEMS.SNOWSCULPTURES_SEALEDSTATUSITEM.SEALED2 :
						(string)STRINGS.BUILDINGS.STATUSITEMS.SNOWSCULPTURES_SEALEDSTATUSITEM.SEALED;

					return string.Format(str, text);
				}

				return str;
			});
		}
	}
}

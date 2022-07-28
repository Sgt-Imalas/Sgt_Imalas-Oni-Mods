using Cryopod.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopod
{
    internal class ModAssets
    {
        public static Components.Cmps<CryopodReusable> CryoPods = new Components.Cmps<CryopodReusable>();
        public class StatusItems
        {
            public static StatusItem DupeName;
            public static StatusItem CurrentDupeTemperature;

            public static void Register()
            {
                DupeName = new StatusItem(
                   "CRY_DuplicantNameStatus",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Info,
                   NotificationType.Neutral,
                   false,
                   OverlayModes.None.ID);
                CurrentDupeTemperature = new StatusItem(
                   "CRY_DuplicantInternalTemperature",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Info,
                   NotificationType.Neutral,
                   false,
                   OverlayModes.None.ID
                   );

                DupeName.SetResolveStringCallback((str, obj) =>
                {
                    if (obj is CryopodReusable cryopod)
                    {
                        string dupeName = cryopod.GetDupeName();
                        return str.Replace("{DupeName}", dupeName);
                    }
                    return str;
                });

                CurrentDupeTemperature.SetResolveStringCallback((str, obj) =>
                {
                    if (obj is CryopodReusable cryopod)
                    {
                        float dupeTemp = cryopod.InternalTemperatureKelvin;
                        return str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(dupeTemp));
                    }
                    return str;
                });

            }
        }
    }
}

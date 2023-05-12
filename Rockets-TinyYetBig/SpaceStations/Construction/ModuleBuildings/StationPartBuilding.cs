using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.SpaceStations.Construction.ModuleBuildings
{
    public class StationPartBuilding : KMonoBehaviour
    {

        [MyCmpReq]
        public AttachableBuilding attatchableBuilding;


        [Serialize]
        public int PositionIndex = -1;
        [Serialize]
        public bool destroyOnConvert = true;
        [Serialize]
        public Tag DropID { get; private set; }
        [Serialize]
        public float DropAmount { get; private set; }
        [Serialize]
        public int ModuleParts { get; private set; }

        [Serialize]
        public string PartID;
        public void SetStationPart(string partID)
        {
            PartID = partID;
        }
        public void ApplyStationPart (StationPart part)
        {
            SgtLogger.l(part.PartBuildingAmount.ToString(), nameof(part.PartBuildingAmount));
            SgtLogger.l(part.SplitsInto.ToString(), nameof(part.SplitsInto));
            SgtLogger.l(part.DropPresetID.ToString(), nameof(part.DropPresetID));

            ModuleParts = part.PartBuildingAmount;
            DropAmount = part.SplitsInto;
            DropID = part.DropPresetID;
        }
        

        public override void OnSpawn()
        {
            base.OnSpawn();
            ApplyStationPart(ModuleParts_Central.StationParts[PartID]);

        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();

        }
        public void DestroyWithoutDrops()
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }


        public bool PartComplete()
        {
            int PartCount = 0;
            int TargetPartCount = 0;
            foreach (var part in AttachableBuilding.GetAttachedNetwork(attatchableBuilding))
            {
                if (part.TryGetComponent<StationPartBuilding>(out var partBuilding))
                {
                    if (partBuilding.destroyOnConvert)
                    {
                        TargetPartCount = partBuilding.ModuleParts;
                        PartCount++;
                    }
                }
            }
            return PartCount == TargetPartCount && TargetPartCount > 0;
        }
    }
}

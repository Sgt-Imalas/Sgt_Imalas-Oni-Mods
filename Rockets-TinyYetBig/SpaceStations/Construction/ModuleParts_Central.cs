using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    public class ModuleParts_Central
    {
        public static Tag RTB_Part_Base = nameof(RTB_Part_Base);
        public static Tag RTB_Part_A_1 = nameof(RTB_Part_A_1);
        public static Tag RTB_Part_A_2 = nameof(RTB_Part_A_2);
        public static Tag RTB_Part_A_3 = nameof(RTB_Part_A_3);

        public static Tag RTB_Part_B_1 = nameof(RTB_Part_B_1);
        public static Tag RTB_Part_B_2 = nameof(RTB_Part_B_2);
        public static Tag RTB_Part_B_3 = nameof(RTB_Part_B_3);

        public static Tag RTB_Part_C_1 = nameof(RTB_Part_C_1);
        public static Tag RTB_Part_C_2 = nameof(RTB_Part_C_2);
        public static Tag RTB_Part_C_3 = nameof(RTB_Part_C_3);

        public static Tag RTB_Part_D_1 = nameof(RTB_Part_D_1);
        public static Tag RTB_Part_D_2 = nameof(RTB_Part_D_2);
        public static Tag RTB_Part_D_3 = nameof(RTB_Part_D_3);





        public static Dictionary<string, StationPart> StationParts = new Dictionary<string, StationPart>()
        {
            {
                "PartCostA",

                new StationPart(
          new string[]
                {
                "RefinedMetal"
                },
                new float[]
                {
                    600
                },
            3,
            GeneShufflerRechargeConfig.tag,
            20)
            }

        };
    }
    public struct StationPart
    {
        public StationPart(string[] _mats, float[] _costs, int _parts, Tag _dropPresetID, int _splitInto = 0)
        {
            this.Materials = _mats;
            this.MaterialCosts = _costs;
            this.partBuildingAmount = _parts;
            this.splitsInto = _splitInto;
            this.DropPresetID = _dropPresetID;

            if (_splitInto == 0)
            {
                this.splitsInto = partBuildingAmount * 10;
            }
        }

        string[] Materials;
        float[] MaterialCosts;
        int partBuildingAmount;
        int splitsInto;
        public Tag DropPresetID;
        public int PartBuildingAmount => partBuildingAmount;
        public int SplitsInto => splitsInto;
        public string[] SplitMaterials()
        {
            return Materials;
        }
        public float[] SplitMaterialCosts()
        {
            var newfloat = new List<float>();
            foreach (var mat in MaterialCosts)
            {
                newfloat.Add(mat / (float)partBuildingAmount);
            }
            return newfloat.ToArray();
        }
    }
}

using BlueprintsV2.ModAPI;
using Klei.AI;
using KSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ResearchTypes;

namespace BlueprintsV2.BlueprintData
{
    internal class UnderConstructionDataTransfer : KMonoBehaviour
    {
        public static Dictionary<Tuple<int, ObjectLayer>, UnderConstructionDataTransfer> RegisteredTransferPlans = new();

        
        [Serialize]
        [SerializeField]
        public Dictionary<string, string> ToApplyData = new();

        [MyCmpGet]
        public Building building;

        Tuple<int, ObjectLayer> currentPos;


        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            currentPos = new(Grid.PosToCell(this), building.Def.ObjectLayer);
            RegisteredTransferPlans[currentPos] = this;
        }
        public override void OnCleanUp()
        {
            RegisteredTransferPlans.Remove(currentPos);
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

        public Dictionary<string,string> GetStoredData()
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
    }
}

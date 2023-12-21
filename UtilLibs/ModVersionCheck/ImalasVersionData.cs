using Newtonsoft.Json;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.ModVersionCheck
{
    public class ImalasVersionData
    {
        [JsonIgnore]
        public const string ModVersionDataKey = "Sgt_Imalas_ModVersionData";
        [JsonIgnore]
        public const string CurrentlyFetchingKey = "Sgt_Imalas_ModVersionData_CurrentlyFetching";

        //public static async void FetchVersionData()
        //{
        //    var data = PRegistry.GetData<ImalasVersionData>(ModVersionDataKey);
        //    if(data == null && PRegistry.GetData<bool>(CurrentlyFetchingKey) == false)
        //    {
        //        PRegistry.PutData(CurrentlyFetchingKey,true);

        //        SgtLogger.l("Mod Version Data was null, trying to fetch it");
        //        using (var client = new HttpClient())
        //        {
        //            HttpResponseMessage response = await client.GetAsync("http://www.contoso.com/");
        //            response.EnsureSuccessStatusCode();
        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            var FoundData = JsonConvert.DeserializeObject<ImalasVersionData>(responseBody);
        //            if(FoundData!=null)
        //                PRegistry.PutData(ModVersionDataKey, FoundData);
        //            PRegistry.PutData(CurrentlyFetchingKey, false);
        //        }
        //    }
        //}

    }
}

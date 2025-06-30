using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.Update
{
    public class FetchableRepoInfo
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public string Name;
		public string UpdateIndexUrl;
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public string Url;

        public FetchableRepoInfo() 
        { 
            if(string.IsNullOrEmpty(Name))
				Name = "Unnamed Repo";
		}
        public FetchableRepoInfo(string name, string url)
        {
            Name = name;
            UpdateIndexUrl = url;
			Url = url.Substring(0, url.LastIndexOf("/"));
        }
    }
}

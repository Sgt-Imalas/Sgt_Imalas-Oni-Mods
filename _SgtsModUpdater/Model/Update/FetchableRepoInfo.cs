using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.Update
{
    public class FetchableRepoInfo
	{
		public string UpdateIndexName;
		public string UpdateIndexURL;
		public string RepoUrl;
        public FetchableRepoInfo()
		{
			InferMissing();
		}
        public void InferMissing()
        {
			if (string.IsNullOrEmpty(UpdateIndexName))
				UpdateIndexName = "Unnamed Repo";
			if (string.IsNullOrEmpty(RepoUrl) && !string.IsNullOrEmpty(UpdateIndexURL))
				RepoUrl = UpdateIndexURL.Substring(0, UpdateIndexURL.LastIndexOf("/"));
		}
        public FetchableRepoInfo(string name, string url)
        {
            UpdateIndexName = name;
            UpdateIndexURL = url;
			RepoUrl = url.Substring(0, url.LastIndexOf("/"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.Update
{
    public class FetchableRepoInfo
    {
        public string Name;
        public string ReleaseInfo;
        public string Url;
        public FetchableRepoInfo(string name, string url)
        {
            Name = name;
			Url = url.Substring(0, url.LastIndexOf("/"));
            ReleaseInfo = url;

        }
    }
}

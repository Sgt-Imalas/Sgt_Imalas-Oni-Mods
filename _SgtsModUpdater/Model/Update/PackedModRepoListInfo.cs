﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.Update
{
    public class PackedModRepoListInfo : ILocalRepoInfo
	{
        public string RepoName => _repoName;
        string _repoName = "";
    }
}

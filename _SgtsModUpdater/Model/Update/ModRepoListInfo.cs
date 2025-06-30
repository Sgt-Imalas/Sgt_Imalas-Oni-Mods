using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.Update
{
	public class ModRepoListInfo : ILocalRepoInfo
	{
		public string RepoName => _repoName;
		public string RepoUrl => _repoUrl;

		string _repoName, _repoUrl;
		public ModRepoListInfo(string repoName, string repoUrl)
		{
			_repoName = repoName;
			_repoUrl = repoUrl;
		}

		public ObservableCollection<RemoteMod> Mods = new();
		public override string ToString()
		{
			return string.Format("{0}, {1} mods", RepoName, Mods.Count);
		}
		public string UIEntry => ToString();
	}
}

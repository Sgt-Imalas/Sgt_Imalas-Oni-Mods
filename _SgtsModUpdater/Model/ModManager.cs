using _SgtsModUpdater.Model.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model
{
    class ModManager
    {
        private static ModManager _instance;
        public static ModManager Instance
        {
            get 
            {
                if(_instance == null)
				{
					_instance = new ModManager();
				}
				return _instance;            
            }
        }
        public List<ModRepoListInfo> Repos = new();
	}
}

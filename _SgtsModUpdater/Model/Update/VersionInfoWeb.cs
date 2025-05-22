using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.Update
{
	public class VersionInfoWeb
	{
		public string staticId;
		public string version;
		public string minimumSupportedBuild;

		public Version Version
		{
			get
			{
				if (string.IsNullOrEmpty(version))
					return new Version(0, 0, 0, 0);
				return new Version(version);
			}
		}
		public bool IsNewVersionAvailable(Version currentVersion)
		{
			if (currentVersion == null)
				return false;
			if (currentVersion >= Version)
				return false;
			return true;
		}
	}
}

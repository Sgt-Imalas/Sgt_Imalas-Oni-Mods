using LocalModLoader.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace LocalModLoader
{
	internal class VersionCheck
	{
		public static bool CheckForUpdate(out RemoteModInfo info)
		{
			info = null;
			uint currentGameVersion = WebRequestHelper.GetGameVersion();
			string ownVersion = Mod.Instance.mod.packagedModInfo.version;

			if (WebRequestHelper.GetRemoteVersionInfo(Mod.Info.VersionInfoUrl, out info)
				&& uint.TryParse(info.minimumSupportedBuild, out var targetMinSupportVersion)
				&& targetMinSupportVersion <= currentGameVersion)
			{
				string remoteVersion = info.version;
				return IsNewerVersionAvailable(ownVersion, remoteVersion);
			}
			else if (WebRequestHelper.GetRemoteVersionInfo(Mod.Info.PrevVersionInfoUrl, out info)
				&& uint.TryParse(info.minimumSupportedBuild, out var prevTargetMinSupportVersion)
				&& prevTargetMinSupportVersion <= currentGameVersion)
			{
				string remoteVersion = info.version;
				return IsNewerVersionAvailable(ownVersion, remoteVersion);
			}
			SgtLogger.l("could not find a viable remote version");
			return false;
		}
		static bool IsNewerVersionAvailable(string ownVersion, string remoteVersion)
		{
			SgtLogger.l("own: " + ownVersion + ", remote: " + remoteVersion);

			if (System.Version.TryParse(remoteVersion, out var FetchableVersion) &&
				System.Version.TryParse(ownVersion, out var LocalVersion))
			{
				return FetchableVersion > LocalVersion;
			}
			return ownVersion != remoteVersion;
		}
	}
}

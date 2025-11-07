using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalModLoader
{
	public class TargetModInfo
	{
		/// <summary>
		/// debug purpose
		/// </summary>
		public string TargetStaticID = "ClusterTraitGenerationManager";

		public uint CurrentVersion = KleiVersion.ChangeList;

		public string VersionInfoUrl = @"https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/download/AllMods_Automated_Build_FullRelease/_ReleaseVersionInfoData.json";
		public string CurrentVersionURL = @"https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/download/AllMods_Automated_Build_FullRelease/ClusterTraitGenerationManager.zip";

		public string PrevVersionInfoUrl = @"https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/download/AllMods_U54/_ReleaseVersionInfoData.json";
		public string PrevBranchUrl = @"https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/download/AllMods_U54/ClusterTraitGenerationManager.zip";
	}
}

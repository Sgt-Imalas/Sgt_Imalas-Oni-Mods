using System.Collections.Generic;

namespace SetStartDupes.CarePackageEditor
{
    public class VanillaCarePackageInformation
    {
        public HashSet<VanillaDisabledCarePackageInfo> DisabledVanillaCarePackages = [];

		internal bool CarePackageDisabled(CarePackageInfo entry)
		{
			var info = VanillaDisabledCarePackageInfo.CreateFromPackage(entry);
			return DisabledVanillaCarePackages.Contains(info);

		}
		internal bool CarePackageDisabled(CarePackageOutline entry)
		{
			var info = VanillaDisabledCarePackageInfo.CreateFromOutline(entry);
			return DisabledVanillaCarePackages.Contains(info);
		}

		internal void ClearAll()
		{
			DisabledVanillaCarePackages.Clear();
		}

		internal int GetCount()
		{
			return DisabledVanillaCarePackages.Count;
		}

		internal void Initialize()
		{

		}

		internal bool ToggleVanillaCarePackage(CarePackageOutline vanillaOutline)
		{
			var info = VanillaDisabledCarePackageInfo.CreateFromOutline(vanillaOutline);
			if(DisabledVanillaCarePackages.Contains(info))
				DisabledVanillaCarePackages.Remove(info);
			else
				DisabledVanillaCarePackages.Add(info);

			return DisabledVanillaCarePackages.Contains(info);
		}
	}
}

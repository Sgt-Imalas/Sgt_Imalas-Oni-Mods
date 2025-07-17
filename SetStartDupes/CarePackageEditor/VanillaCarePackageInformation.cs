using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			//throw new NotImplementedException();
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

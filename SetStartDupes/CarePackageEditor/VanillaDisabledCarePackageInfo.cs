using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.CarePackageEditor
{
    /// <summary>
    /// find vanilla care package by these identifiers
    /// </summary>
    public class VanillaDisabledCarePackageInfo : IEquatable<VanillaDisabledCarePackageInfo>
	{
		public bool Equals(VanillaDisabledCarePackageInfo other)
		{
			return other?.TargetID == this.TargetID && other?.TargetAmount == this.TargetAmount;
		}
		public override bool Equals(object obj) => obj is VanillaDisabledCarePackageInfo other && Equals(other);

		public static bool operator ==(VanillaDisabledCarePackageInfo a, VanillaDisabledCarePackageInfo b) => a?.TargetID == b?.TargetID && a?.TargetAmount == b?.TargetAmount;
		public static bool operator !=(VanillaDisabledCarePackageInfo a, VanillaDisabledCarePackageInfo b) => !(a == b);
		public override int GetHashCode()
		{
			var val = TargetID.GetHashCode() ^ TargetAmount.GetHashCode();
			return val;
		}
		public string TargetID;
		public float TargetAmount;
        public VanillaDisabledCarePackageInfo(string id, float amount)
        {
            TargetAmount = amount;
            TargetID = id;
        }

        public static VanillaDisabledCarePackageInfo CreateFromOutline(CarePackageOutline source)
        {
			return new(source.ItemId, source.Amount);
		}
		public static VanillaDisabledCarePackageInfo CreateFromPackage(CarePackageInfo source)
		{
			return new(source.id, source.quantity);
		}
	}
}

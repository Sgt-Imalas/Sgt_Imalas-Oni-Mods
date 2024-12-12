using Database;
using Klei.AI;
using System.Collections.Generic;
using System.Linq;

namespace SetStartDupes.DuplicityEditing.Helpers
{
	public static class AmountHelper
	{

		class AmountEqualityComparer : IEqualityComparer<Amount>
		{
			public bool Equals(Amount x, Amount y)
			{
				return x.Id == y.Id;
			}

			public int GetHashCode(Amount obj)
			{
				return obj.Id.GetHashCode();
			}
		}

		public static List<Amount> GetAllEditableAmounts()
		{
			return GetNormalEditableAmounts().Concat(GetBionicEditableAmounts()).Distinct(new AmountEqualityComparer()).ToList();
		}

		public static bool IsValidModelAmount(Amount amount, Tag model)
		{
			if (model == GameTags.Minions.Models.Bionic)
				return IsBionicAmount(amount);
			else
				return IsNormalAmount(amount);
		}
		public static List<Amount> GetAmountsForModel(Tag model)
		{
			if (model == GameTags.Minions.Models.Bionic)
				return GetBionicEditableAmounts();
			else
				return GetNormalEditableAmounts();
		}

		public static bool IsNormalAmount(Amount a) => _normalAmountsHashSet.Contains(a);
		public static bool IsBionicAmount(Amount a) => _bionicAmountsHashSet.Contains(a);

		private static List<Amount> _normalAmounts = null;
		private static HashSet<Amount> _normalAmountsHashSet = null;
		public static List<Amount> GetNormalEditableAmounts()
		{
			if (_normalAmounts == null)
			{
				var AmountIds = MinionConfig.GetAmounts();
				_normalAmounts = new List<Amount>();
				var amounts = Db.Get().Amounts;
				foreach (var amountId in AmountIds)
				{
					var amountVal = amounts.Get(amountId);

					if (amountVal.Name.Contains("MISSING.STRINGS"))
						continue;

					_normalAmounts.Add(amountVal);
				}
				_normalAmountsHashSet = _normalAmounts.ToHashSet();
			}
			return _normalAmounts;
		}
		private static List<Amount> _bionicAmounts = null;
		private static HashSet<Amount> _bionicAmountsHashSet = null;
		public static List<Amount> GetBionicEditableAmounts()
		{
			if (_bionicAmounts == null)
			{
				var AmountIds = BionicMinionConfig.GetAmounts();
				_bionicAmounts = new List<Amount>();
				var amounts = Db.Get().Amounts;
				foreach (var amountId in AmountIds)
				{
					var amountVal = amounts.Get(amountId);

					if (amountVal.Name.Contains("MISSING.STRINGS"))
						continue;

					_bionicAmounts.Add(amountVal);
				}
				_bionicAmountsHashSet = _bionicAmounts.ToHashSet();
			}
			return _bionicAmounts;
		}
	}

}

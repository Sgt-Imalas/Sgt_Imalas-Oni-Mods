using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class CustomOxidizers
	{
		private static Dictionary<Tag, float> oxidizerEfficiencies;
		public static Dictionary<Tag, float> GetOxidizerEfficiencies()
		{
			if (oxidizerEfficiencies == null)
			{
				oxidizerEfficiencies = new Dictionary<Tag, float>()
					{
						{ ModAssets.Tags.OxidizerEfficiency_1, 1 },
						{ ModAssets.Tags.OxidizerEfficiency_2, 2 },
						{ ModAssets.Tags.OxidizerEfficiency_3, 3 },
						{ ModAssets.Tags.OxidizerEfficiency_4, 4 },
						{ ModAssets.Tags.OxidizerEfficiency_5, 5 },
						{ ModAssets.Tags.OxidizerEfficiency_6, 6 },
					};
			}
			return oxidizerEfficiencies;
		}
		public static void RegisterCustomOxidizers()
		{
			foreach (var efficiency in GetOxidizerEfficiencies())
			{
				if (!Clustercraft.dlc1OxidizerEfficiencies.ContainsKey(efficiency.Key))
				{
					SgtLogger.l($"Added {efficiency.Key} to oxidizer efficiencies with a value of {efficiency.Value}");
					Clustercraft.dlc1OxidizerEfficiencies.Add(efficiency.Key, efficiency.Value);
				}
			}
		}
	}
}

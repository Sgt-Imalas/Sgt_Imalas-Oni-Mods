using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	internal class CritterDietsInfo
	{
		/// <summary>
		/// ConsumedElement, ConversionRate, ProducedElement, CritterId
		/// </summary>
		public static Dictionary<SourceModInfo, HashSet<Tuple<Tag,float,Tag,Tag>>> CritterDietList = [];

		public static Diet.Info AddToList(string[] critterIds, SourceModInfo mod, Diet.Info diet)
		{
			//SgtLogger.l($"Adding critter diet {diet} (inputs: {string.Join(", ", diet.consumedTags.Select(t => t.ToString()))}, outputs: {string.Join(", ", diet.producedElement)}) for mod {mod} with critters: {string.Join(", ", critterIds)}");

			if (!CritterDietList.TryGetValue(mod, out var hashset))
			{
				hashset = [];
				CritterDietList.Add(mod, hashset);
			}
			foreach (var critterId in critterIds)
			{
				var data = new Tuple<Tag, float, Tag, Tag>( diet.consumedTags.FirstOrDefault(),diet.producedConversionRate, diet.producedElement, critterId);				
				hashset.Add(data);
			}
			return diet;
		}
		public static List<Tuple<Tag, float, Tag, Tag>> GetCritterInfo(SourceModInfo mod)
		{
			if (!CritterDietList.TryGetValue(mod, out var dic))
				return [];
			return dic.ToList();
		}
	}
}

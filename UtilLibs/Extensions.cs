using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.DUPLICANTS.STATUSITEMS;

namespace UtilLibs
{
	public static class Extensions
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var known = new HashSet<TKey>();
			return source.Where(element => known.Add(keySelector(element)));
		}
	}
}

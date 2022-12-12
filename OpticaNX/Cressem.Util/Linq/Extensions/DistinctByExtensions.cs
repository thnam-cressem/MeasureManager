using System;
using System.Collections.Generic;
using System.Linq;

namespace Cressem.Util.Linq.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	/// <example>
	/// <![CDATA[
	/// var list1 = people.DistinctBy(x => x.Gender);
	/// var list2 = people.DistinctBy(x => new { x.Gender, x.Age });
	/// 
	/// // DistinctBy() is equvalant to
	/// var list3 = people.GroupBy(x => x.Gender)
	/// 						 .Select(x => x.First())
	/// 						 .ToList();
	/// var list4 = people.GroupBy(x => new { x.Gender, x.Age })
	///						 .Select(x => x.First())
	///						 .ToList();
	/// ]]>
	/// </example>
	public static class DistinctByExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="items"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
		{
			return items.GroupBy(property).Select(x => x.First());
		}
	}
}

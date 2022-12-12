using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cressem.Util.Linq.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// http://stackoverflow.com/questions/5489987/linq-full-outer-join/13503860#13503860
	/// </remarks>
	public static class JoinExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TA"></typeparam>
		/// <typeparam name="TB"></typeparam>
		/// <typeparam name="TK"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="selectKeyA"></param>
		/// <param name="selectKeyB"></param>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static IList<TR> FullOuterGroupJoin<TA, TB, TK, TR>(
			 this IEnumerable<TA> a,
			 IEnumerable<TB> b,
			 Func<TA, TK> selectKeyA, Func<TB, TK> selectKeyB,
			 Func<IEnumerable<TA>, IEnumerable<TB>, TK, TR> projection)
		{
			var alookup = a.ToLookup(selectKeyA);
			var blookup = b.ToLookup(selectKeyB);

			var keys = new HashSet<TK>(alookup.Select(p => p.Key));
			keys.UnionWith(blookup.Select(p => p.Key));

			var join = from key in keys
						  let xa = alookup[key]
						  let xb = blookup[key]
						  select projection(xa, xb, key);

			return join.ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TA"></typeparam>
		/// <typeparam name="TB"></typeparam>
		/// <typeparam name="TK"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="selectKeyA"></param>
		/// <param name="selectKeyB"></param>
		/// <param name="projection"></param>
		/// <param name="defaultA"></param>
		/// <param name="defaultB"></param>
		/// <returns></returns>
		public static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
			 this IEnumerable<TA> a,
			 IEnumerable<TB> b,
			 Func<TA, TK> selectKeyA, Func<TB, TK> selectKeyB,
			 Func<TA, TB, TK, TR> projection,
			 TA defaultA = default(TA), TB defaultB = default(TB))
		{
			var alookup = a.ToLookup(selectKeyA);
			var blookup = b.ToLookup(selectKeyB);

			var keys = new HashSet<TK>(alookup.Select(p => p.Key));
			keys.UnionWith(blookup.Select(p => p.Key));

			var join = from key in keys
						  from xa in alookup[key].DefaultIfEmpty(defaultA)
						  from xb in blookup[key].DefaultIfEmpty(defaultB)
						  select projection(xa, xb, key);

			return join.ToList();
		}
	}
}

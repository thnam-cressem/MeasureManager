using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cressem.Util.Linq.Extensions
{
	public static class EnumerableExtension
	{
		public static T FirstOr<T>(this IEnumerable<T> source, T alternate)
		{
			return source.DefaultIfEmpty(alternate)
							 .First();
		}

		public static T FirstOr<T>(this IEnumerable<T> source, Func<T, bool> predicate, T alternate)
		{
			return source.Where(predicate)
							 .FirstOr(alternate);
		}

		public static int IndexOf<T>(this IEnumerable<T> list, T item)
		{
			return list.Select((x, index) => EqualityComparer<T>.Default.Equals(item, x)
														? index
														: -1)
						  .FirstOr(x => x != -1, -1);
		}
	}
}

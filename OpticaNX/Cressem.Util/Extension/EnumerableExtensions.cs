﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cressem.Util.Extension
{
	public static class EnumerableExtensions
	{
		public static int IndexOf<T>(this IEnumerable<T> obj, T value)
		{
			return obj.IndexOf(value, null);
		}

		public static int IndexOf<T>(this IEnumerable<T> obj, T value, IEqualityComparer<T> comparer)
		{
			comparer = comparer ?? EqualityComparer<T>.Default;
			var found = obj
				 .Select((a, i) => new { a, i })
				 .FirstOrDefault(x => comparer.Equals(x.a, value));
			return found == null ? -1 : found.i;
		}
	}

}

using Cressem.Util.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cressem.Util.Helpers
{
	public static class MathHelper
	{
		public static T? GetNullableMin<T>(T? lhs, T? rhs) where T : struct, IComparable<T>
		{
			// 둘다 null이면 null을 반환
			if (lhs == null && rhs == null)
				return (T?)null;

			// 둘다 null이 아니면 비교하여 작은 값을 반환 null을 반환
			if (lhs != null && rhs != null)
				return Nullable.Compare(lhs, rhs) < 0 ? lhs : rhs; ;

			return lhs == null ? rhs : lhs;
		}

		public static T? GetNullableMax<T>(T? lhs, T? rhs) where T : struct, IComparable<T>
		{
			// 둘다 null이면 null을 반환
			if (lhs == null && rhs == null)
				return (T?)null;

			// 둘다 null이 아니면 비교하여 큰 값을 반환 null을 반환
			if (lhs != null && rhs != null)
				return Nullable.Compare(lhs, rhs) > 0 ? lhs : rhs;

			return lhs == null ? rhs : lhs;
		}

		public static T? GetNullableMed<T>(T? lhs, T? rhs, T? devide) where T : struct, IComparable<T>
		{
			// 둘다 null이면 null을 반환
			if (lhs == null && rhs == null)
				return (T?)null;

			// 둘 중 하나가 null이면 null이아닌 값을 반환
			if (lhs == null)
				return rhs;
			if (rhs == null)
				return lhs;

			// 둘 다 있을경우, 둘중의 중간 값을 반환
			T sum = Calculator<T>.Add(lhs.Value, rhs.Value);
		
			T result = Calculator<T>.Divide(sum, devide.Value);

			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cressem.Util.Generics
{
	public struct Number<T>
	where T : IComparable<T>, IEquatable<T>
	{
		private readonly T _Value;

		public Number(T value)
		{
			_Value = value;
		}

		#region Comparison

		public bool Equals(Number<T> other)
		{
			return _Value.Equals(other._Value);
		}

		public bool Equals(T other)
		{
			return _Value.Equals(other);
		}

		public int CompareTo(Number<T> other)
		{
			return _Value.CompareTo(other._Value);
		}

		public int CompareTo(T other)
		{
			return _Value.CompareTo(other);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is T)
				return _Value.Equals((T)obj);
			if (obj is Number<T>)
				return _Value.Equals(((Number<T>)obj)._Value);
			return false;
		}

		public override int GetHashCode()
		{
			return (_Value == null) ? 0 : _Value.GetHashCode();
		}

		static public bool operator ==(Number<T> a, Number<T> b)
		{
			return a._Value.Equals(b._Value);
		}

		static public bool operator !=(Number<T> a, Number<T> b)
		{
			return !a._Value.Equals(b._Value);
		}

		static public bool operator <(Number<T> a, Number<T> b)
		{
			return a._Value.CompareTo(b._Value) < 0;
		}

		static public bool operator <=(Number<T> a, Number<T> b)
		{
			return a._Value.CompareTo(b._Value) <= 0;
		}

		static public bool operator >(Number<T> a, Number<T> b)
		{
			return a._Value.CompareTo(b._Value) > 0;
		}

		static public bool operator >=(Number<T> a, Number<T> b)
		{
			return a._Value.CompareTo(b._Value) >= 0;
		}

		static public Number<T> operator !(Number<T> a)
		{
			return new Number<T>(Calculator<T>.Negate(a._Value));
		}

		#endregion
		#region Arithmatic operations

		static public Number<T> operator +(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.Add(a._Value, b._Value));
		}

		static public Number<T> operator -(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.Subtract(a._Value, b._Value));
		}

		static public Number<T> operator *(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.Multiply(a._Value, b._Value));
		}

		static public Number<T> operator /(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.Divide(a._Value, b._Value));
		}

		static public Number<T> operator %(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.Modulo(a._Value, b._Value));
		}

		static public Number<T> operator -(Number<T> a)
		{
			return new Number<T>(Calculator<T>.Negate(a._Value));
		}

		static public Number<T> operator +(Number<T> a)
		{
			return new Number<T>(Calculator<T>.Plus(a._Value));
		}

		static public Number<T> operator ++(Number<T> a)
		{
			return new Number<T>(Calculator<T>.Increment(a._Value));
		}

		static public Number<T> operator --(Number<T> a)
		{
			return new Number<T>(Calculator<T>.Decrement(a._Value));
		}

		#endregion
		#region Bitwise operations

		static public Number<T> operator <<(Number<T> a, int b)
		{
			return new Number<T>(Calculator<T>.LeftShift(a._Value, b));
		}

		static public Number<T> operator >>(Number<T> a, int b)
		{
			return new Number<T>(Calculator<T>.RightShift(a._Value, b));
		}

		static public Number<T> operator &(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.And(a._Value, b._Value));
		}

		static public Number<T> operator |(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.Or(a._Value, b._Value));
		}

		static public Number<T> operator ^(Number<T> a, Number<T> b)
		{
			return new Number<T>(Calculator<T>.Xor(a._Value, b._Value));
		}

		static public Number<T> operator ~(Number<T> a)
		{
			return new Number<T>(Calculator<T>.OnesComplement(a._Value));
		}

		#endregion
		#region Casts

		static public implicit operator Number<T>(T value)
		{
			return new Number<T>(value);
		}

		static public explicit operator T(Number<T> value)
		{
			return value._Value;
		}

		#endregion
		#region Other members

		public override string ToString()
		{
			return (_Value == null) ? string.Empty : _Value.ToString();
		}

		#endregion
	}

}

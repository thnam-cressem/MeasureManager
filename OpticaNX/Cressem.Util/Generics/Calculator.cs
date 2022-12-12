using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cressem.Util.Generics
{
	//https://codereview.stackexchange.com/questions/26022/generic-calculator-and-generic-number
	/// <summary>
	/// Class to allow operations (like Add, Multiply, etc.) for generic types. This type should allow these operations themselves.
	/// If a type does not support an operation, an exception is throw when using this operation, not during construction of this class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class Calculator<T>
	{
		static Calculator()
		{
			Add = CreateDelegate<T>(Expression.AddChecked, "Addition", true);
			Subtract = CreateDelegate<T>(Expression.SubtractChecked, "Substraction", true);
			Multiply = CreateDelegate<T>(Expression.MultiplyChecked, "Multiply", true);
			Divide = CreateDelegate<T>(Expression.Divide, "Divide", true);
			Modulo = CreateDelegate<T>(Expression.Modulo, "Modulus", true);
			Negate = CreateDelegate(Expression.NegateChecked, "Negate", true);
			Plus = CreateDelegate(Expression.UnaryPlus, "Plus", true);
			Increment = CreateDelegate(Expression.Increment, "Increment", true);
			Decrement = CreateDelegate(Expression.Decrement, "Decrement", true);
			LeftShift = CreateDelegate<int>(Expression.LeftShift, "LeftShift", false);
			RightShift = CreateDelegate<int>(Expression.RightShift, "RightShift", false);
			OnesComplement = CreateDelegate(Expression.OnesComplement, "OnesComplement", false);
			And = CreateDelegate<T>(Expression.And, "BitwiseAnd", false);
			Or = CreateDelegate<T>(Expression.Or, "BitwiseOr", false);
			Xor = CreateDelegate<T>(Expression.ExclusiveOr, "ExclusiveOr", false);
		}

		static private Func<T, T2, T> CreateDelegate<T2>(Func<Expression, Expression, Expression> @operator, string operatorName, bool isChecked)
		{
			try
			{
				Type convertToTypeA = ConvertTo(typeof(T));
				Type convertToTypeB = ConvertTo(typeof(T2));
				ParameterExpression parameterA = Expression.Parameter(typeof(T), "a");
				ParameterExpression parameterB = Expression.Parameter(typeof(T2), "b");
				Expression valueA = (convertToTypeA != null) ? Expression.Convert(parameterA, convertToTypeA) : (Expression)parameterA;
				Expression valueB = (convertToTypeB != null) ? Expression.Convert(parameterB, convertToTypeB) : (Expression)parameterB;
				Expression body = @operator(valueA, valueB);
				if (convertToTypeA != null)
				{
					if (isChecked)
						body = Expression.ConvertChecked(body, typeof(T));
					else
						body = Expression.Convert(body, typeof(T));
				}
				return Expression.Lambda<Func<T, T2, T>>(body, parameterA, parameterB).Compile();
			}
			catch
			{
				return (a, b) =>
				{
					throw new InvalidOperationException("Operator " + operatorName + " is not supported by type " + typeof(T).FullName + ".");
				};
			}
		}

		static private Func<T, T> CreateDelegate(Func<Expression, Expression> @operator, string operatorName, bool isChecked)
		{
			try
			{
				Type convertToType = ConvertTo(typeof(T));
				ParameterExpression parameter = Expression.Parameter(typeof(T), "a");
				Expression value = (convertToType != null) ? Expression.Convert(parameter, convertToType) : (Expression)parameter;
				Expression body = @operator(value);
				if (convertToType != null)
				{
					if (isChecked)
						body = Expression.ConvertChecked(body, typeof(T));
					else
						body = Expression.Convert(body, typeof(T));
				}
				return Expression.Lambda<Func<T, T>>(body, parameter).Compile();
			}
			catch
			{
				return (a) =>
				{
					throw new InvalidOperationException("Operator " + operatorName + " is not supported by type " + typeof(T).FullName + ".");
				};
			}
		}

		static private Type ConvertTo(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Char:
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
					return typeof(int);
			}
			return null;
		}

		/// <summary>
		/// Adds two values of the same type.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> Add;

		/// <summary>
		/// Subtracts two values of the same type.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> Subtract;

		/// <summary>
		/// Multiplies two values of the same type.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> Multiply;

		/// <summary>
		/// Divides two values of the same type.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> Divide;

		/// <summary>
		/// Divides two values of the same type and returns the remainder.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> Modulo;

		/// <summary>
		/// Gets the negative value of T.
		/// Supported by: All numeric values, but will throw an OverflowException on unsigned values which are not 0.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T> Negate;

		/// <summary>
		/// Gets the negative value of T.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T> Plus;

		/// <summary>
		/// Gets the negative value of T.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T> Increment;

		/// <summary>
		/// Gets the negative value of T.
		/// Supported by: All numeric values.
		/// </summary>
		/// <exception cref="OverflowException"/>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T> Decrement;

		/// <summary>
		/// Shifts the number to the left.
		/// Supported by: All integral types.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, int, T> LeftShift;

		/// <summary>
		/// Shifts the number to the right.
		/// Supported by: All integral types.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, int, T> RightShift;

		/// <summary>
		/// Inverts all bits inside the value.
		/// Supported by: All integral types.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T> OnesComplement;

		/// <summary>
		/// Performs a bitwise OR.
		/// Supported by: All integral types.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> Or;

		/// <summary>
		/// Performs a bitwise AND
		/// Supported by: All integral types.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> And;

		/// <summary>
		/// Performs a bitwise Exclusive OR.
		/// Supported by: All integral types.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static readonly Func<T, T, T> Xor;
	}
}

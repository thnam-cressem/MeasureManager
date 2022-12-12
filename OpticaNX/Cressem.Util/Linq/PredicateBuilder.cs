using System;
using System.Linq;
using System.Linq.Expressions;

namespace Cressem.Util.Linq
{
	/// <summary>
	/// Dynamically Composing Expression Predicates
	/// </summary>
	/// <example>
	/// <code>
	/// <![CDATA[
	/// IList<ProductInfo> SearchProducts (params string[] keywords)
	/// {
	///    IEnumerable<ProductInfo> list = DAL.GetProducts();
	///    var predicate = PredicateBuilder.False<ProductInfo>();
	///    // if you use predicate.And, then use PredicateBuilder.True
	///    foreach (string keyword in keywords)
	///    {
	///	    string temp = keyword;
	///	    predicate = predicate.Or(p => p.Description.Contains(temp));
	///    }
	///    list = list.AsQueryable().Where(predicate);
	///    
	///    return list.ToList();
	/// }
	///  ]]>
	/// </code>
	/// </example>
	/// <remarks>
	/// http://www.albahari.com/nutshell/predicatebuilder.aspx
	/// </remarks>
	public static class PredicateBuilder
	{
		public static Expression<Func<T, bool>> True<T>()
		{ 
			return f => true; 
		}

		public static Expression<Func<T, bool>> False<T>()
		{ 
			return f => false;
		}

		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());

			return Expression.Lambda<Func<T, bool>>
					(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
		}

		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
																			  Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T, bool>>
					(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
		}
	}
}

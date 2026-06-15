using System.Linq.Expressions;

namespace market_mvc.Infrastructure.Common
{
    /// <summary>
    /// Extension methods for combining LINQ expressions
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines two expressions with AND logic
        /// </summary>
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            if (left == null)
                return right;

            if (right == null)
                return left;

            var parameter = Expression.Parameter(typeof(T), "x");
            var leftInvoked = Expression.Invoke(left, parameter);
            var rightInvoked = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftInvoked, rightInvoked);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /// <summary>
        /// Combines two expressions with OR logic
        /// </summary>
        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            if (left == null)
                return right;

            if (right == null)
                return left;

            var parameter = Expression.Parameter(typeof(T), "x");
            var leftInvoked = Expression.Invoke(left, parameter);
            var rightInvoked = Expression.Invoke(right, parameter);
            var combined = Expression.OrElse(leftInvoked, rightInvoked);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /// <summary>
        /// Negates an expression
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(
            this Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                return null;

            var parameter = expression.Parameters[0];
            var body = Expression.Not(expression.Body);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}

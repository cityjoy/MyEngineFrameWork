using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Engine.Infrastructure.Utils 
{
    public static class IQueryableExtention
    {
        /// <summary>
        /// 扩展IQueryable IN 方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="valueSelector"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> In<TEntity, TValue>(this IQueryable<TEntity> source, Expression<Func<TEntity, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }
            ParameterExpression p = valueSelector.Parameters.Single();
            // p => valueSelector(p) == values[0] || valueSelector(p) == ...
            if (!values.Any())
            {
                return source;
            }
            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return source.Where(Expression.Lambda<Func<TEntity, bool>>(body, p));
        }
    }
}
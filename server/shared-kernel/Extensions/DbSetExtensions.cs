using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoE.Ideas.Shared.Extensions
{
    // this class is from stackover, from my own answer (Dan Chenier)
    // (haha...I've always wanted to do that!)
    public static class DbSetExtensions
    {

        public static void AddOrUpdate<T>(this DbSet<T> dbSet, Expression<Func<T, object>> identifierExpression, params T[] entities) where T : class
        {
            foreach (var entity in entities)
                AddOrUpdate(dbSet, identifierExpression, entity);
        }


        public static void AddOrUpdate<T>(this DbSet<T> dbSet, Expression<Func<T, object>> identifierExpression, T entity) where T : class
        {
            if (identifierExpression == null)
                throw new ArgumentNullException(nameof(identifierExpression));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyObject = identifierExpression.Compile()(entity);
            var parameter = Expression.Parameter(typeof(T), "p");

            var lambda = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    ReplaceParameter(identifierExpression.Body, parameter),
                    Expression.Constant(keyObject)),
                parameter);

            var item = dbSet.FirstOrDefault(lambda.Compile());
            if (item == null)
            {
                // easy case
                dbSet.Add(entity);
            }
            else
            {
                // get Key fields, using KeyAttribute if possible otherwise convention
                var dataType = typeof(T);
                var keyFields = dataType.GetProperties().Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToList();
                if (!keyFields.Any())
                {
                    string idName = dataType.Name + "Id";
                    keyFields = dataType.GetProperties().Where(p =>
                        string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(p.Name, idName, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // update all non key and non collection properties
                foreach (var p in typeof(T).GetProperties().Where(p => p.GetSetMethod() != null && p.GetGetMethod() != null))
                {
                    // ignore collections
                    if (p.PropertyType != typeof(string) && p.PropertyType.GetInterface(nameof(System.Collections.IEnumerable)) != null)
                        continue;

                    // ignore ID fields
                    if (keyFields.Any(x => x.Name == p.Name))
                        continue;

                    var existingValue = p.GetValue(entity);
                    if (!Equals(p.GetValue(item), existingValue))
                    {
                        p.SetValue(item, existingValue);
                    }
                }

                // also update key values on incoming data item where appropriate
                foreach (var idField in keyFields.Where(p => p.GetSetMethod() != null && p.GetGetMethod() != null))
                {
                    var existingValue = idField.GetValue(item);
                    if (!Equals(idField.GetValue(entity), existingValue))
                    {
                        idField.SetValue(entity, existingValue);
                    }
                }
            }
        }


        private static Expression ReplaceParameter(Expression oldExpression, ParameterExpression newParameter)
        {
            switch (oldExpression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var m = (MemberExpression)oldExpression;
                    return Expression.MakeMemberAccess(newParameter, m.Member);
                case ExpressionType.New:
                    var newExpression = (NewExpression)oldExpression;
                    var arguments = new List<Expression>();
                    foreach (var a in newExpression.Arguments)
                        arguments.Add(ReplaceParameter(a, newParameter));
                    var returnValue = Expression.New(newExpression.Constructor, arguments.ToArray());
                    return returnValue;
                default:
                    throw new NotSupportedException("Unknown expression type for AddOrUpdate: " + oldExpression.NodeType);
            }
        }
    }
}

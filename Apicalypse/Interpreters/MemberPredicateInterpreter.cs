using Apicalypse.Configuration;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Apicalypse.Interpreters
{
    public class MemberPredicateInterpreter
    {
        /// <summary>
        /// Returns a member or list of members as a string from a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string Run(Expression predicate, QueryBuilderOptions options)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            switch (predicate.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ComputeMemberAccess(predicate, options);
                case ExpressionType.New:
                    return ComputeNewObject(predicate, options);
                default:
                    throw new Exception("Invalid predicate provided");
            }
        }

        private static string ComputeNewObject(Expression predicate,  QueryBuilderOptions options)
        {
            var newPredicate = (predicate as NewExpression);

            var properties = newPredicate.Arguments.Select(a => ComputeMemberAccess(a, options));

            return string.Join(",", properties);
        }

        private static string ComputeMemberAccess(Expression predicate,  QueryBuilderOptions options)
        {
            var memberExpression = (predicate as MemberExpression);
            switch (memberExpression.Member.MemberType)
            {
                case MemberTypes.Property:
                    var path = UnrollMemberPath(memberExpression, options);
                    return path;
                default:
                    throw new NotImplementedException($"Works only with properties of the Generic object");
            }
        }

        private static string UnrollMemberPath(MemberExpression predicate,  QueryBuilderOptions options)
        {
            var path = "";
            if (predicate.Expression != null && predicate.Expression is MemberExpression)
                path = UnrollMemberPath(predicate.Expression as MemberExpression, options) + ".";

            return path + FieldInterpreter.Run(predicate.Member.Name, options);
        }
    }
}

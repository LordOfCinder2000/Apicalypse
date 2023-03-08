using Apicalypse.Configuration;
using Apicalypse.Exceptions;
using Apicalypse.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Apicalypse.Interpreters
{
    /// <summary>
    /// Interpreter for predicates in Where method of the query builder
    /// </summary>
    public class WherePredicateInterpreter
    {
        public enum ArrayPostfixMode { ContainsAny, ExactMatch, ContainsAll };
        /// <summary>
        /// Interpretes a predicate to convert it to a string usable by IGDB API
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static string Run(Expression predicate,  QueryBuilderOptions options, bool invert = false, ArrayPostfixMode arrayPostfixMode = ArrayPostfixMode.ContainsAny)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var binaryPredicate = predicate as BinaryExpression;
            switch (predicate.NodeType)
            {
                /*
                 * First part of switch : binary operators
                 * -------
                 * if Node type referes to a binary operators we set the operator
                 * and let the method continue after the switch to create the
                 * expression.
                 * binary operators are either logical operators (&& or ||),
                 * relational operators (>, >=, <, <=) or equality operators (==, !=)
                 */
                case ExpressionType.AndAlso:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, "&", options);
                case ExpressionType.OrElse:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, "|", options);
                case ExpressionType.GreaterThan:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, ">", options);
                case ExpressionType.GreaterThanOrEqual:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, ">=", options);
                case ExpressionType.LessThan:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, "<", options);
                case ExpressionType.LessThanOrEqual:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, "<=", options);
                case ExpressionType.NotEqual:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, "!=", options);
                case ExpressionType.Equal:
                    return ComputeBinaryOperator(binaryPredicate.Left, binaryPredicate.Right, "=", options);
                /*
                 * Second part of switch : members
                 * -------
                 * If node type refers to a member, a constant or an array then
                 * then return an interpretation.
                 */
                case ExpressionType.MemberAccess:
                    return ComputeMemberAccess(predicate, options, arrayPostfixMode);
                case ExpressionType.Constant:
                    return ComputeConstant(predicate, options);
                case ExpressionType.NewArrayInit:
                    return ComputeArray(predicate, arrayPostfixMode, options);
                /*
                 * Third part of switch : methods
                 * -------
                 * if node type referes to a method corresponding to 
                 */
                case ExpressionType.Not:
                    return ComputeNotCall(predicate, options);
                case ExpressionType.Call:
                    return ComputeMethodCall(predicate, options, invert);
                /*
                 * Fourth part of switch : convert
                 * -------
                 * if node type corresponding to 
                 */
                case ExpressionType.Convert:
                    return ComputeConvert(predicate, options);
                default:
                    throw new InvalidPredicateException(predicate);
            }
        }

        private static string ComputeConvert(Expression predicate, QueryBuilderOptions options)
        {
            return Run((predicate as UnaryExpression).Operand, options);
        }

        private static string ComputeMemberAccess(Expression predicate,  QueryBuilderOptions options, ArrayPostfixMode arrayPostfixMode)
        {
            var memberExpression = (MemberExpression)predicate;
            if (memberExpression.Member.MemberType == MemberTypes.Field)
            {
                var container = ((memberExpression.Expression as ConstantExpression));
                if(container == null)
                {
                    return FieldInterpreter.Run(memberExpression.Member.Name, options);
                }

                var member = memberExpression.Member;

                var value = ((FieldInfo)member).GetValue(container.Value);
                var valueType = value.GetType();

                if (valueType.IsArray)
                {
                    var arrayItemType = valueType.GetElementType();
                    var array = value as Array;

                    var expressionArray = new List<Expression>();
                    foreach (var item in array)
                    {
                        expressionArray.Add(Expression.Constant(item, arrayItemType));
                    }

                    return Run(Expression.NewArrayInit(arrayItemType, expressionArray), options, arrayPostfixMode: arrayPostfixMode);
                }
                
                return Run(Expression.Constant(value), options);
            }

            return MemberPredicateInterpreter.Run(predicate, options);
        }

        private static string ComputeBinaryOperator(Expression left, Expression right, string binaryOperator,  QueryBuilderOptions options)
        {
            var leftMember = Run(left, options);
            var rightMember = Run(right, options);

            return $"{leftMember} {binaryOperator} {rightMember}";
        }

        private static string ComputeConstant(Expression constant,  QueryBuilderOptions options)
        {
            var value = (constant as ConstantExpression).Value;
            if (value is string)
                return $"\"{(value as string).Replace("\"", "\\\"")}\"";
            if (value is bool boolean)
                return boolean ? "true" : "false";
            if (value is null)
                return "null";
            if (value is IConvertible)
                return (value as IConvertible).ToString(CultureInfo.InvariantCulture);

            return value.ToString();
        }
       
        private static string ComputeArray(Expression array, ArrayPostfixMode arrayPostfixMode,  QueryBuilderOptions options)
        {
            var list = string.Join(
                ",",
                (array as NewArrayExpression).Expressions.Select(e => Run(e as ConstantExpression, options))
            );

            switch (arrayPostfixMode)
            {
                case ArrayPostfixMode.ContainsAny:
                    return $"({list})";

                case ArrayPostfixMode.ExactMatch:
                    return $"[{list}]";

                case ArrayPostfixMode.ContainsAll:
                    return $"{{{list}}}";

                default:
                    throw new Exception("Unknown array postfix mode");
            }
        }

        private static string ComputeNotCall(Expression notCall,  QueryBuilderOptions options)
        {
            return Run((notCall as UnaryExpression).Operand, options, true);
        }

        private static string ComputeMethodCall(Expression methodCall,  QueryBuilderOptions options, bool invert = false)
        {
            var method = (methodCall as MethodCallExpression);

            if (method.Object != null && method.Object.NodeType == ExpressionType.MemberAccess)
                return ComputeStringComparison(method, options);
            if (
                method.Object is null
                && method.Arguments.Count() >= 1
                && (method.Arguments[0].NodeType == ExpressionType.NewArrayInit || method.Arguments[0].Type.IsArray)
                ||
                method.Object != null
                && method.Object.NodeType == ExpressionType.NewArrayInit
            )
                return ComputeArrayComparison(method, options, invert);

            throw new InvalidPredicateException(methodCall);
        }

        private static string ComputeArrayComparison(MethodCallExpression method,  QueryBuilderOptions options, bool inverted)
        {
            var invert = inverted ? "!" : "";
            string left;
            string right;
            switch (method.Method.Name)
            {
                case nameof(Enumerable.Contains):
                    left = Run(method.Arguments[1], options);
                    right = Run(method.Arguments[0], options, arrayPostfixMode: ArrayPostfixMode.ContainsAny);
                    break;
                case nameof(IEnumerableExtensions.IsContainedIn):
                    left = Run(method.Arguments[1], options);
                    right = Run(method.Arguments[0], options, arrayPostfixMode: ArrayPostfixMode.ContainsAll);
                    break;
                case nameof(Enumerable.Equals):
                    left = Run(method.Arguments[0], options);
                    right = Run(method.Object, options, arrayPostfixMode: ArrayPostfixMode.ExactMatch);
                    break;
                case nameof(IEnumerableExtensions.IsAnyIn):
                    left = Run(method.Arguments[1], options);
                    right = Run(method.Arguments[0], options, arrayPostfixMode: ArrayPostfixMode.ContainsAny);
                    break;
                default:
                    throw new NotImplementedException($"Array method {method.Method.Name} is not implemented");
            }

            return $"{left} = {invert}{right}";
        }

        private static string ComputeStringComparison(MethodCallExpression method,  QueryBuilderOptions options)
        {

            switch (method.Method.Name)
            {
                case nameof(string.Contains):
                    return MakeStringComparisonString(method, options, true, true, DoesMethodIgnoreCase(method));
                case nameof(string.StartsWith):
                    return MakeStringComparisonString(method, options, false, true, DoesMethodIgnoreCase(method));
                case nameof(string.EndsWith):
                    return MakeStringComparisonString(method, options, true, false, DoesMethodIgnoreCase(method));
                default:
                    throw new NotImplementedException($"The string comparison method {method.Method.Name} is not implemented");
            }
        }

        private static bool DoesMethodIgnoreCase(MethodCallExpression method)
        {
            if (method.Arguments.Count > 1
                && (method.Arguments[1] as ConstantExpression).Value is StringComparison)
                return StringComparisonArgIgnoreCase(method);
            if (method.Arguments.Count > 2
                && (method.Arguments[1] as ConstantExpression).Value is bool)
                return BoolAndCultureArgsIgnoreCase(method);

            return false;
        }

        private static bool StringComparisonArgIgnoreCase(MethodCallExpression method)
        {
            StringComparison[] flags = new StringComparison[]
            {
                StringComparison.OrdinalIgnoreCase, StringComparison.CurrentCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase
            };
            return (
                method.Arguments.Count() > 1
                && flags.Contains((StringComparison)(method.Arguments[1] as ConstantExpression).Value));

        }

        private static bool BoolAndCultureArgsIgnoreCase(MethodCallExpression method)
        {
            return (
                method.Arguments.Count() >= 2
                && ((bool)(method.Arguments[1] as ConstantExpression).Value) == true
            );
        }

        private static string MakeStringComparisonString(MethodCallExpression method,  QueryBuilderOptions options, bool startsAny, bool endsAny, bool ignoreCase)
        {
            var comparison = ignoreCase ? "~" : "=";
            var startingStar = startsAny ? "*" : "";
            var endingStar = endsAny ? "*" : "";
            return $"{Run(method.Object, options)} {comparison} {startingStar}{Run(method.Arguments.First(), options)}{endingStar}";
        }
    }
}

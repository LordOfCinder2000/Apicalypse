using Apicalypse;
using Apicalypse.Configuration;
using Apicalypse.Extensions;
using Apicalypse.NamingPolicies;
using Apicalypse.Test.Builders;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

var builder = new QueryBuilder<Game>(new QueryBuilderOptions { NamingPolicy = NamingPolicy.SnakeCase });

var number = new int[] { 99};
var temp = 1;
number.Select(x => x == temp);
Expression<Func<Game, bool>> expression = global => global.Follows == 1;
Expression<Func<Game, bool>> expression2 = global => global.Follows == temp;

var left = ((BinaryExpression)expression2.Body).Left;
var right = ((BinaryExpression)expression2.Body).Right;

var member = ((MemberExpression)(((UnaryExpression)right).Operand));

var rs = new Visitor().Visit(member);

var str = builder.Select<Game>().Where(g => number.IsAnyIn(g.Tags)).Build();
//var str = builder.Select<Game>().Where(g => g.Follows == temp).Build();

Console.WriteLine();
class Visitor : ExpressionVisitor
{
    protected override Expression VisitMember
        (MemberExpression memberExpression)
    {
        // Recurse down to see if we can simplify...
        var expression = Visit(memberExpression.Expression);

        // If we've ended up with a constant, and it's a property or a field,
        // we can simplify ourselves to a constant
        if (expression is ConstantExpression)
        {
            object container = ((ConstantExpression)expression).Value;
            var member = memberExpression.Member;
            if (member is FieldInfo)
            {
                object value = ((FieldInfo)member).GetValue(container);
                return Expression.Constant(value);
            }
            if (member is PropertyInfo)
            {
                object value = ((PropertyInfo)member).GetValue(container, null);
                return Expression.Constant(value);
            }
        }
        return base.VisitMember(memberExpression);
    }
}
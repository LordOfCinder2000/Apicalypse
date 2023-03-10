using Apicalypse;
using Apicalypse.Configuration;
using Apicalypse.Extensions;
using Apicalypse.NamingPolicies;
using Apicalypse.Test.Builders;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

var builder = new QueryBuilder<Game>();
var str = builder.Select().Build();
//var number = new int[] { 99, 100};
//var temp = 1;
//number.Select(x => x == temp);
//Expression<Func<Game, bool>> expression = global => global.Follows == 1;
//Expression<Func<Game, bool>> expression2 = global => global.Follows == temp;

//var left = ((BinaryExpression)expression2.Body).Left;
//var right = ((BinaryExpression)expression2.Body).Right;

//var member = ((MemberExpression)(((UnaryExpression)right).Operand));


//var str = builder.Select<Game>().Where(g => g.Follows == temp).Build();

Console.WriteLine();
using Apicalypse.Configuration;
using Apicalypse.Interpreters;
using System;
using System.Linq.Expressions;

namespace Apicalypse
{
    /// <summary>
    /// Entry point for the lib.<br/>The query builder provides methods to prepare the
    /// query statements to send to the API.
    /// </summary>
    /// <typeparam name="TSource">The API model on witch the query is based</typeparam>
    public class QueryBuilder<TSource>
    {
        private readonly QueryBuilderOptions options;
        private string selects;
        private string filters;
        private string excludes;
        private string orders;
        private string search;
        private int take;
        private int skip;

        public QueryBuilder()
            : this(new QueryBuilderOptions { NamingPolicy = null })
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public QueryBuilder(QueryBuilderOptions options)
        {
            selects = "*";
            orders = "";
            this.options = options;
        }


        /// <summary>
        /// Sets the list of fields to gather from the API model with the public
        /// properties of the <em>TSelect</em> generic parameter.<br/>
        /// Each call replace the previous<br/>
        /// Prepares the <strong>fields</strong> statement of the Apicalypse query.
        /// </summary>
        /// <typeparam name="TSelect"></typeparam>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> Select<TSelect>()
        {
            if (!string.IsNullOrEmpty(excludes))
                throw new InvalidOperationException("Can't combine Exclude and Select methods.");

            selects = SelectTypeInterpreter.Run<TSelect>(options);

            return this;
        }

        /// <summary>
        /// Sets the list of fields to gather from the API model with predicate passed
        /// as parameter.<br/>
        /// Each call replace the previous<br/>
        /// Prepares the <strong>fields</strong> statement of the Apicalypse query.
        /// </summary>
        /// <param name="predicate">A predicate expression that provides a list of fields or a single fields</param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> Select<TKey>(Expression<Func<TSource, TKey>> predicate)
        {
            if (!string.IsNullOrEmpty(excludes))
                throw new InvalidOperationException("Can't combine Exclude and Select methods.");

            selects = MemberPredicateInterpreter.Run(predicate.Body, options);

            return this;
        }

        /// <summary>
        /// Sets the list of fields to exclude from the API model using the predicate used
        /// in parameters.<br/>
        /// Each call replace the previous.<br/>
        /// Prepares the <strong>excludes</strong> statement of the Apicalypse query.<br/>
        /// Can't be combine with the <em>Select(Expression&lt;Func&lt;T, object&gt;&gt;)</em> or the
        /// <em>Select&lt;TSelect&gt;()</em> methods.
        /// </summary>
        /// <param name="predicate">A predicate expression that provides a list of fields or a single field</param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> Exclude<TKey>(Expression<Func<TSource, TKey>> predicate)
        {
            if (selects != "*")
                throw new InvalidOperationException("Can't combine Exclude and Select methods.");

            excludes = MemberPredicateInterpreter.Run(predicate.Body, options);

            return this;
        }

        /// <summary>
        /// Sets the where clause to send to the API.<br/>
        /// Each call replace the previous<br/>
        /// Prepares the <strong>where</strong> statement of the Apicalypse query.
        /// </summary>
        /// <param name="predicate">A predicate expression that provides a conditional test</param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> Where(Expression<Func<TSource, bool>> predicate)
        {
            filters = WherePredicateInterpreter.Run(predicate.Body, options);

            return this;
        }

        /// <summary>
        /// Adds one or multiple <em>order by ascending</em> statements to the order by clause of
        /// the API.<br/>
        /// Each call adds a statement to the orders clause<br/>
        /// Prepares the <strong>orders</strong> statement of the Apicalypse query.
        /// </summary>
        /// <param name="predicate">A predicate expression that provides a list of fields or a single fields</param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> predicate)
        {
            if (!string.IsNullOrEmpty(orders))
                orders += ",";
            orders += MemberPredicateInterpreter.Run(predicate.Body, options);
            return this;
        }

        /// <summary>
        /// Adds one or multiple <em>order by descending</em> statements to the order by clause of
        /// the API.<br/>
        /// Each call adds a statement to the orders clause<br/>
        /// Prepares the <strong>orders</strong> statement of the Apicalypse query.
        /// </summary>
        /// <param name="predicate">A predicate expression that provides a list of fields or a single fields</param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> OrderByDescending<TKey>(Expression<Func<TSource, TKey>> predicate)
        {
            if (!string.IsNullOrEmpty(orders))
                orders += ",";
            orders += InvertedOrderByInterpreter.Run(predicate.Body, options);

            return this;
        }

        /// <summary>
        /// Sets a string to search in the API.<br/>
        /// Each call replace the previous<br/>
        /// Prepares the <strong>search</strong> statement of the Apicalypse query.
        /// </summary>
        /// <param name="search"></param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> Search(string search)
        {
            if (string.IsNullOrEmpty(search))
                this.search = "";
            else
                this.search = PrepareSearchString(search);

            return this;
        }

        public QueryBuilder<TSource> Search<TKey>(string search, Expression<Func<TSource, TKey>> field)
        {
            if (string.IsNullOrEmpty(search))
                this.search = "";
            else
                this.search = $"{MemberPredicateInterpreter.Run(field.Body, options)} {PrepareSearchString(search)}";

            return this;
        }

        /// <summary>
        /// Sets the number of items to gather from the API.<br/>
        /// Usefull to pagination.<br/>
        /// Each call replace the previous<br/>
        /// Prepares the <strong>limit</strong> statement of the Apicalypse query.
        /// </summary>
        /// <param name="count"></param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> Take(int count)
        {
            take = count;

            return this;
        }

        /// <summary>
        /// Sets the number of items to skip from the API. Usefull to pagination is combined to
        /// <em>Take(int)</em><br/>
        /// Each call replace the previous<br/>
        /// Prepares the <strong>offset</strong> statement of the Apicalypse query.
        /// </summary>
        /// <param name="count"></param>
        /// <returns>The query builder, to chain the statements</returns>
        public QueryBuilder<TSource> Skip(int count)
        {
            skip = count;

            return this;
        }

        /// <summary>
        /// Builds the query and provides an <em>Apicalypse</em> query.
        /// </summary>
        /// <returns>An Apicalypse query that can send the query to the API</returns>
        public string Build()
        {
            return QueryBuilderInterpreter.Run(selects, filters, excludes, orders, search, take, skip);
        }

        private string PrepareSearchString(string search)
        {
            var escaped = search.Replace("\"", "\\\"");

            return $"\"{escaped}\"";
        }
    }
}

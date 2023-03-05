using System;
using System.Linq.Expressions;

namespace Apicalypse.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid predicate is passed to the builder.
    /// </summary>
    public class InvalidPredicateException : Exception
    {
        private const string MESSAGE = "Could not parse predicate :";

        /// <summary>
        /// The string representation of the incriminated predicate.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Exception thrown when a predicate could not be parsed.<br/>
        /// Check the Body property to see what predicate is incriminated.
        /// </summary>
        /// <param name="predicate"></param>
        public InvalidPredicateException(Expression predicate)
            : base($"{MESSAGE} {predicate.NodeType}")
        {
            Body = predicate.ToString();
        }
    }
}

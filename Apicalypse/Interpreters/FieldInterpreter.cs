using Apicalypse.Configuration;
using Apicalypse.Extensions;
using Apicalypse.NamingPolicies;

namespace Apicalypse.Interpreters
{
    /// <summary>
    /// Static class holding the method to convert fields case depending on options provided
    /// </summary>
    public static class FieldInterpreter
    {
        /// <summary>
        /// Converts fields case depending on options provided
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string Run(string fieldName, QueryBuilderOptions options)
        {
            if (options.NamingPolicy is null)
            {
                return fieldName;
            }

            return options.NamingPolicy.ConvertName(fieldName);
        }
    }
}

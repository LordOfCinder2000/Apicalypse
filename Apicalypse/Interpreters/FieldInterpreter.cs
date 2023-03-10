using Apicalypse.Attributes;
using Apicalypse.Configuration;
using Apicalypse.Extensions;
using Apicalypse.NamingPolicies;
using System.ComponentModel;
using System.Reflection;

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
        public static string Run(MemberInfo memberInfo, QueryBuilderOptions options)
        {
            var fieldName = memberInfo.Name;
            var displayName = memberInfo.GetCustomAttribute<DisplayNameAttribute>();
            if (displayName != null)
            {
                fieldName = displayName.DisplayName;
            }

            if (options.NamingPolicy is null)
            {
                return fieldName;
            }

            return options.NamingPolicy.ConvertName(fieldName);
        }
    }
}

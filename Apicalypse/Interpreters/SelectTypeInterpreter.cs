using Apicalypse.Attributes;
using Apicalypse.Configuration;
using Apicalypse.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apicalypse.Interpreters
{
    /// <summary>
    /// Interpreter for generic types in Select method of the query builder
    /// </summary>
    public static class SelectTypeInterpreter
    {
        /// <summary>
        /// Interprets the public properties of the generic type to generate a list of selects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string Run<T>(QueryBuilderOptions options)
        {
            var properties = typeof(T)
                .GetProperties();

            var fields = new List<string>();

            foreach(var p in properties)
            {
                fields.Add(GetFields(p, options));
            }

            return string.Join(",",fields.Where(f => !string.IsNullOrEmpty(f)));
        }

        private static string GetFields(PropertyInfo property, QueryBuilderOptions options, string parentPath = "")
        {
            var fields = new List<string>();
            var path = parentPath + FieldInterpreter.Run(property.Name, options);
            if (property.GetCustomAttribute<IncludeAttribute>() != null)
            {
                foreach(var p in property.PropertyType.GetProperties())
                {
                    fields.Add(GetFields(p, options, path + "."));
                }
            }
            else if(property.GetCustomAttribute<ExcludeAttribute>() == null)
            {
                fields.Add(path);
            }

            return string.Join(",", fields);
        }
    }
}

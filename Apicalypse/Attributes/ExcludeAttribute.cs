using System;

namespace Apicalypse.Attributes
{
    /// <summary>
    /// Excludes the marked property from the select statement of the Apicalypse query
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeAttribute : Attribute
    {
        public ExcludeAttribute()
        {
        }
    }
}

using System;

namespace Apicalypse.Attributes
{
    /// <summary>
    /// If added to a linked entity, includes all its public properties in the select statement of the Apicalypse query
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IncludeAttribute : Attribute
    {
        public IncludeAttribute()
        {
        }
    }
}

using System;

namespace MvcUtilities.Binders
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true, Inherited=true)]
    public sealed class FieldNameAttribute : Attribute
    {
        public FieldNameAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName
        {
            get;
            private set;
        }
    }
}

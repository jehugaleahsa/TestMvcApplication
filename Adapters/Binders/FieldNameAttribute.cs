using System;

namespace Adapters.Binders
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true, Inherited=true)]
    public class FieldNameAttribute : Attribute
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

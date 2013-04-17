using System;

namespace Policies
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class InjectAttribute : Attribute
    {
    }
}

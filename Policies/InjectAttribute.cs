using System;

namespace Policies
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public sealed class InjectAttribute : Attribute
    {
    }
}

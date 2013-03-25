using System;

namespace Policies
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class SynchronizeAttribute : Attribute
    {
    }
}

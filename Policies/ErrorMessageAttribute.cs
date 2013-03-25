using System;

namespace Policies
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class ErrorMessageAttribute : Attribute
    {
        public ErrorMessageAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }
}

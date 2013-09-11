using System;

namespace Policies
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public class ErrorMessageAttribute : Attribute
    {
        public ErrorMessageAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public Type ExceptionType { get; set; }

        public string ErrorMessage { get; private set; }
    }
}

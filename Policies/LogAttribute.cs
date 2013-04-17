﻿using System;

namespace Policies
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class LogAttribute : Attribute
    {
        public string LogName { get; set; }
    }
}
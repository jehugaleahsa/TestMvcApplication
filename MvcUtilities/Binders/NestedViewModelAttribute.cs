﻿using System;

namespace MvcUtilities.Binders
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public sealed class NestedViewModelAttribute : Attribute
    {
        public NestedViewModelAttribute()
        {
        }
    }
}

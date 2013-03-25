using System;
using System.Web.Mvc;
using Adapters.Binders;
using Adapters.Models;

namespace TestMvcApplication
{
    public class BindingConfig
    {
        public static void RegisterModelBinders(ModelBinderDictionary binders)
        {
            binders.Add(typeof(CustomerData), new CustomFieldNameModelBinder());
        }
    }
}
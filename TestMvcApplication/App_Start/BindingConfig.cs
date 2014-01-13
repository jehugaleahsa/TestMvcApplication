using System.Web.Mvc;
using MvcUtilities.Binders;
using ViewModels;

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
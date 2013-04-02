using System;
using System.Web.Mvc;

namespace MvcUtilities.Helpers
{
    public static class DisplayAttributeExtensions
    {
        public static MvcHtmlString SetAttribute<TModel, TProp>(this HtmlHelper<TModel> helper, string attribute, Func<TModel, TProp> propertySelector)
        {
            TModel model = helper.ViewData.Model;
            if (model == null)
            {
                return MvcHtmlString.Empty;
            }
            TProp value = propertySelector(model);
            string result = String.Format(@"{0}=""{1}""", attribute, value);
            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString SetFlagAttribute<TModel>(this HtmlHelper<TModel> helper, string attribute, Func<TModel, bool> propertySelector)
        {
            TModel model = helper.ViewData.Model;
            bool isEnabled = propertySelector(model);
            if (isEnabled)
            {
                string result = String.Format(@"{0}=""{0}""", attribute);
                return MvcHtmlString.Create(result);
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }
    }
}
using System;
using System.Web.Mvc;

namespace MvcUtilities.Helpers
{
    public static class DisplayAttributeExtensions
    {
        public static MvcHtmlString SetAttribute<TModel, TProp>(this HtmlHelper<TModel> helper, string attribute, Func<TModel, TProp> propertySelector)
        {
            if (helper == null)
            {
                throw new ArgumentNullException("helper");
            }
            if (propertySelector == null)
            {
                throw new ArgumentNullException("propertySelector");
            }
            TModel model = helper.ViewData.Model;
            if (model == null)
            {
                return MvcHtmlString.Empty;
            }
            TProp value = propertySelector(model);
            string result = String.Format(null, @"{0}=""{1}""", attribute, value);
            return MvcHtmlString.Create(result);
        }
    }
}
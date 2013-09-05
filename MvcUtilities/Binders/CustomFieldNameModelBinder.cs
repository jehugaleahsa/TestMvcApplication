using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MvcUtilities.Binders
{
    public class CustomFieldNameModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object model = Activator.CreateInstance(bindingContext.ModelMetadata.ModelType);
            
            foreach (PropertyInfo propertyInfo in bindingContext.ModelMetadata.ModelType.GetProperties())
            {
                bool isViewModel = propertyInfo.GetCustomAttributes(typeof(NestedViewModelAttribute), true).Any();
                if (isViewModel)
                {
                    ModelBindingContext subContext = new ModelBindingContext(bindingContext);
                    subContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, propertyInfo.PropertyType);

                    IModelBinder subBinder = ModelBinders.Binders.GetBinder(propertyInfo.PropertyType);
                    object subModel = subBinder.BindModel(controllerContext, subContext);
                    propertyInfo.SetValue(model, subModel, null);
                    continue;
                }
                List<string> fieldNames = new List<string>();
                fieldNames.Add(propertyInfo.Name);
                var names = propertyInfo.GetCustomAttributes(typeof(FieldNameAttribute), true)
                    .Cast<FieldNameAttribute>()
                    .Select(attribute => attribute.FieldName);
                fieldNames.AddRange(names);
                foreach (string fieldName in fieldNames)
                {
                    bool wasSet = setValue(bindingContext, model, propertyInfo, fieldName);
                    if (wasSet)
                    {
                        break;
                    }
                }
            }

            bindingContext.ModelMetadata.Model = model;

            ValidationContext context = new ValidationContext(model, null, null);
            List<ValidationResult> results = new List<ValidationResult>();
            if (Validator.TryValidateObject(model, context, results, true))
            {
                foreach (ValidationResult result in results)
                {
                    foreach (string memberName in result.MemberNames)
                    {
                        bindingContext.ModelState.AddModelError(memberName, result.ErrorMessage);
                    }
                }
            }
            return model;
        }

        private static bool setValue(ModelBindingContext context, object model, PropertyInfo propertyInfo, string fieldName)
        {
            ValueProviderResult result = context.ValueProvider.GetValue(fieldName);
            if (result != null)
            {
                try
                {
                    object value = result.ConvertTo(propertyInfo.PropertyType);
                    propertyInfo.SetValue(model, value, null);
                    return true;
                }
                catch
                {
                    string message = String.Format(CultureInfo.CurrentCulture, "Failed to map {0} to {1}.", fieldName, propertyInfo.Name);
                    context.ModelState.AddModelError(propertyInfo.Name, message);
                }
            }
            return false;
        }
    }
}

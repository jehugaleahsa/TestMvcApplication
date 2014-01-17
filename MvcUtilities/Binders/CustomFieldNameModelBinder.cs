using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MvcUtilities.Binders
{
    public class CustomFieldNameModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }
            object model = Activator.CreateInstance(bindingContext.ModelMetadata.ModelType);
            setModelValues(controllerContext, bindingContext, model);
            bindingContext.ModelMetadata.Model = model;
            validateModel(bindingContext);
            return model;
        }

        private static void setModelValues(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
        {
            foreach (PropertyInfo propertyInfo in bindingContext.ModelMetadata.ModelType.GetProperties())
            {
                bool isViewModel = propertyInfo.GetCustomAttributes(typeof(NestedViewModelAttribute), true).Any();
                if (isViewModel)
                {
                    bindNestedViewModel(controllerContext, bindingContext, model, propertyInfo);
                    continue;
                }
                string[] fieldNames = getFieldNames(propertyInfo);
                foreach (string fieldName in fieldNames)
                {
                    bool wasSet = setValue(bindingContext, model, propertyInfo, fieldName);
                    if (wasSet)
                    {
                        break;
                    }
                }
            }
        }

        private static void bindNestedViewModel(ControllerContext controllerContext, ModelBindingContext bindingContext, object model, PropertyInfo propertyInfo)
        {
            ModelBindingContext subContext = new ModelBindingContext(bindingContext);
            subContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, propertyInfo.PropertyType);

            IModelBinder subBinder = ModelBinders.Binders.GetBinder(propertyInfo.PropertyType);
            object subModel = subBinder.BindModel(controllerContext, subContext);
            propertyInfo.SetValue(model, subModel, null);
        }

        private static string[] getFieldNames(PropertyInfo propertyInfo)
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add(propertyInfo.Name);
            var names = propertyInfo.GetCustomAttributes(typeof(FieldNameAttribute), true)
                .Cast<FieldNameAttribute>()
                .Select(attribute => attribute.FieldName);
            fieldNames.AddRange(names);
            return fieldNames.ToArray();
        }

        private static bool setValue(ModelBindingContext context, object model, PropertyInfo propertyInfo, string fieldName)
        {
            ValueProviderResult result = context.ValueProvider.GetValue(fieldName);
            if (result != null)
            {
                try
                {
                    object value = result.ConvertTo(propertyInfo.PropertyType, null);
                    propertyInfo.SetValue(model, value, null);
                    return true;
                }
                catch
                {
                    string message = String.Format(null, "Failed to map {0} to {1}.", fieldName, propertyInfo.Name);
                    context.ModelState.AddModelError(propertyInfo.Name, message);
                }
            }
            return false;
        }

        private static void validateModel(ModelBindingContext bindingContext)
        {
            object model = bindingContext.ModelMetadata.Model;
            ValidationContext context = new ValidationContext(model);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, context, results, true))
            {
                var errorDetails = from result in results
                                   from memberName in result.MemberNames
                                   select new
                                   {
                                       MemberName = memberName,
                                       ErrorMessage = result.ErrorMessage,
                                   };
                foreach (var error in errorDetails)
                {
                    bindingContext.ModelState.AddModelError(error.MemberName, error.ErrorMessage);
                }
            }
        }
    }
}

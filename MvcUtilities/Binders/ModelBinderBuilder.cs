using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace MvcUtilities.Binders
{
    public static class ModelBinderBuilder
    {
        public static BoundModelBinder<TModel> Bind<TModel>()
        {
            return new BoundModelBinder<TModel>();
        }
    }

    public sealed class BoundModelBinder<TModel>
    {
        private Func<TModel> modelCreator;
        private Dictionary<PropertyInfo, PropertyBinder> mappers;

        internal BoundModelBinder()
        {
            modelCreator = () => Activator.CreateInstance<TModel>();
            mappers = new Dictionary<PropertyInfo, PropertyBinder>();
        }

        public BoundModelBinder<TModel> Create(Func<TModel> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }
            this.modelCreator = creator;
            return this;
        }

        public BoundModelBinder<TModel> Map<TProp>(Expression<Func<TModel, TProp>> propertySelector, string fieldName)
        {
            MemberExpression memberExpression = getMember<TProp>(propertySelector);
            if (String.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException("The field name to map to was blank.", "fieldName");
            }
            return map(fieldName, default(TProp), memberExpression);
        }

        public BoundModelBinder<TModel> Map<TProp>(Expression<Func<TModel, TProp>> propertySelector, string fieldName, TProp defaultValue)
        {
            MemberExpression memberExpression = getMember(propertySelector);
            if (String.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException("The field name to map to was blank.", "fieldName");
            }
            return map<TProp>(fieldName, defaultValue, memberExpression);
        }

        private static MemberExpression getMember<TProp>(Expression<Func<TModel, TProp>> propertySelector)
        {
            if (propertySelector == null)
            {
                throw new ArgumentNullException("propertySelector");
            }
            MemberExpression memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null || memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("The property selector did not return a property.", "propertySelector");
            }
            return memberExpression;
        }

        private BoundModelBinder<TModel> map<TProp>(string fieldName, TProp value, MemberExpression memberExpression)
        {
            PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
            mappers[propertyInfo] = new PropertyBinder() { PropertyInfo = propertyInfo, FieldName = fieldName, DefaultValue = value };
            return this;
        }

        public IModelBinder ToBinder()
        {
            return new ModelBinder(modelCreator, mappers.Values);
        }

        private sealed class PropertyBinder
        {
            public PropertyInfo PropertyInfo { get; set; }

            public string FieldName { get; set; }

            public object DefaultValue { get; set; }

            public void BindProperty(ModelBindingContext context)
            {
                ValueProviderResult result = context.ValueProvider.GetValue(FieldName);
                object value;
                if (result == null)
                {
                    value = DefaultValue;
                }
                else
                {
                    try
                    {
                        value = result.ConvertTo(PropertyInfo.PropertyType, null);
                    }
                    catch (Exception exception)
                    {
                        context.ModelState.AddModelError(PropertyInfo.Name, exception);
                        return;
                    }
                }
                PropertyInfo.SetValue(context.Model, value, null);
            }
        }

        private sealed class ModelBinder : IModelBinder
        {
            private readonly Func<TModel> creator;
            private readonly IEnumerable<PropertyBinder> mappers;

            public ModelBinder(Func<TModel> creator, IEnumerable<PropertyBinder> mappers)
            {
                this.creator = creator;
                this.mappers = mappers;
            }

            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                if (bindingContext == null)
                {
                    throw new ArgumentNullException("bindingContext");
                }
                bindingContext.ModelMetadata.Model = creator();
                foreach (PropertyBinder binder in mappers)
                {
                    binder.BindProperty(bindingContext);
                }

                ModelValidator validator = DataAnnotationsModelValidator.GetModelValidator(bindingContext.ModelMetadata, controllerContext);
                IEnumerable<ModelValidationResult> validationResults = validator.Validate(bindingContext.ModelMetadata.Model);
                foreach (ModelValidationResult result in validationResults)
                {
                    bindingContext.ModelState.AddModelError(result.MemberName, result.Message);
                }

                return bindingContext.ModelMetadata.Model;
            }
        }
    }
}
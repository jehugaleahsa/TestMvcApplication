using System;
using System.Reflection;
using System.Reflection.Emit;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Syntax;

namespace TestMvcApplication
{
    public static class NinjectExtensions
    {
        public static IBindingWhenInNamedWithOrOnSyntax<TImplementation> BindProperties<TImplementation>(this IBindingWhenInNamedWithOrOnSyntax<TImplementation> binding)
        {
            binding.OnActivation(getActivator<TImplementation>());
            return binding;
        }

        private static Action<IContext, TImplementation> getActivator<TImplementation>()
        {
            const string name = "Activator";
            Type returnType = typeof(void);
            Type contextType = typeof(IContext);
            Type implementationType = typeof(TImplementation);
            Type[] parameterTypes = new Type[] { contextType, implementationType };
            DynamicMethod method = new DynamicMethod(name, returnType, parameterTypes);

            PropertyInfo kernelPropertyInfo = contextType.GetProperty("Kernel");

            Type extensionsType = typeof(ResolutionExtensions);
            Type kernelType = typeof(IKernel);
            Type typeType = typeof(Type);
            Type[] tryGetParameterTypes = new Type[] { kernelType, typeType, typeof(IParameter[]) };
            MethodInfo tryGetMethodInfo = extensionsType.GetMethod("TryGet", tryGetParameterTypes);

            MethodInfo getTypeFromHandleMethodInfo = typeType.GetMethod("GetTypeFromHandle");

            ILGenerator generator = method.GetILGenerator();
            LocalBuilder valueBuilder = generator.DeclareLocal(typeof(Object));
            foreach (PropertyInfo propertyInfo in implementationType.GetProperties())
            {
                Label doneLabel = generator.DefineLabel();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Callvirt, kernelPropertyInfo.GetGetMethod());
                generator.Emit(OpCodes.Ldtoken, propertyInfo.PropertyType);
                generator.Emit(OpCodes.Call, getTypeFromHandleMethodInfo);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Newarr, typeof(IParameter));
                generator.Emit(OpCodes.Call, tryGetMethodInfo);
                generator.Emit(OpCodes.Stloc, valueBuilder);

                generator.Emit(OpCodes.Ldloc, valueBuilder);
                generator.Emit(OpCodes.Brfalse, doneLabel);

                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldloc, valueBuilder);
                generator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                MethodInfo propertySetter = propertyInfo.GetSetMethod();
                generator.Emit(OpCodes.Callvirt, propertySetter);

                generator.MarkLabel(doneLabel);
            }
            generator.Emit(OpCodes.Ret);

            return (Action<IContext, TImplementation>)method.CreateDelegate(typeof(Action<IContext, TImplementation>));
        }
    }
}
using EmployeeManagement.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EmployeeManagement.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// This method is used to register the types implementing any of the <see cref="IScopedService"/>, <see cref="ITransientService"/> and <see cref="ISingletonService"/>
        /// interfaces.
        /// </summary>
        /// <typeparam name="T">Any of the <see cref="IScopedService"/>, <see cref="ITransientService"/> and <see cref="ISingletonService"/> interfaces.</typeparam>
        /// <param name="services">Type of <see cref="IServiceCollection"/>.</param>
        public static void RegisterAllTypes<T>(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            ServiceLifetime lifetime = ServiceLifetime.Scoped;

            switch (typeof(T).Name)
            {
                case nameof(ITransientService):
                    lifetime = ServiceLifetime.Transient;
                    break;
                case nameof(IScopedService):
                    lifetime = ServiceLifetime.Scoped;
                    break;
                case nameof(ISingletonService):
                    lifetime = ServiceLifetime.Singleton;
                    break;
            }

            List<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Distinct().ToList();
            string[] loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            string[] referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            List<string> toLoadAssemblies = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            foreach (var path in toLoadAssemblies)
            {
                Assembly assembly = Assembly.LoadFrom(path);
                loadedAssemblies.Add(assembly);
            }

            // toLoadAssemblies.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));
            List<Type> implementations = loadedAssemblies
                .SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass).ToList();

            foreach (Type implementation in implementations)
            {
                Type[] interfaceTypes = implementation.GetInterfaces().ToArray();
                foreach (Type interfaceType in interfaceTypes)
                {
                    if (interfaceType != typeof(ITransientService) && interfaceType != typeof(IScopedService) && interfaceType != typeof(ISingletonService))
                    {
                        services.Add(new ServiceDescriptor(interfaceType, implementation, lifetime));
                    }
                }
            }
        }
    }
}

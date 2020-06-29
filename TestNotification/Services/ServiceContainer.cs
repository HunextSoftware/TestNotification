﻿using System;
using System.Collections.Generic;

namespace TestNotification.Services
{
    //This is a trimmed-down version of the ServiceContainer class from the XamCAT repository. It will be used as a light-weight IoC (Inversion of Control) container.
    public class ServiceContainer
    {
        static readonly Dictionary<Type, Lazy<object>> services
           = new Dictionary<Type, Lazy<object>>();

        public static void Register<T>(Func<T> function)
            => services[typeof(T)] = new Lazy<object>(() => function());

        public static T Resolve<T>()
            => (T)Resolve(typeof(T));

        public static object Resolve(Type type)
        {
            {
                if (services.TryGetValue(type, out var service))
                    return service.Value;

                throw new KeyNotFoundException($"Service not found for type '{type}'");
            }
        }
    }
}

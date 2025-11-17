using System;
using System.Collections.Generic;

namespace Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> services = new();

        /// <summary>
        /// Registers a service instance so it can be retrieved globally.
        /// </summary>
        public static void Register<T>(T service) where T : IService
        {
            var type = typeof(T);
            services.TryAdd(type, service);
        }

        /// <summary>
        /// Returns a previously registered service.
        /// </summary>
        public static T Get<T>() where T : IService
        {
            return (T)services[typeof(T)];
        }
    }
}
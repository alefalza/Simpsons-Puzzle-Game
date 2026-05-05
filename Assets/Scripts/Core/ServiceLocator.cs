using System;
using System.Collections.Generic;
using System.Linq;
using Boot;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Improved Service Locator with auto-initialization and type caching
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> services = new();
        private static readonly Dictionary<Type, Type> typeCache = new();
        private static bool isInitialized = false;

        public static void Initialize(ServiceConfiguration config, ServiceBootstrap bootstrap)
        {
            if (isInitialized)
            {
                Debug.LogWarning("[ServiceLocator] Already initialized!");
                return;
            }

            Debug.Log($"[ServiceLocator] Initializing {config.Services.Count} services...");

            foreach (var serviceDefinition in config.Services)
            {
                if (serviceDefinition == null)
                {
                    Debug.LogWarning("[ServiceLocator] Null service definition found, skipping.");
                    continue;
                }

                try
                {
                    IService instance = serviceDefinition.CreateInstance(bootstrap);
                    Type serviceType = instance.GetType();

                    // Find all IService interfaces it implements
                    foreach (var interfaceType in serviceType.GetInterfaces())
                    {
                        if (interfaceType != typeof(IService) && typeof(IService).IsAssignableFrom(interfaceType))
                        {
                            services[interfaceType] = instance;
                            Debug.Log($"[ServiceLocator] Registered {interfaceType.Name}");
                        }
                    }

                    // Also register by concrete type
                    services[serviceType] = instance;
                    
                    instance.Initialize();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ServiceLocator] Failed to create service from {serviceDefinition.name}: {e}");
                }
            }

            // Mark as initialized before PostInitialize so services can resolve dependencies via ServiceLocator.Get<T>().
            isInitialized = true;

            // Second phase: allow services to safely interact with other services
            // after everything has been instantiated + initialized.
            foreach (var service in services.Values.Distinct())
            {
                if (service is IPostInitializableService postInitializable)
                {
                    try
                    {
                        postInitializable.PostInitialize();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[ServiceLocator] Error in PostInitialize for {service.GetType().Name}: {e}");
                    }
                }
            }
            Debug.Log("[ServiceLocator] Initialization complete!");
        }

        public static T Get<T>() where T : class, IService
        {
            if (!isInitialized)
            {
                Debug.LogError("[ServiceLocator] Not initialized! Call Initialize first.");
                return null;
            }

            Type type = typeof(T);
            
            // Try cached type first
            if (typeCache.TryGetValue(type, out Type cachedType))
            {
                type = cachedType;
            }

            if (services.TryGetValue(type, out IService service))
            {
                // Cache the type for faster future lookups
                if (!typeCache.ContainsKey(typeof(T)))
                {
                    typeCache[typeof(T)] = type;
                }
                return service as T;
            }

            Debug.LogError($"[ServiceLocator] Service of type {type.Name} not found!");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class, IService
        {
            service = Get<T>();
            return service != null;
        }

        public static void Shutdown()
        {
            Debug.Log("[ServiceLocator] Shutting down services...");

            foreach (var service in services.Values)
            {
                try
                {
                    service.Shutdown();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ServiceLocator] Error shutting down service: {e}");
                }
            }

            services.Clear();
            typeCache.Clear();
            isInitialized = false;
        }
    }
}

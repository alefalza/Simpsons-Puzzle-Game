using Boot;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Base ScriptableObject for defining services
    /// </summary>
    public abstract class ServiceDefinition : ScriptableObject
    {
        public abstract IService CreateInstance(ServiceBootstrap bootstrap);
    }

    /// <summary>
    /// Generic ScriptableObject for stateless services
    /// </summary>
    public abstract class ServiceDefinition<T> : ServiceDefinition where T : IService, new()
    {
        public override IService CreateInstance(ServiceBootstrap bootstrap)
        {
            return new T();
        }
    }
}

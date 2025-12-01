using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Main service configuration
    /// Create Asset Menu: Assets/Create/Core/Service Configuration
    /// </summary>
    [CreateAssetMenu(fileName = "ServiceConfiguration", menuName = "Core/Service Configuration")]
    public class ServiceConfiguration : ScriptableObject
    {
        [SerializeField] private List<ServiceDefinition> services = new();

        public List<ServiceDefinition> Services => services;
    }
}

using Boot;
using UnityEngine;

namespace Core.Services.SettingsService
{
    [CreateAssetMenu(fileName = "SettingsServiceDef", menuName = "Core/Services/Settings Service")]
    public class SettingsServiceDefinition : ServiceDefinition
    {
        public override IService CreateInstance(ServiceBootstrap bootstrap)
        {
            return new SettingsService();
        }
    }
}

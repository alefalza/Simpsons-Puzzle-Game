using Boot;
using UnityEngine;

namespace Core.Services.LevelProgressionService
{
    [CreateAssetMenu(fileName = "LevelProgressionServiceDef", menuName = "Core/Services/Level Progression Service")]
    public class LevelProgressionServiceDefinition : ServiceDefinition<LevelProgressionService>
    {
        public override IService CreateInstance(ServiceBootstrap bootstrap)
        {
            return new LevelProgressionService();
        }
    }
}

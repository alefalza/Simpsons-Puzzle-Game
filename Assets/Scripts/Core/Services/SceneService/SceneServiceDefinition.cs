using Boot;
using UnityEngine;

namespace Core.Services.SceneService
{
    [CreateAssetMenu(fileName = "SceneServiceDef", menuName = "Core/Services/Scene Service")]
    public class SceneServiceDefinition : ServiceDefinition
    {
        public override IService CreateInstance(ServiceBootstrap bootstrap)
        {
            return new SceneService(bootstrap);
        }
    }
}

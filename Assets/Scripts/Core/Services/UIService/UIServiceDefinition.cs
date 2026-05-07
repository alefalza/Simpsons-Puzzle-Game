using Boot;
using UnityEngine;

namespace Core.Services.UIService
{
    [CreateAssetMenu(fileName = "UIServiceDef", menuName = "Core/Services/UI Service")]
    public class UIServiceDefinition : ServiceDefinition
    {
        public override IService CreateInstance(ServiceBootstrap bootstrap)
        {
            return new UIService(bootstrap.LoadingOverlay, bootstrap.LoadingBar);
        }
    }
}

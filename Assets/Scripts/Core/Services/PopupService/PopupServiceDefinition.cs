using Boot;
using UnityEngine;

namespace Core.Services.PopupService
{
    [CreateAssetMenu(fileName = "PopupServiceDef", menuName = "Core/Services/Popup Service")]
    public class PopupServiceDefinition : ServiceDefinition
    {
        public override IService CreateInstance(ServiceBootstrap bootstrap)
        {
            return new PopupService(bootstrap.PopupRoot);
        }
    }
}

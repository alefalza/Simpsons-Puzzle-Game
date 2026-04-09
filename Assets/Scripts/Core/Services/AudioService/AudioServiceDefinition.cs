using Boot;
using UnityEngine;

namespace Core.Services.AudioService
{
    [CreateAssetMenu(fileName = "AudioServiceDef", menuName = "Core/Services/Audio Service")]
    public class AudioServiceDefinition : ServiceDefinition
    {
        public override IService CreateInstance(ServiceBootstrap bootstrap)
        {
            return new AudioService(bootstrap.MainMixer, bootstrap.MusicSource, bootstrap.SfxSource);
        }
    }
}

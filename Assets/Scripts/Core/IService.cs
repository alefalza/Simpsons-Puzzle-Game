namespace Core
{
    /// <summary>
    /// Base interface for all services
    /// </summary>
    public interface IService
    {
        void Initialize();
        void Shutdown();
    }

    /// <summary>
    /// Optional lifecycle step invoked after ALL services are created and initialized.
    /// Use this when a service needs to safely interact with other services at startup.
    /// </summary>
    public interface IPostInitializableService : IService
    {
        void PostInitialize();
    }
}

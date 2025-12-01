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
}

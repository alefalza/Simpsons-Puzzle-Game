namespace Core.Services.SceneService
{
    public interface ISceneService : IService
    {
        void LoadScene(string sceneName);
        void LoadSceneAdditive(string sceneName);
        void UnloadScene(string sceneName);
    }
}

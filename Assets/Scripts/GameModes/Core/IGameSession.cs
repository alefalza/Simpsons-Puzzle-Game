namespace GameModes.Core
{
    public interface IGameSession
    {
        void Retry();
    }

    public static class GameSession
    {
        public static IGameSession Current { get; internal set; }
    }
}

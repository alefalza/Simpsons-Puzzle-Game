public static class GameConstants
{
    public const string MAIN_MENU_SCENE = "MainMenuScene";
        
    public static class BubbleMerge
    {
        public const int ScorePerTier = 10;
    }

    public static class DrinkSort
    {
        public const int ScorePerMatch = 10;
        public const float InitialPopulateDelay = 0.1f;
        public const float ItemPopulateDelay = 0.05f;
        public const float MatchProcessDelay = 0.2f;
        public const float PostPopulateDelay = 0.1f;
    }

    public static class DonutStack
    {
        public const float MatchProcessDelay = 0.2f;
        public const float PieceRemoveDelay = 0.05f;
        public const float PostDestroyDelay = 0.2f;
        public const float NewTurnDelay = 0.5f;
    }
}

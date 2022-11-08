namespace Game.Utility
{
    public static class GameTag
    {
        // public static string ScorePlatform = "ScorePlatform";
    }

    public static class GameLayer
    {
        public const int Default = 0;
        public const int Water = 4;
        public const int UI = 5;

    }

    public static class GameLayerMask
    {
        public const int Default = 1;
        public const int Water = 1 << GameLayer.Water;
        public const int UI = 1 << GameLayer.UI;
    }
}
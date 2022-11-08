namespace Game.GameSystem
{
    public static class SystemPriority
    {
        private static int id;
        public static readonly int MotorcycleRespawnSystem = id++;
        public static readonly int Common = id++;
    }
}
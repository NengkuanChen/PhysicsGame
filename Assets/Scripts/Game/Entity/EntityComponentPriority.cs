namespace Game.Entity
{
    public static class EntityComponentPriority
    {
        private static int priority;
        public static readonly int MotorcycleRoadDetector = priority++;
        public static readonly int Common = priority++;
        public static readonly int BeforeMovement = priority++;
        public static readonly int MotorcycleMovement = priority++;
        public static readonly int AfterMovement = priority++;
        public static readonly int BeforeBodyControl = priority++;
        public static readonly int MotorcycleBodyControl = priority++;
        public static readonly int AfterBodyControl = priority++;
        public static readonly int MotorcyclePreventExceedRoadWidth = priority++;
        public static readonly int AfterEverything = priority++;
    }
}
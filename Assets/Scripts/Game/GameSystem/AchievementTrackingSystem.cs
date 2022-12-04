namespace Game.GameSystem
{
    public class AchievementTrackingSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        public static AchievementTrackingSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as AchievementTrackingSystem;
        }

        public void OnGameReset()
        {
            
        }
    }


    public enum AchievementType
    {
        PlatformBreaker,
        
    }
}
using Cysharp.Threading.Tasks;

namespace Game.GameSystem
{
    /// <summary>
    /// 正式游戏流程进入系统
    /// </summary>
    public class GameStartSystem : SystemBase
    {
        public static GameStartSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as GameStartSystem;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        internal override void OnEnable()
        {
            base.OnEnable();
            Log.Info("游戏启动");

        #if ONLYCAR_FRAMEWORK_REPORT
            var reportEventSystem = ReportEventSystem.Get();
            reportEventSystem.ReportGameLaunch();
        #endif

            StartGameAsync().Forget();
        }

        private async UniTaskVoid StartGameAsync()
        {
        }
    }
}
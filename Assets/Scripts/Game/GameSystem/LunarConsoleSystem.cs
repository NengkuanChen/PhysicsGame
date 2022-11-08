using Game.GameSystem;
using LunarConsolePlugin;
using UnityEngine;

namespace Game.GameSystem
{
    public class LunarConsoleSystem : SystemBase
    {
        public static LunarConsoleSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as LunarConsoleSystem;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        internal override void OnEnable()
        {
            base.OnEnable();
            
            LunarConsole.RegisterAction("清空存档",
                () =>
                {
                    var gameDataSystem = GameDataSystem.Get();
                    gameDataSystem?.ClearSaveData();
                    Application.Quit();
                });
        }
    }
}
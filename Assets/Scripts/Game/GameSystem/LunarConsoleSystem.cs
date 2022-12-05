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
        
        [CVarContainer]
        public static class LunarDebugVariables
        {
            
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            
            LunarConsole.RegisterAction("Clear Saves",
                () =>
                {
                    var gameDataSystem = GameDataSystem.Get();
                    gameDataSystem?.ClearSaveData();
                    Application.Quit();
                });
            
        }
    }
}
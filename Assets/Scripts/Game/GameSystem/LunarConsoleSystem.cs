using Game.GameSystem;
using Game.PlatForm;
using Game.Scene;
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
            public static readonly CEnumVar<PlatformDebugMode> PlatformDebug =
                new CEnumVar<PlatformDebugMode>("Platform Debug Mode", PlatformDebugMode.Disabled);
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
            LunarConsole.RegisterAction("Reboot Gyroscope",
                (() =>
                {
                    var playerInputSystem = PlayerInputSystem.Get();
                    playerInputSystem?.RebootGyroscope();
                }));
            LunarConsole.RegisterAction("Stop Scroll", () =>
            {
                if (ScrollRoot.Current != null)
                {
                    ScrollRoot.Current.PauseScroll();
                }
            });
            
            LunarConsole.RegisterAction("Start Scroll", (() =>
            {
                if (ScrollRoot.Current != null)
                {
                    ScrollRoot.Current.ContinueScroll();
                }
            }));
            
            LunarDebugVariables.PlatformDebug.AddDelegate((debugMode) =>
            {
                if (ProceduralPlatformGenerateSystem.Get() != null)
                {
                    ProceduralPlatformGenerateSystem.Get()
                        .EnablePlatformDebugMode(((CEnumVar<PlatformDebugMode>)debugMode).EnumValue);
                }
            });
        }
    }
}
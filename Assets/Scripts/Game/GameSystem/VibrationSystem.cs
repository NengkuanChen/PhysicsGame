// using UnityEngine;
//
// namespace Game.GameSystem
// {
//     public class VibrationSystem : SystemBase
//     {
//         public static VibrationSystem Get()
//         {
//             return SystemEntry.GetSystem(id) as VibrationSystem;
//         }
//
//         private static int id = UniqueIdGenerator.GetUniqueId();
//         internal override int ID => id;
//
//         private float lastVibrateTime;
//
//         public void WeakVibrate()
//         {
//             if (CanVibrate())
//             {
//                 Vibration.VibratePop();
//                 lastVibrateTime = Time.unscaledTime;
//             }
//         }
//
//         public void StrongVibrate()
//         {
//             if (CanVibrate())
//             {
//                 Vibration.VibratePeek();
//                 lastVibrateTime = Time.unscaledTime;
//             }
//         }
//
//         public void DefaultVibrate()
//         {
//             if (CanVibrate())
//             {
//                 Vibration.Vibrate();
//                 lastVibrateTime = Time.unscaledTime;
//             }
//         }
//
//         private bool CanVibrate()
//         {
//             var gameDataSystem = GameDataSystem.Get();
//             // if (!gameDataSystem.VibrationEnable)
//             // {
//             //     return false;
//             // }
//
//             var currentTime = Time.unscaledTime;
//             return currentTime - lastVibrateTime >= .15f;
//         }
//     }
// }
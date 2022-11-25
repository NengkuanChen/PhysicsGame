using Game.Ball;
using Game.GameEvent;
using UnityEngine;

namespace Game.PlatForm
{
    public class MagneticTrigger: MonoBehaviour
    {
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.gameObject.CompareTag("Player"))
        //     {
        //         var currentBall = BallSystem.Get().playerCurrentBall;
        //         if (currentBall is IronBall)
        //         {
        //             Framework.EventComponent.Fire(this, OnBallEnterMagneticFieldEventArgs.Create(this, true));
        //         }
        //     }
        // }
        //
        // private void OnTriggerExit(Collider other)
        // {
        //     if (other.gameObject.CompareTag("Player"))
        //     {
        //         var currentBall = BallSystem.Get().playerCurrentBall;
        //         if (currentBall is IronBall)
        //         {
        //             Framework.EventComponent.Fire(this, OnBallEnterMagneticFieldEventArgs.Create(this, false));
        //         }
        //     }
        // }
    }
}
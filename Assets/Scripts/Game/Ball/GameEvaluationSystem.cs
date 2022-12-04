using Game.GameEvent;
using Game.GameSystem;
using GameFramework.Event;
using UnityEngine;

namespace Game.Ball
{
    public class GameEvaluationSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private float currentSurvivalTime = 0f;
        public float CurrentSurvivalTime => currentSurvivalTime;

        private bool hasEnd = false;

        public static GameEvaluationSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as GameEvaluationSystem;
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            Framework.EventComponent.Subscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }

        private void OnBallDead(object sender, GameEventArgs e)
        {
            hasEnd = true;
        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (!hasEnd)
            {
                currentSurvivalTime += Time.deltaTime;
            }
        }

       

        internal override void OnDisable()
        {
            base.OnDisable();
            Framework.EventComponent.Unsubscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }
    }
}
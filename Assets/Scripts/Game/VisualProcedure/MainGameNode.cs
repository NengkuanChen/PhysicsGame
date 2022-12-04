using FlatNode.Runtime;
using Game.Ball;
using Game.GameEvent;
using Game.UI.Form;
using Game.UI.Form.Control;
using Game.Utility;
using GameFramework.Event;
using UnityEngine;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("Main Game Node", "Main Game Node")]
    [NodeFlowOutPort(FlowOutPort.Evaluation, "Evaluation")]
    public class MainGameNode: ProcedureNodeBase
    {
        private static class FlowOutPort
        {
            public const int Evaluation = 0;
        }

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);
#if UNITY_EDITOR
            UIUtility.OpenForm(EditorTestingControlForm.UniqueId);
#else
            UIUtility.OpenForm(ControlForm.UniqueId);
#endif
            UIUtility.OpenForm(BattleForm.UniqueId);
            new GameEvaluationSystem();
            Framework.EventComponent.Subscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
            Framework.EventComponent.Subscribe(OnGamePauseEventArgs.UniqueId, OnGamePause);
        }
        

        private void OnGamePause(object sender, GameEventArgs e)
        {
            var arg = e as OnGamePauseEventArgs;
            if (arg.IsPause)
            {
                Time.timeScale = 0f;
                UIUtility.OpenForm(PauseForm.UniqueId);
            }
            else
            {
                Time.timeScale = 1f;
                UIUtility.CloseForm(PauseForm.UniqueId);
            }
        }

        private void OnBallDead(object sender, GameEventArgs e)
        {
            ExitProcedure(FlowOutPort.Evaluation);
        }

        public override void OnExit()
        {
            base.OnExit();
            var battleForm = UIUtility.GetForm(BattleForm.UniqueId) as BattleForm;
            if (battleForm != null)
            {
                battleForm.MoveOutForm();
            }
            Framework.EventComponent.Unsubscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
            Framework.EventComponent.Unsubscribe(OnGamePauseEventArgs.UniqueId, OnGamePause);
        }
    }
}
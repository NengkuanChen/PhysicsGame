using FlatNode.Runtime;
using Game.GameEvent;
using Game.UI.Form.Control;
using Game.Utility;
using GameFramework.Event;
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
            Framework.EventComponent.Subscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }

        private void OnBallDead(object sender, GameEventArgs e)
        {
            ExitProcedure(FlowOutPort.Evaluation);
        }

        public override void OnExit()
        {
            base.OnExit();
            Framework.EventComponent.Unsubscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }
    }
}
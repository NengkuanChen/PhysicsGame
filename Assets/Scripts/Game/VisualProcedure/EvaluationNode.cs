using Cysharp.Threading.Tasks;
using FlatNode.Runtime;
using Game.Scene;
using Game.UI.Form;
using Game.UI.Form.Control;
using Game.Utility;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("Evaluation Node", "Evaluation Node")]
    [NodeFlowOutPort(FlowOutPort.ResetScene, "Reset Scene")]
    public class EvaluationNode: ProcedureNodeBase
    {
        private static class FlowOutPort
        {
            public const int ResetScene = 0;
        }

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);
            Evaluation().Forget();
#if UNITY_EDITOR
            UIUtility.CloseForm(EditorTestingControlForm.UniqueId);
#else
            UIUtility.CloseForm(ControlForm.UniqueId);
#endif
        }

        public async UniTaskVoid Evaluation()
        {
            var evaluationForm = await UIUtility.OpenFormAsync<EvaluationForm>(EvaluationForm.UniqueId);
            ScrollRoot.Current.StopScroll();
            await UniTask.WaitUntil(() => evaluationForm.HasFinished);
            evaluationForm.CloseSelf();
            ExitProcedure(FlowOutPort.ResetScene);
        }
    }
}
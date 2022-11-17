using FlatNode.Runtime;
using Game.UI.Form.Control;
using Game.Utility;
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
        }
    }
}
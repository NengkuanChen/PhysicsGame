using Cysharp.Threading.Tasks;
using FlatNode.Runtime;
using Game.Ball;
using Game.PlatForm;
using Game.Scene;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("ResetSceneNode", "ResetSceneNode")]
    [NodeFlowOutPort(FlowOutPort.WaitingGameStart, "WaitingGameStart")]
    public class ResetSceneNode: ProcedureNodeBase
    {
        static class FlowOutPort
        {
            public const int WaitingGameStart = 0;
        }

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);
            ResetScene().Forget();
        }

        private async UniTask ResetScene()
        {
            ScrollRoot.Current.OnSceneReset();
            BallSystem.Get().Reset();
            await ProceduralPlatformGenerateSystem.Get().ResetScene();
            ExitProcedure(FlowOutPort.WaitingGameStart);
        }
        
        
    }
}
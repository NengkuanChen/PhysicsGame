using FlatNode.Runtime;
using Game.GameEvent;
using GameFramework.Event;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("游戏主界面", "游戏主界面")]
    [NodeFlowOutPort(FlowOutPort.GameStart, "游戏开始")]
    public class MainMenuNode: ProcedureNodeBase
    {
        private static class FlowOutPort
        {
            public const int GameStart = 0;
        }

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);
            Framework.EventComponent.Subscribe(OnGameStartEventArgs.UniqueId, OnGameStartButtonClicked);
        }


        private void OnGameStartButtonClicked(object o, GameEventArgs e)
        {
            
        }

        public override void OnExit()
        {
            base.OnExit();
            Framework.EventComponent.Unsubscribe(OnGameStartEventArgs.UniqueId, OnGameStartButtonClicked);
        }
    }
}
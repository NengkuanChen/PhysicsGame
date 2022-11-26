using FlatNode.Runtime;
using Game.Ball;
using Game.GameEvent;
using Game.GameSystem;
using Game.Scene;
using Game.UI.Form;
using Game.Utility;
using GameFramework.Event;
using UnityEngine;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("WaitingStartNode", "WaitingStartNode")]
    [NodeFlowOutPort(FlowOutPort.GameStart, "GameStart")]
    public class WaitingStartNode: ProcedureNodeBase
    {
        public static class FlowOutPort
        {
            public const int GameStart = 0;
        }
        

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);
            var camera = GameCameraSystem.Get().GameCameraEntity;
            camera.transform.position = CameraSpawningPoint.Current.transform.position;
            camera.transform.rotation = CameraSpawningPoint.Current.transform.rotation; 
            UIUtility.OpenForm(WaitingStartForm.UniqueId);

            // var cannonTransform = CannonSystem.Get().CurrentCannon.transform;
            // cannonTransform.position = CannonSpawningPoint.Current.transform.position;
            // cannonTransform.rotation = CannonSpawningPoint.Current.transform.rotation;
            // var ballEntity = BallSystem.Get().playerCurrentBall;
            BallSystem.Get().OnWaitingGameStart();
            Framework.EventComponent.Subscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
        }

        public void OnGameStart(object o, GameEventArgs e)
        {
            ExitProcedure(FlowOutPort.GameStart);
        }

        public override void OnExit()
        {
            base.OnExit();
            Framework.EventComponent.Unsubscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
        }
    }
}
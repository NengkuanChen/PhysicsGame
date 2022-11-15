using FlatNode.Runtime;
using Game.Ball;
using Game.GameSystem;
using Game.Scene;
using UnityEngine;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("PrototypeNode", "PrototypeNode")]
    [NodeFlowOutPort(FlowOutPort.Finish, "Finish")]
    public class PrototypeNode: ProcedureNodeBase
    {
        public static class FlowOutPort
        {
            public const int Finish = 0;
        }

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);
            var camera = GameCameraSystem.Get().GameCameraEntity;
            camera.transform.position = CameraSpawningPoint.Current.transform.position;
            camera.transform.rotation = CameraSpawningPoint.Current.transform.rotation;
            // var cannonTransform = CannonSystem.Get().CurrentCannon.transform;
            // cannonTransform.position = CannonSpawningPoint.Current.transform.position;
            // cannonTransform.rotation = CannonSpawningPoint.Current.transform.rotation;
            var ballEntity = BallSystem.Get().CurrentBall;
            BallSystem.Get().OnWaitingGameStart();
            
        }
    }
}
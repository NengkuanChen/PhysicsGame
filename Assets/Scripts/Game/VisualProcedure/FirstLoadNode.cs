using Cysharp.Threading.Tasks;
using FlatNode.Runtime;
using Game.Ball;
using Game.Car;
using Game.Entity;
using Game.GameCamera;
using Game.GameSystem;
using Game.UI.Form;
using Game.Utility;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("第一次载入", "第一次载入")]
    [NodeFlowOutPort(FlowOutPort.LoadComplete, "完成")]
    public class FirstLoadNode : ProcedureNodeBase
    {
        private static class FlowOutPort
        {
            public const int LoadComplete = 0;
        }

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);

            LoadAsync().Forget();
        }

        private async UniTaskVoid LoadAsync()
        {
            LoadingForm loadingForm = await UIUtility.OpenFormAsync<LoadingForm>(LoadingForm.UniqueId);
            //载入场景
            await SceneUtility.LoadSceneAsync("Prototype");
            var cameraSystem = new GameCameraSystem();
            // var cannonSystem = new CannonSystem();
            await cameraSystem.LoadCameraAsync();
            // await cannonSystem.LoadCannonAsync();
            new PlayerInputSystem();
            var ballSystem = new BallSystem();
            await ballSystem.LoadBallEntityAsync();
            await UIUtility.OpenFormAsync<WaitingStartForm>(WaitingStartForm.UniqueId);
            UIUtility.CloseForm(LoadingForm.UniqueId);
            ExitProcedure(FlowOutPort.LoadComplete);
            
            
            //实际项目中，载入最好放在单独的System中。remove system的时候，就卸载对应的资源
            //载入车辆
            // var car = await EntityUtility.ShowEntityAsync<CarEntity>("Car/Car01", EntityGroupName.Car);
            // car.AddComponent(new CarRotate());
            //载入相机
            
            // camera.AddComponent(new CameraLookAtCar(car.CachedTransform));
        }
    }
}
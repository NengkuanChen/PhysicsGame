using System.Globalization;
using Cysharp.Threading.Tasks;
using FlatNode.Runtime;
using Game.Report;
using Game.Utility;
using UnityEngine;
using VisualProcedure.Runtime;
using VisualProcedure.Runtime.ProcedureNode;

namespace Game.VisualProcedure
{
    [ProcedureGraphNode("游戏启动初始化", "游戏启动初始化")]
    [NodeFlowOutPort(FlowOutPort.InitComplete, "载入完成")]
    public class GameStartUpInitNode : ProcedureNodeBase
    {
        private static class FlowOutPort
        {
            public const int InitComplete = 0;
        }

        public override void OnEnter(NodeTransitionParameter parameter)
        {
            base.OnEnter(parameter);

            InitAsync().Forget();
        }

        private async UniTaskVoid InitAsync()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            Physics.autoSyncTransforms = true;
            Log.Info($"screen resolution, width: {Screen.width}, height: {Screen.height}");

            await GameStartUtility.StartLoadAsync();
            ExitProcedure(FlowOutPort.InitComplete);
        }
    }
}
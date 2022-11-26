using System;
using System.Collections.Generic;

namespace VisualProcedure.Runtime
{
    public static class ProcedureFlowMap
    {
        private static Dictionary<(Type, int), Func<int, VisualProcedure.Runtime.ProcedureNode.ProcedureNodeBase>> flowMap = new Dictionary<(Type, int), Func<int, VisualProcedure.Runtime.ProcedureNode.ProcedureNodeBase>>(new FlowMapKeyComparer())
        {
            {
                (typeof(Game.VisualProcedure.GameStartUpInitNode), 0), portId =>
                {
                    return portId switch
                    {
                        0 => new Game.VisualProcedure.FirstLoadNode(){ ID = 1 },
                        _ => null
                    };
                }
            },
            {
                (typeof(Game.VisualProcedure.FirstLoadNode), 1), portId =>
                {
                    return portId switch
                    {
                        0 => new Game.VisualProcedure.WaitingStartNode(){ ID = 3 },
                        _ => null
                    };
                }
            },
            {
                (typeof(Game.VisualProcedure.WaitingStartNode), 3), portId =>
                {
                    return portId switch
                    {
                        0 => new Game.VisualProcedure.MainGameNode(){ ID = 4 },
                        _ => null
                    };
                }
            },
            {
                (typeof(Game.VisualProcedure.EvaluationNode), 5), portId =>
                {
                    return portId switch
                    {
                        0 => new Game.VisualProcedure.ResetSceneNode(){ ID = 6 },
                        _ => null
                    };
                }
            },
            {
                (typeof(Game.VisualProcedure.MainGameNode), 4), portId =>
                {
                    return portId switch
                    {
                        0 => new Game.VisualProcedure.EvaluationNode(){ ID = 5 },
                        _ => null
                    };
                }
            },
            {
                (typeof(Game.VisualProcedure.ResetSceneNode), 6), portId =>
                {
                    return portId switch
                    {
                        0 => new Game.VisualProcedure.WaitingStartNode(){ ID = 3 },
                        _ => null
                    };
                }
            },
        };
        public static Dictionary<(Type, int), Func<int, VisualProcedure.Runtime.ProcedureNode.ProcedureNodeBase>> FlowMap => flowMap;
        public static VisualProcedure.Runtime.ProcedureNode.ProcedureNodeBase GetEntranceNode()
        {
            return new Game.VisualProcedure.GameStartUpInitNode() { ID = 0 }; 
        }
    }
}

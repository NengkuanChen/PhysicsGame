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
                        0 => new Game.VisualProcedure.MainMenuNode(){ ID = 2 },
                        _ => null
                    };
                }
            },
            {
                (typeof(Game.VisualProcedure.MainMenuNode), 2), portId =>
                {
                    return portId switch
                    {
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

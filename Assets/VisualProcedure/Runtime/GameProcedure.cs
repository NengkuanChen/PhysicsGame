using System;
using UnityEngine;
using VisualProcedure.Runtime.ProcedureNode;

namespace VisualProcedure.Runtime
{
    public class GameProcedure : MonoBehaviour
    {
        private static GameProcedure current;
        public static GameProcedure Current => current;

        private ProcedureNodeBase runningNode;
        public static ProcedureNodeBase RunningNode
        {
            get
            {
                if (current != null)
                {
                    return current.runningNode;
                }

                return null;
            }
        }

        private void Awake()
        {
            current = this;
        }

        public void StartProcedure()
        {
            runningNode = ProcedureFlowMap.GetEntranceNode();
            runningNode.OnEnter(null);
        }

        private void OnDestroy()
        {
            current = null;
        }

        private void Update()
        {
            runningNode?.OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }

        public void ChangeProcedure(ProcedureNodeBase node, int flowOutPortId, NodeTransitionParameter parameter)
        {
            if (ProcedureFlowMap.FlowMap.TryGetValue((node.GetType(), node.ID), out var func))
            {
                var nextProcedureNode = func.Invoke(flowOutPortId);
                if (nextProcedureNode == null)
                {
                    throw new Exception($"流程{node.GetType().FullName}的{flowOutPortId}号流程切换口,没有连接任何流程");
                }

                runningNode.OnExit();
                runningNode = nextProcedureNode;
                nextProcedureNode.OnEnter(parameter);
            }
        }
    }
}
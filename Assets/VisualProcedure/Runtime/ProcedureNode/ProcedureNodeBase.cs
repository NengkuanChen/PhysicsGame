using System;
using UnityEngine;

namespace VisualProcedure.Runtime.ProcedureNode
{
    public abstract class ProcedureNodeBase
    {
        private int id;
        public int ID
        {
            get => id;
            set => id = value;
        }

        public virtual void OnEnter(NodeTransitionParameter parameter)
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        protected void ExitProcedure(int flowOutPortId, NodeTransitionParameter parameter = null)
        {
            GameProcedure.Current.ChangeProcedure(this, flowOutPortId, parameter);
        }
    }
}
using System;

namespace FlatNode.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProcedureGraphNodeAttribute : Attribute
    {
        public string nodeName;
        public string nodeMenuPath;
        public string nodeDescription;

        public ProcedureGraphNodeAttribute(string nodeName, string nodeMenuPath, string nodeDescription = "", bool hasFlowIn = true,bool isEntranceNode = false)
        {
            this.nodeName = nodeName;
            this.nodeMenuPath = nodeMenuPath;
            this.nodeDescription = nodeDescription;

        }
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class NodeFlowOutPortAttribute : Attribute
    {
        public int portId;
        public string portName;
        public string description;

        public NodeFlowOutPortAttribute(int portId, string portName, string description = "")
        {
            this.portId = portId;
            this.portName = portName;
            this.description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NodeInputPortAttribute : Attribute
    {
        public int priority;
        public string portName;
        public string description;

        public NodeInputPortAttribute(int priority, string portName,string description = "")
        {
            this.priority = priority;
            this.portName = portName;
            this.description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class NodeOutputPortAttribute : Attribute
    {
        public int portId;
        public Type outputType;
        public string portName;
        public string description;

        public NodeOutputPortAttribute(int portId,Type outputType,string portName, string description = "")
        {
            this.portId = portId;
            this.outputType = outputType;
            this.portName = portName;
            this.description = description;
        }
    }
}
using FlatNode.Runtime;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
    public class FlowOutPortEditorView : PortEditorView
    {
        public NodeFlowOutPortAttribute flowOutPortAttribute;

        public FlowOutPortEditorView(NodeEditorView nodeView, NodeFlowOutPortAttribute flowOutPortAttribute) :
            base(nodeView)
        {
            FlowType = FlowType.Out;
            this.flowOutPortAttribute = flowOutPortAttribute;
        }

        public override void Draw()
        {
            //流出连接箭头小标
            connectionCircleRect = new Rect(portViewRect.xMax + ConnectionCirclePadding,
                portViewRect.center.y - ConnectionCircleSize / 2f,
                ConnectionCircleSize,
                ConnectionCircleSize);

            if (IsConnected)
            {
                GUI.Box(connectionCircleRect, string.Empty, Utility.GetGuiStyle("PortArrowFilled"));
            }
            else
            {
                GUI.Box(connectionCircleRect, string.Empty, Utility.GetGuiStyle("PortArrow"));
            }

            var nameRect = new Rect(portViewRect);
            nameRect.width = nameRect.width - 5f;

            GUI.Label(nameRect,
                $"({flowOutPortAttribute.portId}){flowOutPortAttribute.portName}",
                Utility.GetGuiStyle("PortNameRight"));
        }

        public override float GetNameWidth()
        {
            float fontSize = Utility.GetGuiStyle("PortNameRight").fontSize;
            return Utility.GetStringGuiWidth(flowOutPortAttribute.portName, fontSize) + 30f; //30代表的是(n)前缀的长度
        }

        public override void DisconnectAllConnections()
        {
            if (connectedPortList.Count > 0)
            {
                foreach (var connectPort in connectedPortList)
                {
                    connectPort.connectedPortList.Remove(this);
                }
            }
            connectedPortList.Clear();
        }

        public override string PortDescription => flowOutPortAttribute.description;
    }
}
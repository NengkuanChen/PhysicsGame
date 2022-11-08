using System;
using System.Collections.Generic;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
    public class GraphEditorData
    {
        private const int MaxNodeLimit = 500;

        public Vector2 GraphOffset
        {
            get => graphOffset;
            set => graphOffset = value;
        }
        private Vector2 graphOffset;

        public float GraphZoom
        {
            get => graphZoom;
            set => graphZoom = value;
        }
        private float graphZoom;

        public const float MaxGraphZoom = 1f;
        public const float MinGraphZoom = .2f;
        public const float GraphZoomSpeed = 50f;

        public NodeEditorView entranceNode;

        public List<NodeEditorView> currentNodes;
        public List<NodeEditorView> selectedNodes; //选中的节点
        public List<ConnectionLineView> connectionLines;
        public List<CommentBoxView> commentBoxViews;

        public GraphEditorData()
        {
            currentNodes = new List<NodeEditorView>();
            connectionLines = new List<ConnectionLineView>();
            commentBoxViews = new List<CommentBoxView>();
            selectedNodes = new List<NodeEditorView>();

            Clear();
        }

        public int GetNewNodeId()
        {
            for (var i = 0; i < MaxNodeLimit; i++)
            {
                var hasAssigned = false;
                for (var j = 0; j < currentNodes.Count; j++)
                {
                    if (currentNodes[j].NodeId == i)
                    {
                        hasAssigned = true;
                        break;
                    }
                }

                if (!hasAssigned)
                {
                    return i;
                }
            }

            return 0;
        }

        public void DeleteNode(NodeEditorView nodeView)
        {
            if (nodeView == null)
            {
                return;
            }

            if (!currentNodes.Contains(nodeView))
            {
                return;
            }

            var allPortList = nodeView.allPortList;
            for (var i = 0; i < allPortList.Count; i++)
            {
                var portView = allPortList[i];
                ClearPortAllConnections(portView);
            }

            currentNodes.Remove(nodeView);
            if (currentNodes.Contains(nodeView))
            {
                currentNodes.Remove(nodeView);
            }
        }

        public void DeleteConnectionLine(ConnectionLineView connectionLineView)
        {
            if (connectionLineView == null)
            {
                return;
            }

            if (!connectionLines.Contains(connectionLineView))
            {
                return;
            }

            var flowOutPort = connectionLineView.FlowOutPortView;
            var flowInPort = connectionLineView.FlowInPortView;

            if (flowOutPort.connectedPortList.Contains(flowInPort))
            {
                flowOutPort.connectedPortList.Remove(flowInPort);
            }

            if (flowInPort.connectedPortList.Contains(flowOutPort))
            {
                flowInPort.connectedPortList.Remove(flowOutPort);
            }

            connectionLines.Remove(connectionLineView);

            if (connectionLines.Contains(connectionLineView))
            {
                connectionLines.Remove(connectionLineView);
            }
        }

        public void DeleteCommentBox(CommentBoxView commentBoxView)
        {
            if (commentBoxViews.Contains(commentBoxView))
            {
                commentBoxViews.Remove(commentBoxView);
            }
        }

        public void ClearPortAllConnections(PortEditorView portView)
        {
            if (portView == null)
            {
                return;
            }

            var connectedPortList = portView.connectedPortList;
            foreach (var connectedPort in connectedPortList)
            {
                if (connectedPort.connectedPortList.Contains(portView))
                {
                    connectedPort.connectedPortList.Remove(portView);
                    FindConnectionByPortsAndRemoveIt(portView, connectedPort);
                }
            }

            portView.connectedPortList.Clear();
        }

        public void FindConnectionByPortsAndRemoveIt(PortEditorView portA, PortEditorView portB)
        {
            if (portA == null || portB == null)
            {
                return;
            }

            if (portA.FlowType == portB.FlowType)
            {
                Debug.LogError("RemoveConnectionByPort err: 两个接口类型相同");
                return;
            }

            var flowInPort = portA.FlowType == FlowType.In ? portA : portB;
            var flowOutPort = portA.FlowType == FlowType.Out ? portA : portB;

            ConnectionLineView needRemoveConnectionLineView = null;

            for (var i = 0; i < connectionLines.Count; i++)
            {
                var connectionLineView = connectionLines[i];
                if (connectionLineView.FlowInPortView == flowInPort &&
                    connectionLineView.FlowOutPortView == flowOutPort)
                {
                    needRemoveConnectionLineView = connectionLineView;
                    break;
                }
            }

            if (needRemoveConnectionLineView != null)
            {
                if (connectionLines.Contains(needRemoveConnectionLineView))
                {
                    connectionLines.Remove(needRemoveConnectionLineView);
                }
            }
        }

        public NodeEditorView GetNode(int nodeId)
        {
            for (var i = 0; i < currentNodes.Count; i++)
            {
                if (currentNodes[i].NodeId == nodeId)
                {
                    return currentNodes[i];
                }
            }

            return null;
        }

        public void Clear()
        {
            // graphId = 0;
            // graphName = string.Empty;
            // graphDescription = string.Empty;

            currentNodes.Clear();
            connectionLines.Clear();
            commentBoxViews.Clear();

            graphOffset = Vector2.zero;
            graphZoom = 1f;

            ClearSelectedNode();
        }

        /// <summary>
        /// 检查是否设置了入口节点
        /// </summary>
        /// <returns></returns>
        public bool CheckHasEntranceNode()
        {
            return entranceNode != null;
        }

        /// <summary>
        /// 将节点放到队尾，这样在渲染的时候就可以让这个节点在最上面
        /// </summary>
        public void PutNodeToListTail(int targetNodeIndex)
        {
            if (targetNodeIndex < 0 || targetNodeIndex >= currentNodes.Count)
            {
                return;
            }

            if (targetNodeIndex == currentNodes.Count - 1)
            {
                return;
            }

            var tailNode = currentNodes[currentNodes.Count - 1];
            currentNodes[currentNodes.Count - 1] = currentNodes[targetNodeIndex];
            currentNodes[targetNodeIndex] = tailNode;
        }

        public void UpdateSelectedNode(Rect selectionRect)
        {
            ClearSelectedNode();

            foreach (var nodeView in currentNodes)
            {
                if (nodeView.IsContainInRect(selectionRect))
                {
                    nodeView.isSelected = true;
                    selectedNodes.Add(nodeView);
                }
            }

            //            Debug.Log("select node count: " + selectedNodeList.Count);
        }

        public void ClearSelectedNode()
        {
            foreach (var node in selectedNodes)
            {
                node.isSelected = false;
            }

            selectedNodes.Clear();
        }

        public void OnLoadFinish()
        {
            for (var i = 0; i < currentNodes.Count; i++)
            {
                currentNodes[i].OnLoadFinish();
            }
        }

        public void SetEntranceNode(NodeEditorView node)
        {
            if (entranceNode == node)
            {
                return;
            }

            if (entranceNode != null)
            {
                entranceNode.isEntranceNode = false;
            }

            node.isEntranceNode = true;
            entranceNode = node;
        }
    }
}
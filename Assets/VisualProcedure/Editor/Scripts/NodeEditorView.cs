using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
    /// <summary>
    /// 负责在编辑器界面绘制节点
    /// </summary>
    public class NodeEditorView
    {
        public GraphEditorWindow ownerGraph;
        public bool isEntranceNode;

        public Vector2 PositionInWindow { get; private set; }
        public Vector2 PositionInGraph { get; set; }
        public Rect viewRect;

        public int NodeId { get; set; }
        public NodeReflectionInfo ReflectionInfo { get; private set; }

        /// <summary>
        /// 流入接口
        /// </summary>
        public FlowInPortEditorView flowInPortView;
        /// <summary>
        /// 允许进入多个流程
        /// </summary>
        public FlowOutPortEditorView[] flowOutPortViews;

        private PortLayoutHelper leftPortLayoutHelper;
        private PortLayoutHelper rightPortLayoutHelper;

        public List<PortEditorView> allPortList;

        public bool isSelected;

        private const float PortAreaPadding = 20f;

    #region Rect Defines

        public Rect NodeNameRect => new Rect(viewRect.x, viewRect.y + 5, viewRect.width, 20f);

    #endregion

        public NodeEditorView(Vector2 graphPosition,
                              GraphEditorWindow ownerGraph,
                              int nodeId,
                              bool isEntranceNode,
                              NodeReflectionInfo reflectionInfo)
        {
            this.ownerGraph = ownerGraph;
            this.isEntranceNode = isEntranceNode;

            NodeId = nodeId;
            ReflectionInfo = reflectionInfo;

            PositionInGraph = graphPosition;
            PositionInWindow = ownerGraph.GraphPositionToWindowPosition(graphPosition);

            viewRect = new Rect(Vector2.zero, new Vector2(200, 400));

            allPortList = new List<PortEditorView>();

            leftPortLayoutHelper = new PortLayoutHelper();
            rightPortLayoutHelper = new PortLayoutHelper();

            flowInPortView = new FlowInPortEditorView(this) {portId = 0};
            allPortList.Add(flowInPortView);

            if (reflectionInfo.flowOutPortDefineAttributes.Length > 0)
            {
                flowOutPortViews = new FlowOutPortEditorView[reflectionInfo.flowOutPortDefineAttributes.Length];
                for (var i = 0; i < flowOutPortViews.Length; i++)
                {
                    flowOutPortViews[i] =
                        new FlowOutPortEditorView(this, reflectionInfo.flowOutPortDefineAttributes[i]) {portId = i};
                    allPortList.Add(flowOutPortViews[i]);
                }
            }
            else
            {
                flowOutPortViews = new FlowOutPortEditorView[0];
            }

            CalculateNodeSize();
        }

        private void CalculateNodeSize()
        {
            var nodeNameWidth = Utility.GetStringGuiWidth(ReflectionInfo.NodeName,
                Utility.GetGuiStyle("NodeName").fontSize) + 20;
            //width
            float leftMaxWidth = 0f, rightMaxWidth = 0f;
            var leftPortCount = 0;
            var rightPortCount = 0;
            for (var i = 0; i < allPortList.Count(); i++)
            {
                var portView = allPortList[i];
                var width = portView.GetNameWidth();

                if (portView.FlowType == FlowType.In)
                {
                    leftPortCount++;
                    if (width > leftMaxWidth)
                    {
                        leftMaxWidth = width;
                    }
                }
                else
                {
                    rightPortCount++;
                    if (width > rightMaxWidth)
                    {
                        rightMaxWidth = width;
                    }
                }
            }

            viewRect.width = Mathf.Max(leftMaxWidth + rightMaxWidth + PortAreaPadding, nodeNameWidth);
            viewRect.height = NodeNameRect.height + PortAreaPadding + Mathf.Max(
                PortLayoutHelper.CalculateHeightByPortCount(leftPortCount),
                PortLayoutHelper.CalculateHeightByPortCount(rightPortCount));

            leftPortLayoutHelper.SetOffset(0, leftMaxWidth);
            rightPortLayoutHelper.SetOffset(viewRect.width - rightMaxWidth, rightMaxWidth); //中间留点padding
        }

        /// <summary>
        ///流程图载入完成时
        /// </summary>
        public void OnLoadFinish()
        {
        }

        public void DrawNodeGUI()
        {
            if (ownerGraph == null)
            {
                return;
            }

            PositionInWindow = ownerGraph.GraphPositionToWindowPosition(PositionInGraph);
            viewRect.center = PositionInWindow;

            if (isSelected)
            {
                var highLightRect = new Rect(viewRect);
                highLightRect.position -= Vector2.one * 2f;
                highLightRect.max += Vector2.one * 4f;
                GUI.Box(highLightRect, "", Utility.GetGuiStyle("Highlight"));
            }

            if (isEntranceNode)
            {
                GUI.Box(viewRect, "", Utility.GetGuiStyle("EntranceNode"));
            }
            else
            {
                GUI.Box(viewRect, "", Utility.GetGuiStyle("NodeBg"));
            }

            //draw node name
            GUI.Label(NodeNameRect, $"{ReflectionInfo.NodeName}", Utility.GetGuiStyle("NodeName"));

            leftPortLayoutHelper.SetPosition(
                new Vector2(viewRect.x, viewRect.y + NodeNameRect.height + PortAreaPadding));
            rightPortLayoutHelper.SetPosition(new Vector2(viewRect.x,
                viewRect.y + NodeNameRect.height + PortAreaPadding));

            if (flowInPortView != null)
            {
                flowInPortView.portViewRect = leftPortLayoutHelper.GetRect();
            }

            if (flowOutPortViews.Length > 0)
            {
                for (var i = 0; i < flowOutPortViews.Length; i++)
                {
                    var flowOutPortView = flowOutPortViews[i];
                    flowOutPortView.portViewRect = rightPortLayoutHelper.GetRect();
                }
            }

            for (var i = 0; i < allPortList.Count; i++)
            {
                var portView = allPortList[i];

                portView.Draw();
            }
        }

        public void Drag(Vector2 delta)
        {
            PositionInGraph += delta;
        }

        public void OpenNodeScriptFile()
        {
            if (ReflectionInfo == null)
            {
                return;
            }

            var procedureNodeClassName = ReflectionInfo.Type.Name;
            var assetsGuids = AssetDatabase.FindAssets("t:script", new[] {"Assets/Scripts/Game"});
            foreach (var guid in assetsGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (string.Equals(fileName, procedureNodeClassName, StringComparison.InvariantCultureIgnoreCase))
                {
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath));
                    return;
                }
            }
        }
        // /// <summary>
        // /// 递归获取该节点创建sequence时需要包含的所有节点id
        // /// 遇到CreateSequenceNode则停止往右探索
        // /// </summary>
        // /// <returns></returns>
        // public void GetSequenceNodesIdsRecursive(ref List<int> rightSideNodeIdList)
        // {
        //     if (rightSideNodeIdList == null)
        //     {
        //         rightSideNodeIdList = new List<int>();
        //     }
        //
        //     if (flowOutPortViews != null)
        //     {
        //         for (var i = 0; i < flowOutPortViews.Length; i++)
        //         {
        //             var connectionPortList = flowOutPortViews[i].connectedPortList;
        //             for (var j = 0; j < connectionPortList.Count; j++)
        //             {
        //                 var targetNode = connectionPortList[j].NodeView;
        //                 var nodeId = targetNode.NodeId;
        //                 if (!rightSideNodeIdList.Contains(nodeId))
        //                 {
        //                     rightSideNodeIdList.Add(nodeId);
        //
        //                     if (!targetNode.ReflectionInfo.IsCreateSequenceNode)
        //                     {
        //                         targetNode.GetSequenceNodesIdsRecursive(ref rightSideNodeIdList);
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //
        //     for (var i = 0; i < outputPortViewList.Count; i++)
        //     {
        //         var outputPort = outputPortViewList[i];
        //         var connectionPortList = outputPort.connectedPortList;
        //         for (var j = 0; j < connectionPortList.Count; j++)
        //         {
        //             var targetNode = connectionPortList[j].NodeView;
        //             var nodeId = targetNode.NodeId;
        //             if (!rightSideNodeIdList.Contains(nodeId))
        //             {
        //                 rightSideNodeIdList.Add(nodeId);
        //
        //                 if (!targetNode.ReflectionInfo.IsCreateSequenceNode)
        //                 {
        //                     targetNode.GetSequenceNodesIdsRecursive(ref rightSideNodeIdList);
        //                 }
        //             }
        //         }
        //     }
        // }

        // /// <summary>
        // /// 检查一个节点是否是通用节点。
        // /// 通用节点是指该节点不会从技能入口流入到或者取值不会从能够从技能入口流入到的节点取值。
        // /// 递归所有左Flow In节点和Input节点，如果能够访问到入口节点，则该节点不是通用节点。
        // /// </summary>
        // /// <returns></returns>
        // public bool CheckNodeIsCommonNode(HashSet<NodeEditorView> checkedNodeSet = null)
        // {
        //     if (ReflectionInfo.Type.IsSubclassOf(typeof(EntranceProcedureNodeBase)))
        //     {
        //         return false;
        //     }
        //
        //     if (checkedNodeSet == null)
        //     {
        //         checkedNodeSet = new HashSet<NodeEditorView>();
        //     }
        //
        //     if (flowInPortView != null && flowInPortView.connectedPortList.Count > 0)
        //     {
        //         var connectionPortList = flowInPortView.connectedPortList;
        //         for (var i = 0; i < connectionPortList.Count; i++)
        //         {
        //             var nodeView = connectionPortList[i].NodeView;
        //             if (checkedNodeSet.Contains(nodeView))
        //             {
        //                 continue;
        //             }
        //
        //             if (nodeView.ReflectionInfo.Type.IsSubclassOf(typeof(EntranceProcedureNodeBase)))
        //             {
        //                 return false;
        //             }
        //
        //             checkedNodeSet.Add(nodeView);
        //             if (!nodeView.CheckNodeIsCommonNode(checkedNodeSet))
        //             {
        //                 return false;
        //             }
        //         }
        //     }
        //
        //     if (inputPortViewList.Count > 0)
        //     {
        //         for (var i = 0; i < inputPortViewList.Count; i++)
        //         {
        //             var connectionPortList = inputPortViewList[i].connectedPortList;
        //             for (var j = 0; j < connectionPortList.Count; j++)
        //             {
        //                 var nodeView = connectionPortList[j].NodeView;
        //                 if (checkedNodeSet.Contains(nodeView))
        //                 {
        //                     continue;
        //                 }
        //
        //                 if (nodeView.ReflectionInfo.Type.IsSubclassOf(typeof(EntranceProcedureNodeBase)))
        //                 {
        //                     return false;
        //                 }
        //
        //                 checkedNodeSet.Add(nodeView);
        //                 if (!nodeView.CheckNodeIsCommonNode(checkedNodeSet))
        //                 {
        //                     return false;
        //                 }
        //             }
        //         }
        //     }
        //
        //     return true;
        // }

        public bool IsContainInRect(Rect rect)
        {
            return rect.Contains(viewRect.position, true) && rect.Contains(viewRect.max, true);
        }

        // /// <summary>
        // /// 检查input port和output port连线的合法性
        // /// </summary>
        // public void CheckIOPortConnectionValidate()
        // {
        //     for (var i = 0; i < inputPortViewList.Count; i++)
        //     {
        //         var inputPortView = inputPortViewList[i];
        //         if (inputPortView.connectedPortList.Count == 1)
        //         {
        //             if (!ConnectionLineView.CheckPortsCanLine(inputPortView, inputPortView.connectedPortList[0]))
        //             {
        //                 ownerGraph.data.FindConnectionByPortsAndRemoveIt(inputPortView,
        //                     inputPortView.connectedPortList[0]);
        //             }
        //         }
        //     }
        //
        //     for (var i = 0; i < outputPortViewList.Count; i++)
        //     {
        //         var outputPortView = outputPortViewList[i];
        //         for (var j = 0; j < outputPortView.connectedPortList.Count; j++)
        //         {
        //             var connectedInputPortView = outputPortView.connectedPortList[j] as InputPortEditorView;
        //             if (connectedInputPortView == null)
        //             {
        //                 continue;
        //             }
        //
        //             if (!ConnectionLineView.CheckPortsCanLine(outputPortView, connectedInputPortView))
        //             {
        //                 ownerGraph.data.FindConnectionByPortsAndRemoveIt(outputPortView, connectedInputPortView);
        //             }
        //         }
        //     }
        // }

        // /// <summary>
        // /// 当节点对应的是<see cref="FlatNode.Runtime.GetVariableNode"/> 或者 <see cref="FlatNode.Runtime.SetVariableNode"/> 时
        // /// 他们接口的类型需要显示为对应类型。
        // /// 还要重新计算节点Rect的大小
        // /// </summary>
        // /// <param name="variableType"></param>
        // /// <param name="needRecheckConnection"></param>
        // public void UpdateGraphVariableNodeIOPortType(Type variableType, bool needRecheckConnection)
        // {
        //     if (ReflectionInfo.Type == typeof(GetVariableNode))
        //     {
        //         //第一个input port是选择要读取哪一个变量
        //         var inputPortView = inputPortViewList[0];
        //         inputPortView.overridePortType = variableType;
        //
        //         //第一个output port是要输出选择的变量
        //         var outputPortView = outputPortViewList[0];
        //         outputPortView.overridePortType = variableType;
        //
        //         CalculateNodeSize();
        //
        //         if (needRecheckConnection)
        //         {
        //             CheckIOPortConnectionValidate();
        //         }
        //     }
        //     else if (ReflectionInfo.Type == typeof(SetVariableNode))
        //     {
        //         //第一个input port是选择要存储到哪个变量
        //         var setVariableInputPort = inputPortViewList[0];
        //         setVariableInputPort.overridePortType = variableType;
        //
        //         //第二个input port是接收要存储的值
        //         var valueVariableInputPort = inputPortViewList[1];
        //         valueVariableInputPort.overridePortType = variableType;
        //
        //         //该节点没有output port
        //
        //         CalculateNodeSize();
        //
        //         if (needRecheckConnection)
        //         {
        //             CheckIOPortConnectionValidate();
        //         }
        //     }
        // }

        // /// <summary>
        // /// 当编辑了变量列表后，需要刷新一下所有接口，看是否还允许连接
        // /// </summary>
        // public void OnGraphVariableListChange()
        // {
        //     for (var i = 0; i < inputPortViewList.Count; i++)
        //     {
        //         inputPortViewList[i].OnGraphVariableListChange();
        //     }
        // }
    }
}
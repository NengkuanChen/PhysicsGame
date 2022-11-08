using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
    /// <summary>
    /// 持久化类，负责图的编辑器存储/加载
    /// </summary>
    public static class PersistenceTool
    {
    #region 存储

        private static readonly string graphEditorDataSaveFilePath =
            $"{Application.dataPath}/VisualProcedure/Editor/ProcedureGraphSave.json";

        public static void SaveGraph(GraphEditorData data)
        {
            if (data.entranceNode == null)
            {
                throw new Exception("当前图没有入口");
            }

            var jsonString = String.Empty;

            var isSuccess = true;
            try
            {
                //存储技能配置json文件,配置文件使用json是因为可读性好
                var graphConfigInfo = new GraphConfigInfo
                {
                    entranceNodeId = data.entranceNode.NodeId,
                    nodesList = new List<NodeConfigInfo>(),
                    commentBoxInfoList = new List<CommentBoxInfo>(),
                };

                foreach (var nodeView in data.currentNodes)
                {
                    graphConfigInfo.nodesList.Add(ConvertToNodeInfo(nodeView));
                }

                foreach (var commentBoxView in data.commentBoxViews)
                {
                    graphConfigInfo.commentBoxInfoList.Add(ConvertToCommentInfo(commentBoxView));
                }

                jsonString = JsonUtility.ToJson(graphConfigInfo, true);
            }
            catch (Exception e)
            {
                isSuccess = false;
                Debug.LogError(e.Message);
                throw;
            }
            finally
            {
                if (isSuccess)
                {
                    var directoryName = Path.GetDirectoryName(graphEditorDataSaveFilePath);
                    if (directoryName != null)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    File.WriteAllText(graphEditorDataSaveFilePath, jsonString);
                    AssetDatabase.Refresh();
                }
            }
        }

        private static CommentBoxInfo ConvertToCommentInfo(CommentBoxView commentBoxView)
        {
            var commentBoxInfo = new CommentBoxInfo
            {
                comment = commentBoxView.comment,
                positionInGraph = commentBoxView.rectInGraph.position,
                size = commentBoxView.rectInGraph.size
            };
            return commentBoxInfo;
        }

        private static NodeConfigInfo ConvertToNodeInfo(NodeEditorView nodeView)
        {
            var nodeConfigInfo = new NodeConfigInfo();
            nodeConfigInfo.nodeId = nodeView.NodeId;
            nodeConfigInfo.positionInGraph = nodeView.PositionInGraph;
            nodeConfigInfo.nodeClassTypeName = nodeView.ReflectionInfo.Type.FullName;

            nodeConfigInfo.flowOutPortInfoList = new List<FlowOutPortConfigInfo>();
            // nodeConfigInfo.inputPortInfoList = new List<InputPortConfigInfo>();

            //flow out info
            for (var i = 0; i < nodeView.flowOutPortViews.Length; i++)
            {
                var flowOutPort = nodeView.flowOutPortViews[i];
                nodeConfigInfo.flowOutPortInfoList.Add(ConvertToFlowOutPortInfo(flowOutPort));
            }

            return nodeConfigInfo;
        }

        private static FlowOutPortConfigInfo ConvertToFlowOutPortInfo(FlowOutPortEditorView flowOutPort)
        {
            var flowOutPortConfigInfo = new FlowOutPortConfigInfo();
            flowOutPortConfigInfo.flowOutPortId = flowOutPort.flowOutPortAttribute.portId;

            flowOutPortConfigInfo.targetNodeList = new List<int>();
            for (var i = 0; i < flowOutPort.connectedPortList.Count; i++)
            {
                var connectedNodeId = flowOutPort.connectedPortList[i].NodeView.NodeId;
                flowOutPortConfigInfo.targetNodeList.Add(connectedNodeId);
            }

            return flowOutPortConfigInfo;
        }

    #endregion

    #region 载入

        public static GraphEditorData LoadGraph(GraphEditorWindow graph)
        {
            var resultData = new GraphEditorData();

            if (!File.Exists(graphEditorDataSaveFilePath))
            {
                return resultData;
            }

            var jsonString = File.ReadAllText(graphEditorDataSaveFilePath);
            var graphConfigInfo = new GraphConfigInfo();
            EditorJsonUtility.FromJsonOverwrite(jsonString, graphConfigInfo);

            //处理注释框
            for (var i = 0; i < graphConfigInfo.commentBoxInfoList.Count; i++)
            {
                var commentBoxInfo = graphConfigInfo.commentBoxInfoList[i];
                var commentBoxView = ParseCommentBoxInfo(commentBoxInfo, graph);

                resultData.commentBoxViews.Add(commentBoxView);
            }

            //如果有节点无法解析出来(可能是改了类名称之类的)，则需要跳过这些节点
            var errorNodeIndexSet = new HashSet<int>();
            //首先将所有的节点都生成
            for (var i = 0; i < graphConfigInfo.nodesList.Count; i++)
            {
                var nodeView = ParseNodeInfo(graphConfigInfo.nodesList[i], graph);
                if (nodeView == null)
                {
                    errorNodeIndexSet.Add(i);
                    continue;
                }

                resultData.currentNodes.Add(nodeView);
            }

            //然后再将所有节点的内容写进去，将节点连起来
            var nodeIndex = 0;
            for (var i = 0; i < graphConfigInfo.nodesList.Count; i++)
            {
                if (errorNodeIndexSet.Contains(i))
                {
                    //skip
                    continue;
                }

                UpdateNodeViewData(graphConfigInfo.nodesList[i], resultData.currentNodes[nodeIndex], resultData);
                nodeIndex++;
            }

            //确定入口节点
            var entranceNodeId = graphConfigInfo.entranceNodeId;
            foreach (var node in resultData.currentNodes)
            {
                if (node.NodeId == entranceNodeId)
                {
                    node.isEntranceNode = true;
                    resultData.entranceNode = node;
                    break;
                }
            }

            return resultData;
        }
   
        private static CommentBoxView ParseCommentBoxInfo(CommentBoxInfo commentBoxInfo, GraphEditorWindow graph)
        {
            var startPositionInGraph = commentBoxInfo.positionInGraph;
            var boxSize = commentBoxInfo.size;
            var endPositionInGraph =
                new Vector2(startPositionInGraph.x + boxSize.x, startPositionInGraph.y + boxSize.y);

            var commentBoxView =
                new CommentBoxView(graph, startPositionInGraph, endPositionInGraph, commentBoxInfo.comment);
            return commentBoxView;
        }

        private static NodeEditorView ParseNodeInfo(NodeConfigInfo nodeConfigInfo, GraphEditorWindow graph)
        {
            var nodeTypeName = nodeConfigInfo.nodeClassTypeName;
            var nodeType = Type.GetType(nodeTypeName + ",Assembly-CSharp");
            if (nodeType == null)
            {
                Debug.LogErrorFormat("无法载入类型{0} ,该节点被跳过", nodeTypeName);
                return null;
            }

            var reflectionInfo = new NodeReflectionInfo(nodeType);
            var nodeView = new NodeEditorView(nodeConfigInfo.positionInGraph,
                graph,
                nodeConfigInfo.nodeId,
                false, //这里暂时都设置为false
                reflectionInfo);

            return nodeView;
        }

        private static void UpdateNodeViewData(NodeConfigInfo nodeConfigInfo,
                                               NodeEditorView nodeView,
                                               GraphEditorData data)
        {
            //flow in port--处理流出节点的时候顺便就处理了

            //flow out port
            for (var i = 0; i < nodeConfigInfo.flowOutPortInfoList.Count; i++)
            {
                var flowOutPortConfigInfo = nodeConfigInfo.flowOutPortInfoList[i];
                var flowOutPortView =
                    GetFlowOutPortViewByPortId(nodeView.flowOutPortViews, flowOutPortConfigInfo.flowOutPortId);
                if (flowOutPortView == null)
                {
                    Debug.Log(
                        $"节点{nodeView.ReflectionInfo.Type}中找不到流出端口 <{flowOutPortConfigInfo.flowOutPortId}> 了,该端口的连接被忽略");
                    continue;
                }

                foreach (var targetNodeId in flowOutPortConfigInfo.targetNodeList)
                {
                    var targetNodeView = data.GetNode(targetNodeId);
                    if (targetNodeView == null)
                    {
                        Debug.LogError($"无法找到节点{targetNodeId}，可能是配置损坏/更改了类名...");
                        continue;
                    }

                    if (targetNodeView.flowInPortView == null)
                    {
                        Debug.LogError($"节点类型{nodeView.ReflectionInfo.Type.FullName}没有流入节点，是否节点性质发生了改变?");
                        continue;
                    }

                    var connectionLineView =
                        new ConnectionLineView(flowOutPortView, targetNodeView.flowInPortView, data);
                    data.connectionLines.Add(connectionLineView);
                }
            }
        }

        private static FlowOutPortEditorView GetFlowOutPortViewByPortId(
            FlowOutPortEditorView[] flowOutPortEditorViews,
            int portId)
        {
            if (flowOutPortEditorViews == null)
            {
                return null;
            }

            for (var i = 0; i < flowOutPortEditorViews.Length; i++)
            {
                if (flowOutPortEditorViews[i].flowOutPortAttribute.portId == portId)
                {
                    return flowOutPortEditorViews[i];
                }
            }

            return null;
        }

    #endregion
    }

    [Serializable]
    public class GraphConfigInfo
    {
        public int entranceNodeId;
        public List<NodeConfigInfo> nodesList;
        public List<CommentBoxInfo> commentBoxInfoList;
    }

    [Serializable]
    public class NodeConfigInfo
    {
        public int nodeId;
        public Vector2 positionInGraph;
        public string nodeClassTypeName;

        public List<FlowOutPortConfigInfo> flowOutPortInfoList;
    }

    [Serializable]
    public class FlowOutPortConfigInfo
    {
        public int flowOutPortId;
        public List<int> targetNodeList;
    }

    [Serializable]
    public class CommentBoxInfo
    {
        public Vector2 positionInGraph;
        public Vector2 size;
        public string comment;
    }
}
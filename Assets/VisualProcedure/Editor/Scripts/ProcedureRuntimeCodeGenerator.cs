using System.IO;
using UnityEditor;
using UnityEngine;
using VisualProcedure.Runtime.ProcedureNode;

namespace VisualProcedure.Editor.Scripts
{
    public static class ProcedureRuntimeCodeGenerator
    {
        private static readonly string codeFilePath =
            $"{Application.dataPath}/VisualProcedure/Runtime/ProcedureFlowMap.cs";

        public static void GenerateCode(GraphEditorData graphData)
        {
            var codeWriter = new CodeWriter();

            codeWriter.WriteLine("using System;");
            codeWriter.WriteLine("using System.Collections.Generic;");
            codeWriter.WriteLine("");
            codeWriter.WriteLine("namespace VisualProcedure.Runtime");
            codeWriter.BeginBlock();
            codeWriter.WriteLine("public static class ProcedureFlowMap");
            codeWriter.BeginBlock();
            var procedureNodeBaseTypeName = typeof(ProcedureNodeBase).FullName;
            codeWriter.WriteLine(
                $"private static Dictionary<(Type, int), Func<int, {procedureNodeBaseTypeName}>> flowMap = new Dictionary<(Type, int), Func<int, {procedureNodeBaseTypeName}>>(new FlowMapKeyComparer())");
            codeWriter.BeginBlock();
            foreach (var node in graphData.currentNodes)
            {
                if (!node.isEntranceNode && node.flowInPortView.connectedPortList.Count == 0)
                {
                    continue;
                }

                codeWriter.BeginBlock();

                var nodeReflectionInfo = node.ReflectionInfo;
                codeWriter.WriteLine(
                    $"(typeof({nodeReflectionInfo.Type.FullName}), {node.NodeId}), portId =>");
                codeWriter.BeginBlock();
                codeWriter.WriteLine("return portId switch");
                codeWriter.BeginBlock();
                foreach (var flowOutPortView in node.flowOutPortViews)
                {
                    if (flowOutPortView.connectedPortList.Count == 0)
                    {
                        //没有连接任何节点
                        continue;
                    }

                    var connectedPort = flowOutPortView.connectedPortList[0];
                    codeWriter.WriteLine(
                        $"{flowOutPortView.portId} => new {connectedPort.NodeView.ReflectionInfo.Type.FullName}(){{ ID = {connectedPort.NodeView.NodeId} }},");
                }

                codeWriter.WriteLine("_ => null");

                codeWriter.EndBlock(";");
                codeWriter.EndBlock();
                codeWriter.EndBlock(",");
            }

            codeWriter.EndBlock(";");
            codeWriter.WriteLine(@$"public static Dictionary<(Type, int), Func<int, {procedureNodeBaseTypeName
            }>> FlowMap => flowMap;");

            codeWriter.WriteLine($"public static {procedureNodeBaseTypeName} GetEntranceNode()");
            codeWriter.BeginBlock();
            codeWriter.WriteLine(
                $"return new {graphData.entranceNode.ReflectionInfo.Type.FullName}() {{ ID = {graphData.entranceNode.NodeId} }}; ");
            codeWriter.EndBlock();

            codeWriter.EndBlock();
            codeWriter.EndBlock();

            File.WriteAllText(codeFilePath, codeWriter.ToString());
            AssetDatabase.Refresh();
        }
    }
}
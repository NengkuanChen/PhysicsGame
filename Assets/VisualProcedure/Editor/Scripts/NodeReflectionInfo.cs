using System;
using System.Collections.Generic;
using System.Reflection;
using FlatNode.Runtime;
using UnityEngine;
using VisualProcedure.Runtime.ProcedureNode;

namespace VisualProcedure.Editor.Scripts
{
    public class NodeReflectionInfo
    {
        public Type Type { get; private set; }
        private object instance;

        public ProcedureGraphNodeAttribute NodeAttribute { get; private set; }

        // private FieldInfo nodeIdFieldInfo;

        public NodeFlowOutPortAttribute[] flowOutPortDefineAttributes;
        // private FieldInfo flowOutTargetNodeIdFieldInfo;

        // public List<InputPortReflectionInfo> inputPortInfoList;
        // public List<OutputPortReflectionInfo> outputPortInfoList;
        //
        // private FieldInfo outputPortFuncsFieldInfo;
        // private Func<NodeVariable>[] outputPortFuncs;

        public NodeReflectionInfo(Type type)
        {
            Type = type;

            if (!Type.IsSubclassOf(typeof(ProcedureNodeBase)))
            {
                Debug.LogErrorFormat("{0} 没有继承自NodeBase类", type.Name);
                return;
            }

            //GraphNodeAttribute
            var attributeObjects = type.GetCustomAttributes(typeof(ProcedureGraphNodeAttribute), false);
            if (attributeObjects.Length == 0)
            {
                Debug.LogErrorFormat("class {0} 不包含GraphNodeAttribute", type.Name);
                return;
            }

            NodeAttribute = attributeObjects[0] as ProcedureGraphNodeAttribute;

            // nodeIdFieldInfo = type.GetField("nodeId",
            //     BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // inputPortInfoList = new List<InputPortReflectionInfo>();
            // outputPortInfoList = new List<OutputPortReflectionInfo>();

            //create instance
            instance = Activator.CreateInstance(type);

            //flowin port -- not need

            //flowout port reflection infos
            InitFlowOutPortReflectionInfos();

            // //Input Port ReflectionInfos
            // InitInputPortReflectionInfos();
            //
            // //Output Port ReflectionInfos
            // InitOutputPortReflectionInfos();
        }

        public object GetClassInstance()
        {
            return instance;
        }

        private void InitFlowOutPortReflectionInfos()
        {
            //FlowOutPortReflectionInfos
            var attributeObjects = Type.GetCustomAttributes(typeof(NodeFlowOutPortAttribute), false);
            if (attributeObjects.Length > 0)
            {
                flowOutPortDefineAttributes = new NodeFlowOutPortAttribute[attributeObjects.Length];
                for (var i = 0; i < flowOutPortDefineAttributes.Length; i++)
                {
                    flowOutPortDefineAttributes[i] = attributeObjects[i] as NodeFlowOutPortAttribute;
                }
            }

            if (flowOutPortDefineAttributes == null)
            {
                flowOutPortDefineAttributes = new NodeFlowOutPortAttribute[0];
            }

            Array.Sort(flowOutPortDefineAttributes, (a, b) => a.portId - b.portId);
            //检查Port id是否是从0开始连续排列的,
            for (var i = 0; i < flowOutPortDefineAttributes.Length; i++)
            {
                if (flowOutPortDefineAttributes[i].portId == i)
                {
                    continue;
                }

                Debug.LogError($"类型 {Type.Name} 上使用的{nameof(flowOutPortDefineAttributes)}的portId没有从0开始连续，请检查");
                return;
            }

            // flowOutTargetNodeIdFieldInfo = Type.GetField("flowOutTargetNodeId",
            //     BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            // if (flowOutTargetNodeIdFieldInfo == null)
            // {
            //     throw new Exception($"类型 {Type.Name} 不包含flowOutTargetNodeId变量，请检查");
            // }
        }
        //
        // private void InitInputPortReflectionInfos()
        // {
        //     var fieldInfos = Type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        //     for (var i = 0; i < fieldInfos.Length; i++)
        //     {
        //         var fieldInfo = fieldInfos[i];
        //
        //         var attributeObjects = fieldInfo.GetCustomAttributes(typeof(NodeInputPortAttribute), false);
        //         if (attributeObjects.Length > 0)
        //         {
        //             var inputPortReflectionInfo =
        //                 new InputPortReflectionInfo(attributeObjects[0] as NodeInputPortAttribute, fieldInfo, instance);
        //             inputPortInfoList.Add(inputPortReflectionInfo);
        //         }
        //     }
        //
        //     inputPortInfoList.Sort((a, b) => a.inputPortAttribute.priority - b.inputPortAttribute.priority);
        // }
        //
        // private void InitOutputPortReflectionInfos()
        // {
        //     var methodInfos = Type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        //     for (var i = 0; i < methodInfos.Length; i++)
        //     {
        //         var methodInfo = methodInfos[i];
        //
        //         var attributeObjects = methodInfo.GetCustomAttributes(typeof(NodeOutputPortAttribute), false);
        //         if (attributeObjects.Length > 0)
        //         {
        //             var outputPortReflectionInfo =
        //                 new OutputPortReflectionInfo(attributeObjects[0] as NodeOutputPortAttribute,
        //                     methodInfo,
        //                     instance);
        //             outputPortInfoList.Add(outputPortReflectionInfo);
        //         }
        //     }
        //
        //     outputPortInfoList.Sort((a, b) => a.outputPortAttribute.portId - b.outputPortAttribute.portId);
        //     for (var i = 0; i < outputPortInfoList.Count; i++)
        //     {
        //         if (outputPortInfoList[i].outputPortAttribute.portId == i) continue;
        //         Debug.LogErrorFormat("类型 {0} 上使用的NodeOutputPortAttribute的portId没有从0开始连续，请检查", Type.Name);
        //         return;
        //     }
        // }

    #region Public Properties

        public string NodeName
        {
            get
            {
                if (NodeAttribute == null)
                {
                    return "错误节点";
                }

                return NodeAttribute.nodeName;
            }
        }

        // /// <summary>
        // /// 是否允许有流程进入入口（也就是允许有父节点）
        // /// </summary>
        // public bool HasFlowInPort
        // {
        //     get
        //     {
        //         if (NodeAttribute == null)
        //         {
        //             return false;
        //         }
        //
        //         return NodeAttribute.hasFlowIn && !NodeAttribute.isEntranceNode;
        //     }
        // }
        //
        // public bool IsEntranceNode
        // {
        //     get
        //     {
        //         if (NodeAttribute == null)
        //         {
        //             return false;
        //         }
        //
        //         return NodeAttribute.isEntranceNode;
        //     }
        // }
        //
        // public bool IsCreateSequenceNode => Type == typeof(CreateSequenceProcedureNode);

    #endregion
    }
}
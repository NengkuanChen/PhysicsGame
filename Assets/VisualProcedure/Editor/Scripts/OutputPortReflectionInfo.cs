// using System;
// using System.Reflection;
// using FlatNode.Runtime;
//
// namespace VisualProcedure.Editor.Scripts
// {
//     public class OutputPortReflectionInfo
//     {
//         public NodeOutputPortAttribute outputPortAttribute;
//         public MethodInfo methodInfo;
//         public object instance;
//
//         public OutputPortReflectionInfo(NodeOutputPortAttribute outputPortAttribute, MethodInfo methodInfo, object instance)
//         {
//             this.outputPortAttribute = outputPortAttribute;
//             this.methodInfo = methodInfo;
//             this.instance = instance;
//         }
//
//         public OutputPortReflectionInfo(MethodInfo methodInfo)
//         {
//             this.methodInfo = methodInfo;
//
//             var attributeObjects = methodInfo.GetCustomAttributes(typeof(NodeOutputPortAttribute), false);
//             if (attributeObjects.Length > 0)
//             {
//                 this.outputPortAttribute = attributeObjects[0] as NodeOutputPortAttribute;
//             }
//         }
//
//         public Func<NodeVariable> CreateMethodDelegate()
//         {
//             Func<NodeVariable> resultDelegate =
//                 (Func<NodeVariable>) Delegate.CreateDelegate(typeof(Func<NodeVariable>), instance, methodInfo, false);
//             return resultDelegate;
//         }
//
//         public string PortName => outputPortAttribute.portName;
//
//         public Type OutputType => outputPortAttribute.outputType;
//     }
// }
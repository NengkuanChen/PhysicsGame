using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FlatNode.Runtime;
using UnityEditor;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
    public static class Utility
    {
        private const string GuiSkinPath = "Assets/VisualProcedure/Editor/Skin/SkillEditorSkin.guiskin";
        private static GUISkin loadedGuiSkin;
        public static GUISkin LoadedGuiSkin => loadedGuiSkin;

        private const string CommentBoxTexturePath = "Assets/VisualProcedure/Editor/Skin/commentbox.png";
        private static Texture2D commentBoxTexture;

        public static void LoadGuiSkin()
        {
            if (loadedGuiSkin == null)
            {
                loadedGuiSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(GuiSkinPath);
            }
        }

        public static GUIStyle GetGuiStyle(string styleName)
        {
            LoadGuiSkin();

            if (loadedGuiSkin == null)
            {
                Debug.LogError($"无法载入guistyle {GuiSkinPath}");
                return GUIStyle.none;
            }

            return loadedGuiSkin.GetStyle(styleName);
        }

        public static Texture2D GetCommentBoxTexture()
        {
            if (commentBoxTexture == null)
            {
                commentBoxTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(CommentBoxTexturePath);
            }

            return commentBoxTexture;
        }

        public static GUIStyle SearchBoxStyle = "ToolbarSeachTextField";
        public static GUIStyle CancelButtonStyle = "ToolbarSeachCancelButton";
        public static GUIStyle DisabledCancelButtonStyle = "ToolbarSeachCancelButtonEmpty";
        public static GUIStyle SelectionStyle = "SelectionRect";

        private static GUIStyle navTitleStyle;

        public static GUIStyle NavTitleStyle
        {
            get
            {
                if (navTitleStyle == null)
                {
                    navTitleStyle = new GUIStyle(EditorStyles.whiteBoldLabel);
                    navTitleStyle.alignment = TextAnchor.MiddleCenter;
                    navTitleStyle.normal.textColor = Color.white;
                }

                return navTitleStyle;
            }
        }

        private static Texture2D lineTexture;

        public static Texture2D GetBezierLineTexture()
        {
            if (lineTexture == null)
            {
                lineTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/FlatNode/Editor/Skin/Line");
            }

            return lineTexture;
        }

        public static float GetStringGuiWidth(string targetString, float singleChineseWidth = 10)
        {
            float result = 0;
            for (var i = 0; i < targetString.Length; i++)
            {
                var c = targetString[i];
                if (c >= 0x4e00 && c <= 0x9fbb)
                {
                    result += singleChineseWidth;
                }
                else
                {
                    result += singleChineseWidth * .5f;
                }
            }

            return result;
        }

        /// <summary>
        /// 这个版本更准确些
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="guiStyle"></param>
        /// <returns></returns>
        public static float GetStringGuiWidth(string targetString, GUIStyle guiStyle)
        {
            return guiStyle.CalcSize(new GUIContent(targetString)).x;
        }

        public static float GetEnumMaxGuiWidth(Type enumType, float singleChineseWidth = 10)
        {
            var enumStrings = Enum.GetNames(enumType);

            var maxLength = enumStrings.Max(x => x.Length);
            var maxLengthString = enumStrings.First(x => x.Length == maxLength);

            return GetStringGuiWidth(maxLengthString, singleChineseWidth);
        }

        public static string BeautifyTypeName(Type type, bool defaultFullName = false)
        {
            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(float))
            {
                return "float";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            if (type.IsEnum && defaultFullName)
            {
                var enumTypeName = type.FullName;
                enumTypeName = enumTypeName.Replace('+', '.');
                return enumTypeName;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericType = type.GetGenericArguments()[0];
                return string.Format("List<{0}>", BeautifyTypeName(genericType, defaultFullName));
            }

            return defaultFullName ? type.FullName : type.Name;
        }

    #region Reflection

        /// <summary>
        /// 获取工程中节点反射信息
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetNodeTypeList()
        {
            var resultList = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (var assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
            {
                var assemblyName = assemblies[assemblyIndex].GetName().Name;
                var assembly = assemblies[assemblyIndex];

                if (!assemblyName.Equals("Assembly-CSharp"))
                {
                    continue;
                }

                var types = assembly.GetTypes();
                for (var typeIndex = 0; typeIndex < types.Length; typeIndex++)
                {
                    var targetType = types[typeIndex];
                    var attributes = targetType.GetCustomAttributes(typeof(ProcedureGraphNodeAttribute), false);
                    if (attributes.Length > 0)
                    {
                        resultList.Add(targetType);
                    }
                }

                break;
            }

            return resultList;
        }

    #endregion

        private static EditorLayerInfo[] editorLayerInfos;

    #region LayerMaskHelper

        /// <summary>
        /// 缓存当前工程layer名字
        /// </summary>
        private static string[] unityLayerNames;

        /// <summary>
        /// 获取Unity Layer名称
        /// </summary>
        /// <returns></returns>
        public static string[] GetUnityLayerNames()
        {
            if (unityLayerNames == null)
            {
                var layerNameList = new List<string>();
                for (var i = 0; i < 32; i++)
                {
                    var layerName = LayerMask.LayerToName(i);
                    if (string.IsNullOrEmpty(layerName))
                    {
                        continue;
                    }

                    layerNameList.Add(layerName);
                }

                unityLayerNames = layerNameList.ToArray();
            }

            return unityLayerNames;
//            
//            if (gameLayerNames == null)
//            {
//                Type gameLayerType = typeof(GameLayer);
//                FieldInfo[] fieldInfos = gameLayerType.GetFields(BindingFlags.Static | BindingFlags.Public);
//
//                gameLayerNames = new string[fieldInfos.Length];
//                for (int i = 0; i < fieldInfos.Length; i++)
//                {
//                    FieldInfo fieldInfo = fieldInfos[i];
//
//                    string fieldName = fieldInfo.Name;
//                    gameLayerNames[i] = fieldName;
//                }
//            }
//
//            return gameLayerNames;
        }

        public static int GetLayerMaxGuiLength(int singleCharWidth = 5)
        {
            var layerNames = GetUnityLayerNames();

            var maxLength = 0;
            for (var i = 0; i < layerNames.Length; i++)
            {
                var layerName = layerNames[i];
                var length = layerName.Length;

                if (length > maxLength)
                {
                    maxLength = length;
                }
            }

            return maxLength * singleCharWidth;
        }

    #endregion
    }

    public enum ControlType
    {
        None,
        DraggingNode,
        DraggingGraph,
        DraggingConnection,
        DraggingMultiSelection,
        DraggingMultiNodes,
        DraggingNewCommentBox,
        DraggingCommentBox,
        EditingComment,
        ResizingCommentBox,
    }

    public class TestClass
    {
        private List<GameObject> testField;
    }

    public class EditorLayerInfo
    {
        public string layerName;
        public int layerValue;

        public override string ToString()
        {
            return string.Format("Name: {0}, Value: {1}", layerName, layerValue);
        }
    }
}
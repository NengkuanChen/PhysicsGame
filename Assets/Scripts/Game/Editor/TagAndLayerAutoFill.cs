using System.Reflection;
using Game.Utility;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class TagAndLayerAutoFill
    {
        [MenuItem("Tools/自动配置GameLayer和GameTag")]
        private static void AutoFillGameLayersAndTag()
        {
            var tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerArraySerializedProperty = tagManager.FindProperty("layers");

            //clear
            for (var i = 6; i < 31; i++)
            {
                var layerSerializedProperty = layerArraySerializedProperty.GetArrayElementAtIndex(i);
                layerSerializedProperty.stringValue = string.Empty;
            }

            var gameLayerType = typeof(GameLayer);
            var fieldInfos = gameLayerType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var fieldInfo in fieldInfos)
            {
                var layerNumberString = fieldInfo.GetRawConstantValue().ToString();
                var layerNumber = int.Parse(layerNumberString);
                var layerSerializedProperty = layerArraySerializedProperty.GetArrayElementAtIndex(layerNumber);
                layerSerializedProperty.stringValue = fieldInfo.Name;
            }

            var tagArraySerializedProperty = tagManager.FindProperty("tags");
            var gameTagType = typeof(GameTag);

            fieldInfos = gameTagType.GetFields(BindingFlags.Public | BindingFlags.Static);
            //clear
            tagArraySerializedProperty.arraySize = fieldInfos.Length;

            for (var i = 0; i < fieldInfos.Length; i++)
            {
                var fieldInfo = fieldInfos[i];
                var tagString = fieldInfo.GetRawConstantValue().ToString();
                var layerSerializedProperty = tagArraySerializedProperty.GetArrayElementAtIndex(i);
                layerSerializedProperty.stringValue = tagString;
            }

            tagManager.ApplyModifiedProperties();
        }
    }
}
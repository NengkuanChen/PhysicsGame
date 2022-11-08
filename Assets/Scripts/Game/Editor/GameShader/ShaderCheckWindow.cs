using System.Collections.Generic;
using Game.Editor.Build;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.GameShader
{
    public class ShaderCheckWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Shader检查面板")]
        private static void ShowWindow()
        {
            var window = GetWindow<ShaderCheckWindow>();
            window.titleContent = new GUIContent("Shader检查");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        [SerializeField, Required, InlineEditor(InlineEditorObjectFieldModes.Hidden, Expanded = true), HideLabel,
         BoxGroup("材质shader替换")]
        private ShaderReplaceConfig shaderReplaceConfig;

        protected override void OnEnable()
        {
            base.OnEnable();

            shaderReplaceConfig =
                AssetDatabase.LoadAssetAtPath<ShaderReplaceConfig>("Assets/Build/ShaderReplaceConfig.asset");
        }

        [Button(ButtonSizes.Large, Name = "替换shader"), BoxGroup("材质shader替换")]
        private void ReplaceShaders()
        {
            if (shaderReplaceConfig == null)
            {
                return;
            }

            var assetGuids = AssetDatabase.FindAssets("t:material", new[] {"Assets/Game/GameResources"});
            foreach (var assetGuid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                var shaderName = material.shader.name;

                if (shaderReplaceConfig.TryGetConfig(shaderName, out var replaceShader))
                {
                    material.shader = replaceShader;
                    Debug.Log($"替换shader. from {shaderName} to {replaceShader.name}");
                }
            }

            AssetDatabase.Refresh();
        }

        public static void ReplaceUnityShaderToOurs()
        {
            var shaderReplaceConfig =
                AssetDatabase.LoadAssetAtPath<ShaderReplaceConfig>("Assets/Build/ShaderReplaceConfig.asset");
            if (shaderReplaceConfig == null)
            {
                return;
            }

            var assetGuids = AssetDatabase.FindAssets("t:material", new[] {"Assets/Game/GameResources"});
            foreach (var assetGuid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                var shaderName = material.shader.name;

                if (shaderReplaceConfig.TryGetConfig(shaderName, out var replaceShader))
                {
                    material.shader = replaceShader;
                    Debug.Log($"替换shader. from {shaderName} to {replaceShader.name}");
                }
            }

            AssetDatabase.Refresh();
        }

        [ShowInInspector, BoxGroup("不规范shader使用"), InfoBox("必须使用Custom下面的Shader", InfoMessageType.Warning),
         ListDrawerSettings(Expanded = true), ReadOnly]
        private List<Material> invalidShaderMaterials;

        [Button(ButtonSizes.Large, Name = "搜索使用不规范shader的材质"), BoxGroup("不规范shader使用"),
         ListDrawerSettings(Expanded = true)]
        private void FindInvalidShaders()
        {
            invalidShaderMaterials ??= new List<Material>();
            invalidShaderMaterials.Clear();

            var assetGuids = AssetDatabase.FindAssets("t:material", new[] {"Assets/Game/GameResources"});
            foreach (var guid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                var shaderName = material.shader.name;

                if (!shaderName.StartsWith("Custom"))
                {
                    invalidShaderMaterials.Add(material);
                }
            }
        }
    }
}
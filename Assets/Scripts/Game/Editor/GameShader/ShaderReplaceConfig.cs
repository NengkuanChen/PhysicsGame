using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.GameShader
{
    [CreateAssetMenu(fileName = "ShaderReplaceConfig", menuName = "EditorSettings/ShaderReplaceConfig", order = 0)]
    public class ShaderReplaceConfig : ScriptableObject
    {
        [Serializable]
        public class ReplaceShaderConfig
        {
            [SerializeField, Required]
            private string oldShaderName;
            public string OldShaderName => oldShaderName;
            [SerializeField, Required]
            private Shader toShader;
            public Shader ToShader => toShader;
        }

        [SerializeField, Required]
        private ReplaceShaderConfig[] replaceShaderConfigs;
        public ReplaceShaderConfig[] ReplaceShaderConfigs => replaceShaderConfigs;

        public bool TryGetConfig(string oldShaderName, out Shader replaceShader)
        {
            replaceShader = null;
            foreach (var config in replaceShaderConfigs)
            {
                if (config.OldShaderName == oldShaderName)
                {
                    replaceShader = config.ToShader;
                    return true;
                }
            }

            return false;
        }
    }
}
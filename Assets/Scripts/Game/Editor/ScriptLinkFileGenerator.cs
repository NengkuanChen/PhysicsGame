using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class ScriptLinkFileGenerator
    {
        /// <summary>
        /// 防止被剔除的类型
        /// </summary>
        private static Type[] preventStrippingTypes =
        {
            typeof(AnimationClip), typeof(Animator), typeof(Avatar), typeof(BoxCollider), typeof(MeshCollider),
            typeof(SphereCollider), typeof(CapsuleCollider), typeof(SkinnedMeshRenderer), typeof(Rigidbody),
            typeof(LightProbes), typeof(LightProbeGroup)
        };

        /// <summary>
        /// 自动生成link.xml,用于防止模块剔除
        /// </summary>
        [MenuItem("Tools/生成link.xml")]
        private static void Generate()
        {
            const string filePath = "Assets/Scripts/link.xml";

            var sb = new StringBuilder();
            sb.AppendLine("<linker>");
            foreach (var type in preventStrippingTypes)
            {
                var assemblyFullName = type.Assembly.GetName().Name;
                var typeName = type.FullName;
                sb.AppendLine($"    <assembly fullname=\"{assemblyFullName}\">");
                sb.AppendLine($"        <type fullname=\"{typeName}\"/>");
                sb.AppendLine($"    </assembly>");
            }

            sb.AppendLine("</linker>");
            File.WriteAllText(filePath, sb.ToString());
            AssetDatabase.Refresh();
        }
    }
}
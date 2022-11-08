using UnityEditor;
using UnityEngine;

namespace Game.Editor.ResourceCheckTools
{
    public static class MaterialCheckTool
    {
        public static void Run()
        {
            var assetGuids = AssetDatabase.FindAssets("t:material", new[] {"Assets/Game"});

            foreach (var assetGuid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (material == null)
                {
                    continue;
                }

                var shaderName = material.shader.name;
                if (shaderName.Contains("Standard"))
                {
                    var modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                    if (modelImporter != null)
                    {
                        if (modelImporter.materialImportMode != ModelImporterMaterialImportMode.None)
                        {
                            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
                            modelImporter.SaveAndReimport();
                            Debug.Log($"已将模型{assetPath}的材质模式修改为none,模型必须手动指定材质球");
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogError($"material shader需要修改: {assetPath}.使用了 {shaderName}");
                    }
                }

                RemoveUnusedTextureReferenceInMaterial(material);
            }

            AssetDatabase.Refresh();
        }

        private static void RemoveUnusedTextureReferenceInMaterial(Material material)
        {
            var propertyNames = material.GetTexturePropertyNames();
            foreach (var propertyName in propertyNames)
            {
                var texture = material.GetTexture(propertyName);
                if (texture != null && !material.HasProperty(propertyName))
                {
                    Debug.Log($"清理无效的材质贴图引用. material: {material.name}, texture: {texture.name}");
                    material.SetTexture(propertyName, null);
                }
            }
        }
    }
}
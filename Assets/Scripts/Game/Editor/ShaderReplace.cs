// using UnityEditor;
// using UnityEngine;
//
// namespace Game.Editor
// {
//     public static class ShaderReplace
//     {
//         [MenuItem("Tools/Shader/URPLit to CustomLit")]
//         private static void UrpLitToCustomLit()
//         {
//             var assetGuids = AssetDatabase.FindAssets("t:material", new[] {"Assets/Game/GameResources"});
//             var customLit = Shader.Find("Custom/Lit");
//             foreach (var assetGuid in assetGuids)
//             {
//                 var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
//                 var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
//                 var shaderName = material.shader.name;
//                 if (shaderName == "Universal Render Pipeline/Lit")
//                 {
//                     material.shader = customLit;
//                     Debug.Log($"替换custom/lit shader at {assetPath}");
//                 }
//             }
//             AssetDatabase.Refresh();
//         }
//     }
// }
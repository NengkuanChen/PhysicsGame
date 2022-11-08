// using Facebook.Unity.Settings;

using Game.Advertisement;
using Game.Report;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.Build
{
    public class BuildPanel : OdinEditorWindow
    {
        [MenuItem("打包/面板")]
        private static void ShowWindow()
        {
            var window = GetWindow<BuildPanel>();
            window.titleContent = new GUIContent("打包面板");
            window.minSize = new Vector2(1200, 600);
            window.Show();
        }

        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden, Expanded = true),
         HorizontalGroup("BuildInfo"), LabelText("Android"), PropertyOrder(0), Title("Android")]
        private PackageInfoConfiguration androidPackageInfoConfiguration;
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden, Expanded = true),
         HorizontalGroup("BuildInfo"), LabelText("IOS"), PropertyOrder(0), Title("IOS")]
        private PackageInfoConfiguration iosPackageInfoConfiguration;
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden, Expanded = true), LabelText("Android"),
         PropertyOrder(0), Title("Android构建"), HideIf("@IsMacOsPlatform")]
        private BuildSettingConfiguration androidBuildSettingConfiguration;
        [ShowInInspector, InlineEditor(InlineEditorObjectFieldModes.Hidden, Expanded = true), LabelText("IOS"),
         PropertyOrder(0), Title("IOS构建"), ShowIf("@IsMacOsPlatform")]
        private BuildSettingConfiguration iosBuildSettingConfiguration;

        private bool IsMacOsPlatform
        {
            get
            {
            #if UNITY_EDITOR_OSX
                return true;
            #else
                return false;
            #endif
            }
        }

        private void OnEnable()
        {
            androidPackageInfoConfiguration =
                AssetDatabase.LoadAssetAtPath<PackageInfoConfiguration>(
                    "Assets/Build/PackageInfoConfiguration(Android).asset");
            iosPackageInfoConfiguration =
                AssetDatabase.LoadAssetAtPath<PackageInfoConfiguration>("Assets/Build/PackageInfoConfiguration(IOS).asset");

            if (IsMacOsPlatform)
            {
                iosBuildSettingConfiguration =
                    AssetDatabase.LoadAssetAtPath<BuildSettingConfiguration>(
                        "Assets/Build/BuildSettingConfiguration(IOS).asset");
                iosBuildSettingConfiguration.PackageInfoConfiguration = iosPackageInfoConfiguration;
            }
            else
            {
                androidBuildSettingConfiguration =
                    AssetDatabase.LoadAssetAtPath<BuildSettingConfiguration>(
                        "Assets/Build/BuildSettingConfiguration(Android).asset");
                androidBuildSettingConfiguration.PackageInfoConfiguration = androidPackageInfoConfiguration;
            }
        }

        private void Update()
        {
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("等待工程编译完成..."));
            }
            else
            {
                RemoveNotification();
            }
        }
    }
}
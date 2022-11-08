using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.AssetProcess
{
    [CreateAssetMenu(fileName = "TextureImporterSetting",
        menuName = "EditorSettings/TextureImporterSetting",
        order = 0)]
    public class TextureImporterSetting : ScriptableObject
    {
        private static TextureImporterSetting current;
        public static TextureImporterSetting Current
        {
            get
            {
                if (current == null)
                {
                    current = AssetDatabase.LoadAssetAtPath<TextureImporterSetting>(
                        "Assets/Build/TextureImporterSetting.asset");
                }

                return current;
            }
        }

        [Serializable]
        public class TextureSizeConfig
        {
            [SerializeField, FolderPath]
            private string folderPath = "Assets/Game/GameResources";
            public string FolderPath => folderPath;
            [SerializeField, Min(1)]
            private int maxSize = 1024;
            public int MAXSize => maxSize;
        }

        [BoxGroup("贴图格式配置")]
        [SerializeField, LabelText("Android RGB贴图格式")]
        private TextureImporterFormat androidRGBTextureFormat = TextureImporterFormat.ETC2_RGB4;
        public TextureImporterFormat AndroidRGBTextureFormat => androidRGBTextureFormat;
        [BoxGroup("贴图格式配置")]
        [SerializeField, LabelText("Android RGBA贴图格式")]
        private TextureImporterFormat androidRGBATextureFormat = TextureImporterFormat.ETC2_RGBA8;
        public TextureImporterFormat AndroidRgbaTextureFormat => androidRGBATextureFormat;
        [BoxGroup("贴图格式配置")]
        [SerializeField, LabelText("Android法线贴图格式")]
        private TextureImporterFormat androidNormalTextureFormat = TextureImporterFormat.ETC2_RGB4;
        public TextureImporterFormat AndroidNormalTextureFormat => androidNormalTextureFormat;

        [BoxGroup("贴图格式配置")]
        [SerializeField, LabelText("IOS RGB贴图格式")]
        private TextureImporterFormat iosRGBTextureFormat = TextureImporterFormat.ASTC_6x6;
        public TextureImporterFormat IOSRGBTextureFormat => iosRGBTextureFormat;
        [BoxGroup("贴图格式配置")]
        [SerializeField, LabelText("IOS RGBA贴图格式")]
        private TextureImporterFormat iosRGBATextureFormat = TextureImporterFormat.ASTC_4x4;
        public TextureImporterFormat IOSRgbaTextureFormat => iosRGBATextureFormat;
        [BoxGroup("贴图格式配置")]
        [SerializeField, LabelText("IOS 法线贴图格式")]
        private TextureImporterFormat iosNormalTextureFormat = TextureImporterFormat.ASTC_5x5;
        public TextureImporterFormat IOSNormalTextureFormat => iosNormalTextureFormat;

        [BoxGroup("贴图尺寸配置")]
        [SerializeField, LabelText("默认最大尺寸"), Min(1)]
        private int defaultTextureMaxSize = 512;
        public int DefaultTextureMaxSize => defaultTextureMaxSize;

        [BoxGroup("贴图尺寸配置")]
        [SerializeField, Required]
        private TextureSizeConfig[] textureSizeConfigs;
        public TextureSizeConfig[] TextureSizeConfigs => textureSizeConfigs;
    }
}
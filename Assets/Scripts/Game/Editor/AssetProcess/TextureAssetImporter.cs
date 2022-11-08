using UnityEditor;
using UnityEngine;

namespace Game.Editor.AssetProcess
{
    public class TextureAssetImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Game"))
            {
                return;
            }

            var textureImporter = (TextureImporter) assetImporter;

            //处理UI
            if (assetPath.StartsWith("Assets/Game/UI/Sprites"))
            {
                textureImporter.mipmapEnabled = false;
                if (textureImporter.textureType != TextureImporterType.Sprite)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    Debug.Log($"UI贴图文件{assetPath} 设置为sprite");
                }
            }

            //按照配置文件处理
            else if (assetPath.StartsWith("Assets/Game/GameResources"))
            {
                var textureImporterSetting = TextureImporterSetting.Current;
                if (textureImporterSetting == null)
                {
                    return;
                }

                var maxSize = textureImporterSetting.DefaultTextureMaxSize;
                if (textureImporterSetting.TextureSizeConfigs != null)
                {
                    foreach (var config in textureImporterSetting.TextureSizeConfigs)
                    {
                        if (assetPath.StartsWith(config.FolderPath))
                        {
                            maxSize = config.MAXSize;
                            break;
                        }
                    }
                }

                var textureType = textureImporter.textureType;
                var androidTextureImporterPlatformSettings = textureImporter.GetPlatformTextureSettings("Android");
                var iosTextureImporterPlatformSettings = textureImporter.GetPlatformTextureSettings("iPhone");
                androidTextureImporterPlatformSettings.maxTextureSize = maxSize;
                iosTextureImporterPlatformSettings.maxTextureSize = maxSize;

                static void setDefaultSettingValue(TextureImporterPlatformSettings settings)
                {
                    settings.overridden = true;
                    settings.textureCompression = TextureImporterCompression.Compressed;
                    settings.compressionQuality = 50;
                }

                setDefaultSettingValue(androidTextureImporterPlatformSettings);
                setDefaultSettingValue(iosTextureImporterPlatformSettings);

                if (textureType == TextureImporterType.Default || textureType == TextureImporterType.Lightmap ||
                    textureType == TextureImporterType.DirectionalLightmap)
                {
                    if (textureImporter.DoesSourceTextureHaveAlpha())
                    {
                        androidTextureImporterPlatformSettings.format = textureImporterSetting.AndroidRgbaTextureFormat;
                        iosTextureImporterPlatformSettings.format = textureImporterSetting.IOSRgbaTextureFormat;
                    }
                    else
                    {
                        androidTextureImporterPlatformSettings.format = textureImporterSetting.AndroidRGBTextureFormat;
                        iosTextureImporterPlatformSettings.format = textureImporterSetting.IOSRGBTextureFormat;
                    }
                }
                else if (textureType == TextureImporterType.NormalMap)
                {
                    androidTextureImporterPlatformSettings.format = textureImporterSetting.AndroidNormalTextureFormat;
                    iosTextureImporterPlatformSettings.format = textureImporterSetting.IOSNormalTextureFormat;
                }

                textureImporter.SetPlatformTextureSettings(androidTextureImporterPlatformSettings);
                textureImporter.SetPlatformTextureSettings(iosTextureImporterPlatformSettings);
                // Debug.Log(
                    // $"set texture format {assetPath}  {androidTextureImporterPlatformSettings.format.ToString()}");
            }
        }

        // private void OnPostprocessTexture(Texture2D texture)
        // {
        //     if (!assetPath.StartsWith("Assets/Game"))
        //     {
        //         return;
        //     }
        //
        //     var textureImporter = (TextureImporter) assetImporter;
        //     if (assetPath.StartsWith("Assets/Game/GameResources"))
        //     {
        //       
        //     }
        // }
    }
}
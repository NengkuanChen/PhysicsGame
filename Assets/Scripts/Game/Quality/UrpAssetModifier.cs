using System;
using System.Reflection;
using UnityEngine.Rendering.Universal;

namespace Game.Quality
{
    /// <summary>
    /// 修改urp的配置文件
    /// 因为有些是私有变量，我们要用反射
    /// 设计上不会频繁调用。
    /// </summary>
    public class UrpAssetModifier
    {
        private Type assetType;
        private UniversalRenderPipelineAsset asset;

        private FieldInfo supportMainLightShadowsFieldInfo;
        private FieldInfo supportsSoftShadowsFieldInfo;

        public UrpAssetModifier(UniversalRenderPipelineAsset asset)
        {
            this.asset = asset;

            assetType = asset.GetType();
            supportMainLightShadowsFieldInfo = assetType.GetField("m_MainLightShadowsSupported",
                BindingFlags.Instance | BindingFlags.NonPublic);
            supportsSoftShadowsFieldInfo =
                assetType.GetField("m_SoftShadowsSupported", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public float RenderScale
        {
            get => asset.renderScale;
            set => asset.renderScale = value;
        }

        public bool HDR
        {
            get => asset.supportsHDR;
            set => asset.supportsHDR = value;
        }

        public bool Shadow
        {
            get => asset.supportsMainLightShadows;
            set => supportMainLightShadowsFieldInfo.SetValue(asset, value);
        }

        public bool SoftShadow
        {
            get => asset.supportsSoftShadows;
            set => supportsSoftShadowsFieldInfo.SetValue(asset, value);
        }
    }
}
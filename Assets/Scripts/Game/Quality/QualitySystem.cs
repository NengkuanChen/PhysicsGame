using System.Collections.Generic;
using Game.GameEvent;
using Game.GameSystem;
using Game.Setting;
using Game.Utility;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Quality
{
    public class QualitySystem : SystemBase
    {
        public static QualitySystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as QualitySystem;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private const int SmoothFrameCount = 60;
        private Queue<float> frameElapseSeconds = new Queue<float>();

        private int frameRate = 60;
        public int FrameRate => frameRate;

        private bool recordingRaceFrameRate;
        private float averageRaceFrameRate;

        private int currentQualityLevel;
        public int CurrentQualityLevel => currentQualityLevel;

        private QualitySetting.QualityConfig currentConfig;
        private float degradeTimer;

        private Vector2Int gameCameraPreferRenderingSize;
        public Vector2Int GameCameraPreferRenderingSize => gameCameraPreferRenderingSize;

        private UrpAssetModifier urpAssetModifier;
        public const int MaxResolution = 1080;

        internal override void OnEnable()
        {
            base.OnEnable();

            var qualitySetting = SettingUtility.QualitySetting;
            qualitySetting.Configs.Sort((a, b) => b.TargetFrameRate - a.TargetFrameRate);

            var gameDataSystem = GameDataSystem.Get();
            currentQualityLevel = gameDataSystem.QualityLevel;

            //输出分辨率上限1080
            var urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            urpAssetModifier = new UrpAssetModifier(urpAsset);

            var outputWidth = Screen.width;
            if (CommonUtility.IsWidthScreen)
            {
                var clipOutputWidth = Screen.width * CommonUtility.WidthScreenWidthScale;
                outputWidth = Mathf.RoundToInt(clipOutputWidth);
            }

            if (outputWidth > MaxResolution)
            {
                urpAssetModifier.RenderScale = (float) MaxResolution / outputWidth;
            }

        #if UNITY_EDITOR
            Application.targetFrameRate = qualitySetting.TestFrameRate;
            if (qualitySetting.TestConfigIndex >= 0 && qualitySetting.TestConfigIndex < qualitySetting.Configs.Count)
            {
                var testConfig = qualitySetting.Configs[qualitySetting.TestConfigIndex];
                ApplyQualityLevel(testConfig);
                return;
            }
        #endif

            ApplyQualityLevel(currentQualityLevel);
        }

        internal override void OnDisable()
        {
            base.OnDisable();

            var urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            urpAsset.renderScale = 1;
        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            FrameRateUpdate(realElapseSeconds);
            CheckIsNeedQualityDegradeUpdate(elapseSeconds);
            RecordAverageFrameRateUpdate(elapseSeconds);
        }

        private void FrameRateUpdate(float elapseSeconds)
        {
            if (elapseSeconds >= .1f)
            {
                return;
            }
            
            frameElapseSeconds.Enqueue(elapseSeconds);

            if (frameElapseSeconds.Count > 60)
            {
                frameElapseSeconds.Dequeue();
            }

            var totalElapseSeconds = 0f;
            foreach (var frameElapseSecond in frameElapseSeconds)
            {
                totalElapseSeconds += frameElapseSecond;
            }

            totalElapseSeconds /= frameElapseSeconds.Count;

            totalElapseSeconds = Mathf.Max(totalElapseSeconds, .0001f);

            frameRate = Mathf.RoundToInt(1f / totalElapseSeconds);
            // Log.Info(frameRate);
        }

        private void CheckIsNeedQualityDegradeUpdate(float elapseSeconds)
        {
            if (currentConfig == null)
            {
                return;
            }

            var qualitySetting = SettingUtility.QualitySetting;

            //最低等级不用判断了
            if (currentQualityLevel >= qualitySetting.Configs.Count - 1)
            {
                return;
            }

        #if UNITY_EDITOR
            if (qualitySetting.DisableQualityDegrade)
            {
                return;
            }
        #endif

            if (frameRate < currentConfig.TargetFrameRate)
            {
                degradeTimer += elapseSeconds;

                if (degradeTimer >= qualitySetting.QualityDegradeCheckTimeThreshold)
                {
                    currentQualityLevel++;
                    ApplyQualityLevel(currentQualityLevel);
                    degradeTimer = 0f;
                }
            }
            else
            {
                degradeTimer = 0f;
            }
        }

        private void ApplyQualityLevel(int level)
        {
            Log.Info($"apply quality {level}");

            var qualitySetting = SettingUtility.QualitySetting;
            var config = qualitySetting.Configs[level];
            ApplyQualityLevel(config);
            var gameDataSystem = GameDataSystem.Get();
            gameDataSystem.QualityLevel = level;
            Framework.EventComponent.Fire(this, OnQualityLevelChangedEventArgs.Create(level, config));
        }

        private void ApplyQualityLevel(QualitySetting.QualityConfig config)
        {
            currentConfig = config;

            var urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (urpAsset == null)
            {
                Log.Error("无法找到urp asset");
                return;
            }

            urpAssetModifier.HDR = config.EnableHDR;
            urpAssetModifier.Shadow = config.EnableShadow;
            urpAssetModifier.SoftShadow = config.EnableSoftShadow;

            //main camera render size
            Log.Info($"screen width:{Screen.width}, screen height: {Screen.height}");
            var screenWidth = Screen.width;
            //需要考虑到相机渲染范围会变化（宽屏两边会加黑边）
            // if (CommonUtility.IsWidthScreen)
            // {
            //     screenWidth = Mathf.RoundToInt(screenWidth * CommonUtility.WidthScreenWidthScale);
            // }

            var uiRenderOutputSize = new Vector2(screenWidth, Screen.height);
            uiRenderOutputSize *= urpAsset.renderScale;
            var mainCameraScale = config.Resolution / uiRenderOutputSize.x;
            gameCameraPreferRenderingSize = new Vector2Int(config.Resolution,
                Mathf.RoundToInt(uiRenderOutputSize.y * mainCameraScale));
            
            if (GameMainCamera.Current != null)
            {
                GameMainCamera.Current.ApplyRenderSize(gameCameraPreferRenderingSize);
            }
        }

        public void StartRecordAverageFrameRate()
        {
            recordingRaceFrameRate = true;
            averageRaceFrameRate = 30;
        }

        public int StopRecordAverageFrameRate()
        {
            recordingRaceFrameRate = false;
            return Mathf.FloorToInt(averageRaceFrameRate);
        }

        private void RecordAverageFrameRateUpdate(float elapseSeconds)
        {
            if (!recordingRaceFrameRate)
            {
                return;
            }

            averageRaceFrameRate += frameRate;
            averageRaceFrameRate *= .5f;
        }
    }
}
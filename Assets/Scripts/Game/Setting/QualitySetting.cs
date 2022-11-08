using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Setting
{
    [CreateAssetMenu(fileName = "QualitySetting", menuName = "Settings/QualitySetting", order = 0)]
    public class QualitySetting : ScriptableObject
    {
    #if UNITY_EDITOR
        [BoxGroup("测试"), SerializeField, LabelText("测试帧率"), Min(1)]
        private int testFrameRate = 60;
        public int TestFrameRate => testFrameRate;
        [BoxGroup("测试"), SerializeField, LabelText("测试配置index"), Min(-1), InfoBox("-1表示没有测试")]
        private int testConfigIndex = -1;
        public int TestConfigIndex => testConfigIndex;
        [BoxGroup("测试"), SerializeField, LabelText("关闭画质切换功能")]
        private bool disableQualityDegrade;
        public bool DisableQualityDegrade => disableQualityDegrade;
    #endif

        [Serializable]
        public class QualityConfig
        {
            [SerializeField, LabelText("目标帧数"), Min(1), MaxValue(60)]
            private int targetFrameRate = 45;
            public int TargetFrameRate => targetFrameRate;
            [SerializeField, LabelText("渲染分辨率"), Min(64)]
            private int resolution = 1080;
            public int Resolution => resolution;
            [SerializeField, LabelText("开启HDR")]
            private bool enableHDR = true;
            public bool EnableHDR => enableHDR;
            [SerializeField, LabelText("开启软阴影")]
            private bool enableSoftShadow = true;
            public bool EnableSoftShadow => enableSoftShadow;
            [SerializeField, LabelText("开启阴影")]
            private bool enableShadow = true;
            public bool EnableShadow => enableShadow;

            public QualityConfig(int targetFrameRate, int resolution, bool enableHDR, bool enableShadow)
            {
                this.targetFrameRate = targetFrameRate;
                this.resolution = resolution;
                this.enableHDR = enableHDR;
                this.enableShadow = enableShadow;
            }
        }

        [SerializeField, ListDrawerSettings(ShowIndexLabels = true)]
        private List<QualityConfig> configs = new List<QualityConfig>
        {
            new QualityConfig(45, 1080, true, true),
            new QualityConfig(35, 720, false, true),
            new QualityConfig(1, 480, false, false)
        };
        public List<QualityConfig> Configs => configs;

        [SerializeField, LabelText("降级检测时间阈值"), Min(.1f)]
        private float qualityDegradeCheckTimeThreshold = 8f;
        public float QualityDegradeCheckTimeThreshold => qualityDegradeCheckTimeThreshold;
    }
}
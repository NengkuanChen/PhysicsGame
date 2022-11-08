using System;
using System.IO;
using System.Text;
using Game.GameEvent;
using Game.Utility;
using Newtonsoft.Json;
using Table;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Game.GameSystem
{
    public class GameDataSystem : SystemBase
    {
        public static GameDataSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as GameDataSystem;
        }

        private static int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private const int SaveFileVersion = 1;
        private readonly byte[] encryptionBytes = Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier);
    #if UNITY_EDITOR
        private readonly string saveFilePath = Application.persistentDataPath + "/save.txt";
    #else
         private readonly string saveFilePath = Application.persistentDataPath + "/save.dat";
    #endif
        private bool needSave;

        [Serializable]
        private class PersistenceData
        {
            public int dataVersion = -1;

            public int variantVersion;
            public string variantName = "Default";

            /// <summary>
            /// 总的游戏时长
            /// </summary>
            public uint totalGameTime;
            /// <summary>
            /// 游戏运行次数
            /// </summary>
            public uint gameLaunchTimes;
            /// <summary>
            /// 第一次运行游戏的UTC时间
            /// </summary>
            public double firstLaunchGameUtcTimestamp;

            /// <summary>
            /// 上一次离开游戏时的UTC时间
            /// </summary>
            public double lastLeaveGameUtcTime = -1;

            public int QualityLevel;
            public bool isMute;
        }

        private float autoSaveTimer;
        private const float AutoSaveInterval = 60f;

        private float updateTotalPlayTimeTimer;
        /// <summary>
        /// 总的游戏时长（秒）
        /// </summary>
        public uint TotalGameTime => persistenceData.totalGameTime;

        /// <summary>
        /// 游戏运行次数
        /// </summary>
        public uint GameLaunchTimes => persistenceData.gameLaunchTimes;

        private PersistenceData persistenceData;

        internal override void OnEnable()
        {
            base.OnEnable();
            LoadData();

            persistenceData.gameLaunchTimes++;
            var utcNow = CommonUtility.UtcNow;

            //第一次启动游戏
            if (persistenceData.gameLaunchTimes == 1)
            {
                persistenceData.firstLaunchGameUtcTimestamp = utcNow;
            }

            if (persistenceData.lastLeaveGameUtcTime < 0)
            {
                persistenceData.lastLeaveGameUtcTime = utcNow;
            }

            autoSaveTimer = AutoSaveInterval;
            Application.focusChanged += OnFocusChanged;
        }

        internal override void OnDisable()
        {
            base.OnDisable();

            Application.focusChanged -= OnFocusChanged;
            SaveDataImmediate();
        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (elapseSeconds > 1f)
            {
                elapseSeconds = 1f;
            }

            updateTotalPlayTimeTimer += elapseSeconds;
            if (updateTotalPlayTimeTimer >= 1f)
            {
                updateTotalPlayTimeTimer -= 1f;
                persistenceData.totalGameTime += 1;
            }

            autoSaveTimer -= elapseSeconds;
            if (autoSaveTimer <= 0f)
            {
                Log.Info("auto save");
                SaveData();
            }

            if (needSave)
            {
                needSave = false;
                SaveDataImmediate();
            }
        }

        private void LoadData()
        {
            if (!File.Exists(saveFilePath))
            {
                persistenceData = new PersistenceData();
            }
            else
            {
                void createNewSaveDataWhenDataIsCorruption()
                {
                    Log.Error($"存档文件可能已损坏 {SystemInfo.deviceUniqueIdentifier}");
                    persistenceData = new PersistenceData();
                }

                if (Application.isEditor)
                {
                    var loadString = File.ReadAllText(saveFilePath);
                    try
                    {
                        persistenceData = JsonConvert.DeserializeObject<PersistenceData>(loadString);
                    }
                    catch (Exception e)
                    {
                        createNewSaveDataWhenDataIsCorruption();
                    }
                }
                else
                {
                    try
                    {
                        var loadBytes = File.ReadAllBytes(saveFilePath);
                        GameFramework.Utility.Encryption.GetSelfXorBytes(loadBytes, encryptionBytes);
                        var jsonString = Encoding.UTF8.GetString(loadBytes);

                        persistenceData = JsonConvert.DeserializeObject<PersistenceData>(jsonString);

                        if (persistenceData.dataVersion != SaveFileVersion)
                        {
                            Log.Info("存档文件版本升级");
                            OnUpgradeSaveFromOlderVersion(persistenceData.dataVersion);
                            persistenceData.dataVersion = SaveFileVersion;
                        }
                    }
                    catch (Exception e)
                    {
                        createNewSaveDataWhenDataIsCorruption();
                    }
                }

                if (persistenceData == null)
                {
                    createNewSaveDataWhenDataIsCorruption();
                }
            }
        }

        private void OnUpgradeSaveFromOlderVersion(int oldVersion)
        {
        }

        private void OnFocusChanged(bool isFocus)
        {
        #if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                Application.focusChanged -= OnFocusChanged;
                return;
            }
        #endif

            if (!isFocus)
            {
                var utcNow = CommonUtility.UtcNow;
                Log.Info($"leave game at utc time: {utcNow}");
                persistenceData.lastLeaveGameUtcTime = utcNow;
                SaveDataImmediate();
            }

            Framework.EventComponent.Fire(this, OnGameFocusChangedEventArgs.Create(isFocus));
        }

        private void SaveDataImmediate()
        {
        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
        #endif

            persistenceData.dataVersion = SaveFileVersion;

            var jsonString = JsonConvert.SerializeObject(persistenceData, Formatting.Indented);
            if (Application.isEditor)
            {
                File.WriteAllText(saveFilePath, jsonString);
            }
            else
            {
                var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                GameFramework.Utility.Encryption.GetSelfXorBytes(jsonBytes, encryptionBytes);
                File.WriteAllBytes(saveFilePath, jsonBytes);
            }

            Log.Info("game saved");
        }

        public void SaveData()
        {
            autoSaveTimer = AutoSaveInterval;
            needSave = true;
        }

        public double FirstLaunchGameUtcTimestamp => persistenceData.firstLaunchGameUtcTimestamp;
        public double LastLeaveGameUtcTime => persistenceData.lastLeaveGameUtcTime;
        public string VariantName
        {
            get => persistenceData.variantName;
            set => persistenceData.variantName = value;
        }
        public int VariantVersion
        {
            get => persistenceData.variantVersion;
            set => persistenceData.variantVersion = value;
        }

        public int QualityLevel
        {
            get => persistenceData.QualityLevel;
            set => persistenceData.QualityLevel = value;
        }

        public bool IsMute
        {
            get => persistenceData.isMute;
            set
            {
                persistenceData.isMute = value;
                Framework.EventComponent.Fire(this, OnAudioStatusChangedEventArgs.Create(!value));
            }
        }

        public void ClearSaveData()
        {
            persistenceData = new PersistenceData();
            SaveDataImmediate();
        }
    }
}
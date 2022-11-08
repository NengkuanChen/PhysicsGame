﻿using Game.Setting;
using Game.UI;
using GameFramework;
using GameFramework.Event;

namespace Game.GameEvent
{
    public class OnFormOpenedEventArgs : GameEventArgs
    {
        public GameUIFormLogic openedForm;
        public int openedFormType;

        public override void Clear()
        {
            openedForm = null;
            openedFormType = 0;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnFormOpenedEventArgs Create(GameUIFormLogic openedForm, int openedFormType)
        {
            var arg = ReferencePool.Acquire<OnFormOpenedEventArgs>();
            arg.openedForm = openedForm;
            arg.openedFormType = openedFormType;
            return arg;
        }
    }

    public class OnFormClosedEventArgs : GameEventArgs
    {
        public int closedFormType;

        public override void Clear()
        {
            closedFormType = 0;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnFormClosedEventArgs Create(int closedFormType)
        {
            var arg = ReferencePool.Acquire<OnFormClosedEventArgs>();
            arg.closedFormType = closedFormType;
            return arg;
        }
    }

    public class OnGameFocusChangedEventArgs : GameEventArgs
    {
        private bool isFocus;
        public bool IsFocus => isFocus;

        public override void Clear()
        {
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnGameFocusChangedEventArgs Create(bool isFocus)
        {
            var arg = ReferencePool.Acquire<OnGameFocusChangedEventArgs>();
            arg.isFocus = isFocus;
            return arg;
        }
    }

    public class OnQualityLevelChangedEventArgs : GameEventArgs
    {
        private int qualityLevel;
        public int QualityLevel => qualityLevel;

        private QualitySetting.QualityConfig qualityConfig;
        public QualitySetting.QualityConfig QualityConfig => qualityConfig;

        public override void Clear()
        {
            qualityConfig = null;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnQualityLevelChangedEventArgs Create(int qualityLevel,
                                                            QualitySetting.QualityConfig qualityConfig)
        {
            var arg = ReferencePool.Acquire<OnQualityLevelChangedEventArgs>();
            arg.qualityLevel = qualityLevel;
            arg.qualityConfig = qualityConfig;
            return arg;
        }
    }

    public class OnAudioStatusChangedEventArgs : GameEventArgs
    {
        private bool isEnable;
        public bool IsEnable => isEnable;
        public override void Clear()
        {
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnAudioStatusChangedEventArgs Create(bool isEnable)
        {
            var arg = ReferencePool.Acquire<OnAudioStatusChangedEventArgs>();
            arg.isEnable = isEnable;
            return arg;
        }
    }

    public class OnGameStartEventArgs : GameEventArgs
    {
        public override void Clear()
        {
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        public override int Id => UniqueId;

        public static OnGameStartEventArgs Create()
        {
            return ReferencePool.Acquire<OnGameStartEventArgs>();
        }
    }
}
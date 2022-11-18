using Game.Ball;
using Game.PlatForm;
using Game.Setting;
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

    public class OnPlayerTapScreen : GameEventArgs
    {
        public override void Clear()
        {
            
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnPlayerTapScreen Create()
        {
            return ReferencePool.Acquire<OnPlayerTapScreen>();
        }
    }

    public class OnPlayerMoveBallEventArgs : GameEventArgs
    {
        public override void Clear()
        {
            
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public float Axis;

        public static OnPlayerMoveBallEventArgs Create(float axis)
        {
            var arg = ReferencePool.Acquire<OnPlayerMoveBallEventArgs>();
            arg.Axis = axis;
            return arg;
        }
    }
    
    public class OnControlFormHitEventArgs : GameEventArgs
    {
        public override void Clear()
        {
            
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnControlFormHitEventArgs Create()
        {
            return ReferencePool.Acquire<OnControlFormHitEventArgs>();
        }
    }
    
    public class OnBallSwitchEventArgs : GameEventArgs
    {
        public override void Clear()
        {
            
        }

        public BallType BallType;

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public static OnBallSwitchEventArgs Create(BallType ballType)
        {
            var arg = ReferencePool.Acquire<OnBallSwitchEventArgs>();
            arg.BallType = ballType;
            return arg;
        }
    }
    
    public class OnEditorPlayerMoveBallEventArgs : GameEventArgs
    {
        public override void Clear()
        {
            
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public float Axis;

        public static OnEditorPlayerMoveBallEventArgs Create(float axis)
        {
            var arg = ReferencePool.Acquire<OnEditorPlayerMoveBallEventArgs>();
            arg.Axis = axis;
            return arg;
        }
    }
    
    public class OnPlayerEnterPlatformGroupEventArgs : GameEventArgs
    {
        public override void Clear()
        {
            PlatformGroup = null;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int Id => UniqueId;

        public PlatformGroupEntity PlatformGroup;
        public static OnPlayerEnterPlatformGroupEventArgs Create(PlatformGroupEntity platformGroup)
        {
            var arg = ReferencePool.Acquire<OnPlayerEnterPlatformGroupEventArgs>();
            arg.PlatformGroup = platformGroup;
            return arg;
        }
    }
}

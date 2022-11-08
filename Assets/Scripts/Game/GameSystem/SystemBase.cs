namespace Game.GameSystem
{
    public abstract class SystemBase
    {
        internal abstract int ID { get; }
        public virtual int Priority { get; } = SystemPriority.Common;

        private bool available = true;
        internal bool Available => available;

        protected SystemBase()
        {
            SystemEntry.AddSystem(this);
        }

        internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal virtual void OnEnable()
        {
            available = true;
        }

        internal virtual void OnDisable()
        {
            available = false;
        }

    #if UNITY_EDITOR
        /// <summary>
        /// 该方法只在editor中有效，继承的方法同样需要放在UNITY_EDITOR宏中
        /// 该方法只用来执行那些，编辑器停止播放时，需要释放或者重置的内容
        /// </summary>
        internal virtual void OnEditorStopPlay()
        {
        }
    #endif

        public void RemoveSelf()
        {
            SystemEntry.RemoveSystem(ID);
        }

    #if UNITY_EDITOR

        public virtual void OnDrawGizmos()
        {
        }
    #endif
    }
}
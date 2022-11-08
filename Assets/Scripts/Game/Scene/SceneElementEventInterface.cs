namespace Game.Scene
{
    public interface ISceneElementEventInterface
    {
    }

    /// <summary>
    /// SceneElement需要Update
    /// </summary>
    public interface ISceneElementUpdate : ISceneElementEventInterface
    {
        public void OnUpdate(float elapseSeconds, float realElapseSeconds);
    }
    
    /// <summary>
    /// SceneElement需要FixedUpdate
    /// </summary>
    public interface ISceneElementFixedUpdate : ISceneElementEventInterface
    {
        public void OnFixedUpdate(float elapseSeconds, float realElapseSeconds);
    }

    /// <summary>
    /// SceneElement需要LateUpdate
    /// </summary>
    public interface ISceneElementLateUpdate : ISceneElementEventInterface
    {
        public void OnLateUpdate(float elapseSeconds, float realElapseSeconds);
    }

    /// <summary>
    /// 游戏开始时
    /// </summary>
    public interface ISceneElementOnGameStart : ISceneElementEventInterface
    {
        public void OnGameStart();
    }

    /// <summary>
    /// 场景重置时
    /// </summary>
    public interface ISceneElementOnSceneReset : ISceneElementEventInterface
    {
        public void OnSceneReset();
    }

    /// <summary>
    /// 场景卸载之前
    /// </summary>
    public interface ISceneElementBeforeSceneUnload : ISceneElementEventInterface
    {
        public void OnBeforeSceneUnload();
    }
}
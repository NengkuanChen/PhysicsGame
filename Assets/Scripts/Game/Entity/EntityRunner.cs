#if UNITY_EDITOR
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entity
{
    /// <summary>
    /// 主要用于entity直接在场景中测试
    /// </summary>
    public abstract class EntityRunner : MonoBehaviour
    {
        [SerializeField, Required]
        protected GameEntityLogic targetLogic;

        protected virtual void OnEnable()
        {
            if (targetLogic == null)
            {
                enabled = false;
                return;
            }

            StartCoroutine(DelayInitCoroutine());
        }

        private IEnumerator DelayInitCoroutine()
        {
            yield return null;
            targetLogic.OnInit(null);
            targetLogic.InvokeOnShow();
            InitComponents();
        }

        protected abstract void InitComponents();

        protected void AddComponent(EntityComponentBase component)
        {
            if (targetLogic != null)
            {
                targetLogic.AddComponent(component);
            }
        }

        private void OnDisable()
        {
            targetLogic.InvokeOnHide();
        }

        private void Update()
        {
            targetLogic.InvokeOnUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            if (targetLogic.Available)
            {
                targetLogic.OnLateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (targetLogic.Available)
            {
                targetLogic.OnFixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
            }
        }

        [Button(ButtonSizes.Large)]
        private void Setup()
        {
            targetLogic = GetComponent<GameEntityLogic>();
        }
    }
}
#endif
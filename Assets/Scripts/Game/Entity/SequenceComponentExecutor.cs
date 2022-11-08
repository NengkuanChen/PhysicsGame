using System;
using System.Collections.Generic;

namespace Game.Entity
{
    /// <summary>
    /// 顺序执行一组组件
    /// </summary>
    public class SequenceComponentExecutor
    {
        private class SequenceStep
        {
        }

        private class ComponentSequenceStep : SequenceStep
        {
            private EntityComponentBase component;
            public EntityComponentBase Component => component;

            public ComponentSequenceStep(EntityComponentBase component)
            {
                this.component = component;
            }
        }

        private class DelaySequenceStep : SequenceStep
        {
            private float delayTime;
            public float DelayTime => delayTime;

            public bool Tick(float elapseSeconds)
            {
                delayTime -= elapseSeconds;
                return delayTime <= 0f;
            }

            public DelaySequenceStep(float delayTime)
            {
                this.delayTime = delayTime;
            }
        }

        private LinkedList<SequenceStep> sequenceSteps = new LinkedList<SequenceStep>();
        private LinkedListNode<SequenceStep> runningNode;

        private GameEntityLogic ownerEntity;

        /// <summary>
        /// 等到列表中最后一个组件都被移除后,才算Sequence完成
        /// </summary>
        public bool WaitLastComponentBeenRemoved { get; set; }

        public SequenceComponentExecutor(GameEntityLogic ownerEntity)
        {
            this.ownerEntity = ownerEntity;
        }

        public void EnqueueComponent(EntityComponentBase component)
        {
            var componentNode = sequenceSteps.AddLast(new ComponentSequenceStep(component));
            if (sequenceSteps.Count == 1)
            {
                //第一个组件
                ownerEntity.AddComponent(component);
                runningNode = componentNode;
            }
        }

        public void EnqueueDelay(float time)
        {
            var delayNode = sequenceSteps.AddLast(new DelaySequenceStep(time));
            if (sequenceSteps.Count == 1)
            {
                //第一个组件
                runningNode = delayNode;
            }
        }

        public void RemoveAllSequenceComponents()
        {
            var node = sequenceSteps.First;
            while (node != null)
            {
                if (node.Value is ComponentSequenceStep componentSequenceStep)
                {
                    ownerEntity.RemoveComponent(componentSequenceStep.Component);
                }

                node = node.Next;
            }

            sequenceSteps.Clear();
        }

        public void Update(float elapseSeconds, out bool isComplete)
        {
            isComplete = false;
            if (runningNode == null)
            {
                isComplete = true;
                return;
            }

            if (runningNode.Value is ComponentSequenceStep componentSequenceStep)
            {
                var runningComponent = componentSequenceStep.Component;
                //当组件被删除后,添加下一个组件
                if (!runningComponent.Available)
                {
                    ownerEntity.RemoveComponent(runningComponent);
                    if (MoveToNextStep())
                    {
                        isComplete = true;
                    }
                }
            }
            else if (runningNode.Value is DelaySequenceStep delaySequenceStep)
            {
                if (delaySequenceStep.Tick(elapseSeconds))
                {
                    if (MoveToNextStep())
                    {
                        isComplete = true;
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private bool MoveToNextStep()
        {
            runningNode = runningNode.Next;

            if (runningNode == null)
            {
                return true;
            }

            if (runningNode.Value is ComponentSequenceStep componentSequenceStep)
            {
                ownerEntity.AddComponent(componentSequenceStep.Component);
            }
            else if (runningNode.Value is DelaySequenceStep delaySequenceStep)
            {
                //do nothing
            }
            else
            {
                throw new NotImplementedException();
            }

            return false;
        }
    }
}
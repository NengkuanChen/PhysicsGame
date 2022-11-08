using System;

namespace Game.Entity
{
    /// <summary>
    /// 指定该component只能适用于某一个entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SpecificEntityComponentBase<T> : EntityComponentBase where T : GameEntityLogic
    {
        private T specificOwner;
        protected override GameEntityLogic OwnerEntity
        {
            get => specificOwner;
            set
            {
                if (value == null)
                {
                    specificOwner = null;
                    return;
                }

                specificOwner = value as T;
                if (specificOwner == null)
                {
                    throw new Exception(
                        $"组件 {GetType().FullName} 附加到的实体{value.Name},类型{value.GetType().FullName},无法转换到类型 {typeof(T).FullName} 上");
                }
            }
        }
        protected T OwnerEntityType => specificOwner;
    }

    public abstract class SpecificInterfaceComponentBase<T> : EntityComponentBase where T : class
    {
        private T specificOwner;
        protected override GameEntityLogic OwnerEntity
        {
            get => specificOwner as GameEntityLogic;
            set
            {
                if (value == null)
                {
                    specificOwner = null;
                    return;
                }

                specificOwner = value as T;
                if (specificOwner == null)
                {
                    throw new Exception(
                        $"组件 {GetType().FullName} 附加到的实体{value.Name},类型{value.GetType().FullName},无法转换到接口 {typeof(T).FullName} 上");
                }
            }
        }
        protected T OwnerEntityType => specificOwner;
    }
}
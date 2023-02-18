using System;

namespace CryoDI
{
#if UNITY_5_3_OR_NEWER
    public class PrefabInstantiatorFactory : IFactory
    {
        public LifeTime LifeTime => LifeTime.Global;
        public bool CanCreate(Type type, string name)
        {
            return type == typeof(IPrefabInstantiator);
        }

        public object Create(Type type, string name, CryoContainer container)
        {
            if (type == typeof(IPrefabInstantiator))
                return new PrefabInstantiator(container);
            return null;
        }
    }
#endif
}
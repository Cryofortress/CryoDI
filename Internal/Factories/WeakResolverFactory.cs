using System;

namespace CryoDI
{
    public class WeakResolverFactory : IFactory
    {
        public LifeTime LifeTime => LifeTime.Global;
        public bool CanCreate(Type type, string name)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IWeakResolver<>));
        }

        public object Create(Type type, string name, CryoContainer container)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IWeakResolver<>))
            {
                return CreateWeakResolver(type, name, container);
            }
            return null;
        }
        
        private object CreateWeakResolver(Type propertyType, string name, CryoContainer container)
        {
            var args = propertyType.GetGenericArguments();
            var resolverType = typeof(WeakResolver<>).MakeGenericType(args);
            return Activator.CreateInstance(resolverType, name, container);
        }
    }
}
using System;

namespace CryoDI
{
    public class ResolverFactory : IFactory
    {
        public LifeTime LifeTime => LifeTime.Global;
        
        public bool CanCreate(Type type, string name)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IResolver<>);
        }

        public object Create(Type type, string name, CryoContainer container)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IResolver<>))
            {
                return CreateResolver(type, name, container);
            }

            return null;
        }
        
        private object CreateResolver(Type propertyType, string name, CryoContainer container)
        {
            var args = propertyType.GetGenericArguments();
            var resolverType = typeof(Resolver<>).MakeGenericType(args);
            return Activator.CreateInstance(resolverType, name, container);
        }
    }
}
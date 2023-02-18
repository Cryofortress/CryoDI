using System;

namespace CryoDI
{
    internal class BuilderFactory : IFactory
    {
        public LifeTime LifeTime => LifeTime.Global;
        
        public bool CanCreate(Type type, string name)
        {
            if (name != null) return false;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IBuilder<>))
                return true;
            return false;
        }

        public object Create(Type type, string name, CryoContainer container)
        {
            if (name != null) return null;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IBuilder<>))
            {
                return CreateBuilder(type, container);
            }

            return null;
        }
        
        private object CreateBuilder(Type propertyType, CryoContainer container)
        {
            var args = propertyType.GetGenericArguments();
            var resolverType = typeof(Builder<>).MakeGenericType(args);
            return Activator.CreateInstance(resolverType, container);
        }
    }
}
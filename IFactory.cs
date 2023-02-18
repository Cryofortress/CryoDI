using System;

namespace CryoDI
{
    /// <summary>
    /// Фабрика объектов
    /// </summary>
    public interface IFactory
    {
        LifeTime LifeTime { get; }
        
        bool CanCreate(Type type, string name);

        object Create(Type type, string name, CryoContainer container);
    }
}
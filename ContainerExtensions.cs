using System;
using CryoDI.Providers;

namespace CryoDI
{
	public static class ContainerExtensions
	{
		#region Type 
		public static Container RegisterType<T>(this Container container) where T : new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(LifeTime.Global), null);
		}
		
		public static Container RegisterType<T>(this Container container, string name) where T : new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(LifeTime.Global), name);
		}

		public static Container RegisterType<T>(this Container container, LifeTime lifeTime) where T : class, new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(lifeTime), null); 
		}
		
		public static Container RegisterType<T>(this Container container, string name, LifeTime lifeTime) where T : new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(lifeTime), name);
		}
		
		public  static Container RegisterType<TBase, TDerived>(this Container container) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(LifeTime.Global), null);
		}

		public  static Container RegisterType<TBase, TDerived>(this Container container, string name) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(LifeTime.Global), name);
		}

		public  static Container RegisterType<TBase, TDerived>(this Container container, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(lifeTime), null);
		}
		
		public  static Container RegisterType<TBase, TDerived>(this Container container, string name, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(lifeTime), name);
		}
		
		public static Container RegisterType<T>(this Container container, Func<T> factoryMethod)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, LifeTime.Global), null);
		}
		
		public static Container RegisterType<T>(this Container container, Func<T> factoryMethod, string name)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, LifeTime.Global), name);
		}

		public static Container RegisterType<T>(this Container container, Func<T> factoryMethod, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, lifeTime), null); 
		}
		
		public static Container RegisterType<T>(this Container container, Func<T> factoryMethod, string name, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, lifeTime), name);
		}
		
		public  static Container RegisterType<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, LifeTime.Global), null);
		}

		public  static Container RegisterType<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod, string name) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, LifeTime.Global), name);
		}

		public  static Container RegisterType<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, lifeTime), null);
		}
		
		public  static Container RegisterType<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod, string name, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, lifeTime), name);
		}
		#endregion Type
		
		#region Singleton
		
		public static Container RegisterSingleton<T>(this Container container) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(LifeTime.Global), null);
		}
		
		public static Container RegisterSingleton<T>(this Container container, Func<T> factoryMethod)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, LifeTime.Global), null);
		}
		
		public static Container RegisterSingleton<T>(this Container container, LifeTime lifeTime) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(lifeTime), null);
		}
		
		public static Container RegisterSingleton<T>(this Container container, Func<T> factoryMethod, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, lifeTime), null);
		}
		
		public static Container RegisterSingleton<T>(this Container container, string name) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(LifeTime.Global), name);
		}
		
		public static Container RegisterSingleton<T>(this Container container, Func<T> factoryMethod, string name)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, LifeTime.Global), name);
		}
		
		public static Container RegisterSingleton<T>(this Container container, string name, LifeTime lifeTime) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(lifeTime), name);
		}
		
		public static Container RegisterSingleton<T>(this Container container, Func<T> factoryMethod, string name, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, lifeTime), name);
		}

		
		public static Container RegisterSingleton<TBase, TDerived>(this Container container) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(LifeTime.Global), null);
		}
		
		public static Container RegisterSingleton<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, LifeTime.Global), null);
		}
	    
		public static Container RegisterSingleton<TBase, TDerived>(this Container container, string name) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(LifeTime.Global), name);
		}
		
		public static Container RegisterSingleton<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod,string name) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, LifeTime.Global), name);
		}
		
		public static Container RegisterSingleton<TBase, TDerived>(this Container container, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(lifeTime), null);
		}
		
		public static Container RegisterSingleton<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod, LifeTime lifeTime) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, lifeTime), null);
		}
		
		public static Container RegisterSingleton<TBase, TDerived>(this Container container, string name, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(lifeTime), name);
		}
		
		public static Container RegisterSingleton<TBase, TDerived>(this Container container, Func<TDerived> factoryMethod, string name, LifeTime lifeTime) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, lifeTime), name);
		}
		#endregion Singleton
		
		#region Instance
		public static Container RegisterInstance<T>(this Container container, T obj, string name = null, LifeTime lifeTime = LifeTime.Global)
		{
			return container.RegisterProvider<T>(new InstanceProvider<T>(obj, lifeTime), name);
		}

		public static Container RegisterInstance<T>(this Container container, T obj, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new InstanceProvider<T>(obj, lifeTime), null);
		}

		public static Container RegisterInstance<TBase, TDerived>(this Container container, TDerived obj, string name = null, LifeTime lifeTime = LifeTime.Global) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new InstanceProvider<TDerived>(obj, lifeTime), name);
		}

		public static Container RegisterInstance<TBase, TDerived>(this Container container, TDerived obj, LifeTime lifeTime) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new InstanceProvider<TDerived>(obj, lifeTime), null);
		}
		#endregion Instance
		
		#region SceneObject
#if UNITY_5_3_OR_NEWER
		public static Container RegisterSceneObject<T>(this Container container, string path, string name = null, LifeTime lifeTime = LifeTime.Global)
		{
			return container.RegisterProvider<T>(new ScenePathProvider<T>(path, lifeTime), name);
		}

		public static Container RegisterSceneObject<T>(this Container container, string path, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new ScenePathProvider<T>(path, lifeTime), null);
		}
#endif
		#endregion SceneObject

	}
}
using System;
using CryoDI.Providers;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace CryoDI
{
	public static class ContainerExtensions
	{
		#region Type 
		public static CryoContainer RegisterType<T>(this CryoContainer container) where T : new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(LifeTime.Global), null);
		}
		
		public static CryoContainer RegisterType<T>(this CryoContainer container, string name) where T : new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(LifeTime.Global), name);
		}

		public static CryoContainer RegisterType<T>(this CryoContainer container, LifeTime lifeTime) where T : class, new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(lifeTime), null); 
		}
		
		public static CryoContainer RegisterType<T>(this CryoContainer container, string name, LifeTime lifeTime) where T : new()
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(lifeTime), name);
		}
		
		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(LifeTime.Global), null);
		}

		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container, string name) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(LifeTime.Global), name);
		}

		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(lifeTime), null);
		}
		
		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container, string name, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(lifeTime), name);
		}
		
		public static CryoContainer RegisterType<T>(this CryoContainer container, Func<T> factoryMethod)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, LifeTime.Global), null);
		}
		
		public static CryoContainer RegisterType<T>(this CryoContainer container, Func<T> factoryMethod, string name)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, LifeTime.Global), name);
		}

		public static CryoContainer RegisterType<T>(this CryoContainer container, Func<T> factoryMethod, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, lifeTime), null); 
		}
		
		public static CryoContainer RegisterType<T>(this CryoContainer container, Func<T> factoryMethod, string name, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new FactoryMethodProvider<T>(factoryMethod, lifeTime), name);
		}
		
		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, LifeTime.Global), null);
		}

		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod, string name) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, LifeTime.Global), name);
		}

		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, lifeTime), null);
		}
		
		public  static CryoContainer RegisterType<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod, string name, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new FactoryMethodProvider<TDerived>(factoryMethod, lifeTime), name);
		}
		#endregion Type
		
		#region Singleton
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(LifeTime.Global), null);
		}
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container, Func<T> factoryMethod)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, LifeTime.Global), null);
		}
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container, LifeTime lifeTime) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(lifeTime), null);
		}
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container, Func<T> factoryMethod, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, lifeTime), null);
		}
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container, string name) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(LifeTime.Global), name);
		}
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container, Func<T> factoryMethod, string name)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, LifeTime.Global), name);
		}
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container, string name, LifeTime lifeTime) where T : new()
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(lifeTime), name);
		}
		
		public static CryoContainer RegisterSingleton<T>(this CryoContainer container, Func<T> factoryMethod, string name, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new SingletonProvider<T>(factoryMethod, lifeTime), name);
		}

		
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(LifeTime.Global), null);
		}
		
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, LifeTime.Global), null);
		}
	    
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container, string name) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(LifeTime.Global), name);
		}
		
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod,string name) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, LifeTime.Global), name);
		}
		
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(lifeTime), null);
		}
		
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod, LifeTime lifeTime) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, lifeTime), null);
		}
		
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container, string name, LifeTime lifeTime) where TDerived : TBase, new()
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(lifeTime), name);
		}
		
		public static CryoContainer RegisterSingleton<TBase, TDerived>(this CryoContainer container, Func<TDerived> factoryMethod, string name, LifeTime lifeTime) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new SingletonProvider<TDerived>(factoryMethod, lifeTime), name);
		}
		#endregion Singleton
		
		#region Instance
		public static CryoContainer RegisterInstance<T>(this CryoContainer container, T obj, string name = null, LifeTime lifeTime = LifeTime.Global)
		{
			return container.RegisterProvider<T>(new InstanceProvider<T>(obj, lifeTime), name);
		}

		public static CryoContainer RegisterInstance<T>(this CryoContainer container, T obj, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new InstanceProvider<T>(obj, lifeTime), null);
		}

		public static CryoContainer RegisterInstance<TBase, TDerived>(this CryoContainer container, TDerived obj, string name = null, LifeTime lifeTime = LifeTime.Global) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new InstanceProvider<TDerived>(obj, lifeTime), name);
		}

		public static CryoContainer RegisterInstance<TBase, TDerived>(this CryoContainer container, TDerived obj, LifeTime lifeTime) where TDerived : TBase
		{
			return container.RegisterProvider<TBase>(new InstanceProvider<TDerived>(obj, lifeTime), null);
		}
		#endregion Instance

#if UNITY_5_3_OR_NEWER

		#region SceneObject
		public static CryoContainer RegisterSceneObject<T>(this CryoContainer container, string path, string name = null, LifeTime lifeTime = LifeTime.Global)
		{
			return container.RegisterProvider<T>(new ScenePathProvider<T>(path, lifeTime), name);
		}

		public static CryoContainer RegisterSceneObject<T>(this CryoContainer container, string path, LifeTime lifeTime)
		{
			return container.RegisterProvider<T>(new ScenePathProvider<T>(path, lifeTime), null);
		}
		
		public static CryoContainer RegisterSceneObject<T>(this CryoContainer container, LifeTime lifeTime) where T : UnityEngine.Object
		{
			return container.RegisterProvider<T>(new FindObjectProvider<T>(lifeTime), null);
		}
		
		public static CryoContainer RegisterSceneObject<TBase, TDerived>(this CryoContainer container, LifeTime lifeTime) where TDerived : UnityEngine.Object, TBase
		{
			return container.RegisterProvider<TBase>(new FindObjectProvider<TDerived>(lifeTime), null);
		}

		public static CryoContainer RegisterComponent<T>(this CryoContainer container, FindComponentHint hint = FindComponentHint.ThisGameObject, string name = null, LifeTime lifeTime = LifeTime.Global)
			where T : Component
		{
			return container.RegisterProvider<T>(new GetComponentProvider<T>(hint, lifeTime), name);
		}

		public static CryoContainer RegisterComponent<T>(this CryoContainer container, FindComponentHint hint, LifeTime lifeTime = LifeTime.Global)
			where T : Component
		{
			return container.RegisterProvider<T>(new GetComponentProvider<T>(hint, lifeTime), null);
		}

		public static CryoContainer RegisterComponent<T>(this CryoContainer container, string name, LifeTime lifeTime = LifeTime.Global)
			where T : Component
		{
			return container.RegisterProvider<T>(new GetComponentProvider<T>(FindComponentHint.ThisGameObject, lifeTime), name);
		}

		public static CryoContainer RegisterComponent<T>(this CryoContainer container, LifeTime lifeTime)
			where T : Component
		{
			return container.RegisterProvider<T>(new GetComponentProvider<T>(FindComponentHint.ThisGameObject, lifeTime), null);
		}
		#endregion SceneObject

#endif

	}
}
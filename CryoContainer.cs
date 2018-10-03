﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryoDI.Providers;

namespace CryoDI
{
	/// <summary>
	/// Класс DI-контейнера
	/// </summary>
	public class CryoContainer : IDisposable
	{
		private readonly Dictionary<ContainerKey, IObjectProvider> _providers = new Dictionary<ContainerKey, IObjectProvider>();

		/// <summary>
		/// Проверить, зарегистрирован ли обьект в контейнере
		/// </summary>
		public virtual bool IsRegistered<T>(string name = null)
		{
			return (ResolveProvider(typeof (T), name) != null);
		} 
		
	    public CryoContainer RegisterProvider<T>(IObjectProvider provider, string name = null)
		{
			var key = new ContainerKey(typeof (T), name);
			_providers[key] = provider;
			return this;
		}

		/// <summary>
		/// Получить объект нужного типа
		/// </summary>
		public T Resolve<T>(string name = null)
		{
			return (T) Resolve(typeof (T), name);
		}

		/// <summary>
		/// Получить объект нужного типа
		/// </summary>
		public virtual object Resolve(Type type, string name = null)
		{
			ContainerKey key;
			IObjectProvider provider = ResolveProvider(type, name, out key);
			if (provider == null)
				throw new ContainerException("Can't resolve type " + type.FullName +
				                             (name == null ? "" : " registered with name \"" + name + "\""));

			try
			{
				LifeTimeStack.Push(key, provider.LifeTime);
				return provider.GetObject(this);
			}
			finally
			{
				LifeTimeStack.Pop();
			}
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public virtual T TryResolve<T>(string name = null)
		{
			return (T) TryResolve(typeof (T), name);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public virtual object TryResolve(Type type, string name = null)
		{
			ContainerKey key;
			IObjectProvider provider = ResolveProvider(type, name, out key);
			if (provider == null)
				return null;

			try
			{
				LifeTimeStack.Push(key, provider.LifeTime);
				return provider.GetObject(this);
			}
			finally
			{
				LifeTimeStack.Pop();
			}
		}

	    /// <summary>
	    /// Заинжектить зависимости в уже существующий объект
	    /// </summary>
	    public void BuildUp(object obj)
	    {
		    BuildUpStack.PushObject(obj);

		    try
		    {
			    BuildUp(obj.GetType(), obj);
			    PostBuildUp(obj);
		    }
		    finally
		    {
			    BuildUpStack.Pop();
		    }
	    }

		private void BuildUp(Type type, object obj)
		{
			if (type.BaseType != typeof(object))
				BuildUp(type.BaseType, obj);
			
#if NETFX_CORE
			IEnumerable<MemberInfo> members =
				type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
#else
			MemberInfo[] members = type.FindMembers(MemberTypes.Property,
				BindingFlags.FlattenHierarchy | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null,
				null);
#endif

			foreach (MemberInfo member in members)
			{
				var attrs = member.GetCustomAttributes(typeof(DependencyAttribute), true);
				if (!attrs.Any())
					continue;
				var attr = attrs.FirstOrDefault();

				var attrib = (DependencyAttribute) attr;
				var propertyInfo = (PropertyInfo) member;
				object valueObj;

				BuildUpStack.SetPropertyName(propertyInfo.Name);

				try
				{
					var propertyType = propertyInfo.PropertyType;
					if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IRuntimeResolver<>))
					{
						valueObj = CreateRuntimeResolver(propertyType, attrib.Name);
					}
					else
						valueObj = Resolve(propertyType, attrib.Name);
				}
				catch (ContainerException ex)
				{
					throw new ContainerException(ex.Message + " for " + type.FullName + "." + propertyInfo.Name);
				}
				catch (Exception ex)
				{
					throw new ContainerException(ex.Message + " while resolving " + type.FullName + "." + propertyInfo.Name, ex);
				}

				var setter = propertyInfo.GetSetMethod(true);
				if (setter != null)
					setter.Invoke(obj, new object[] {valueObj});
			}
		}

		/// <summary>
		/// Этот метод вызывается после выставления зависимостей у обьекта
		/// </summary>
		protected virtual void PostBuildUp(object obj)
		{
			
		}

		private object CreateRuntimeResolver(Type propertyType, string name)
		{
			var args = propertyType.GetGenericArguments();
			var resolverType = typeof(RuntimeResolver<>).MakeGenericType(args);
			return Activator.CreateInstance(resolverType, name, this);
		}

		public void Dispose()
		{
			_providers.Clear();
			LifeTimeManager.DisposeAll();
		}

		private IObjectProvider ResolveProvider(Type type, string name, out ContainerKey key)
		{
			key = new ContainerKey(type, name);
			IObjectProvider provider = null;
			_providers.TryGetValue(key, out provider);
			return provider;
		}

		private IObjectProvider ResolveProvider(Type type, string name)
		{
			ContainerKey key;
			return ResolveProvider(type, name, out key);
		}
	}
}
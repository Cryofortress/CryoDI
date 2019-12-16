using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryoDI.Providers;
using DefaultNamespace;

namespace CryoDI
{
	/// <summary>
	/// Класс DI-контейнера
	/// </summary>
	public class CryoContainer : IDisposable
	{
		private readonly Dictionary<ContainerKey, IObjectProvider> _providers = new Dictionary<ContainerKey, IObjectProvider>();
		private readonly BuildUpStack _buildUpStack = new BuildUpStack();
		private readonly LifeTimeStack _lifetimeStack = new LifeTimeStack();
		
		public Reaction OnLifetimeError
		{
			get { return _lifetimeStack.OnLifetimeError; }
			set { _lifetimeStack.OnLifetimeError = value; }
		}
		
		public Reaction OnCircularDependency
		{
			get { return _buildUpStack.OnCircularDependency; }
			set { _buildUpStack.OnCircularDependency = value; }
		}

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
	    public T Resolve<T>(params object[] parameters)
	    {
		    return (T) ResolveByName(typeof (T), null, parameters);
	    }

	    /// <summary>
	    /// Получить объект нужного типа
	    /// </summary>
	    public object Resolve(Type type, params object[] parameters)
	    {
		    return ResolveByName(type, null, parameters);
	    }

		/// <summary>
		/// Получить объект нужного типа
		/// </summary>
		public T ResolveByName<T>(string name, params object[] parameters)
		{
			return (T) ResolveByName(typeof (T), name, parameters);
		}

		/// <summary>
		/// Получить объект нужного типа
		/// </summary>
		public object ResolveByName(Type type, string name, params object[] parameters)
		{
			return ResolveByNameFor(null, type, name, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public T WeakResolve<T>(params object[] parameters)
		{
			return (T) WeakResolveByName(typeof (T), null, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public object WeakResolve(Type type, params object[] parameters)
		{
			return WeakResolveByName(type, null, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public T WeakResolveByName<T>(string name, params object[] parameters)
		{
			return (T) WeakResolveByName(typeof (T), name, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public object WeakResolveByName(Type type, string name, params object[] parameters)
		{
			ContainerKey key;
			IObjectProvider provider = ResolveProvider(type, name, out key);
			if (provider == null)
				throw new ContainerException("Can't resolve type " + type.FullName +
				                             (name == null ? "" : " registered with name \"" + name + "\""));

			_lifetimeStack.Push(key, provider.LifeTime);
			var obj = provider.WeakGetObject(this, parameters);
			_buildUpStack.CheckCircularDependency(obj);
			_lifetimeStack.Pop();
			return obj;
		}

		/// <summary>
	    /// Заинжектить зависимости в уже существующий объект
	    /// </summary>
	    public void BuildUp(object obj, params object[] parameters)
	    {
		    _buildUpStack.PushObject(obj);

			BuildUp(obj.GetType(), obj, parameters);
			PostBuildUp(obj);
			_buildUpStack.Pop();
	    }
		
		public T BuildUp<T>(T obj, params object[] parameters)
		{
			BuildUp((object) obj, parameters);
			return obj;
		}

		private void BuildUp(Type type, object obj, object[] parameters)
		{
			BuildUp(type, obj, ConvertParameters(parameters));
		}

		private Param[] ConvertParameters(object[] parameters)
		{
			var ret = new Param[parameters.Length];
			for(int i = 0; i < parameters.Length; ++i)
			{
				var param = parameters[i] as Param;
				if (param == null)
					param = new Param(parameters[i]);
				
				ret[i] = param;
			}

			return ret;
		}

		private void BuildUp(Type type, object obj, Param[] parameters)
		{
			if (type.BaseType != typeof(object))
				BuildUp(type.BaseType, obj, parameters);
			
#if NETFX_CORE
			IEnumerable<MemberInfo> members =
				type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
#else
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			
#endif

			foreach (PropertyInfo property in properties)
			{
				var dependencyAttr = property.GetCustomAttributes(typeof(DependencyAttribute), true).FirstOrDefault() as DependencyAttribute;
				if (dependencyAttr != null)
				{
					ProcessDependency(type, obj, property, dependencyAttr.Name);
					continue;
				}
				
				var paramAttribute = property.GetCustomAttributes(typeof(ParamAttribute), true).FirstOrDefault() as ParamAttribute;
				if (paramAttribute != null)
				{
					ProcessParam(type, obj, property, parameters);
				}
			}
		}

		private void ProcessParam(Type type, object obj, PropertyInfo propertyInfo, IEnumerable<Param> parameters)
		{	
			object valueObj;

			_buildUpStack.SetPropertyName(propertyInfo.Name);

			try
			{
				valueObj = FindParameter(propertyInfo, parameters);
			}
			catch (ContainerException ex)
			{
				throw new ContainerException(ex.Message + " for " + type.FullName + ":" + propertyInfo.Name);
			}
			catch (Exception ex)
			{
				throw new ContainerException(ex.Message + " while resolving " + type.FullName + ":" + propertyInfo.Name, ex);
			}

			var setter = propertyInfo.GetSetMethod(true);
			if (setter != null)
				setter.Invoke(obj, new[] {valueObj});
		}

		private object FindParameter(PropertyInfo propertyInfo, IEnumerable<Param> parameters)
		{
			
			var propertyName = propertyInfo.Name;
			Type propertyType = propertyInfo.PropertyType;
			
			var param = parameters.FirstOrDefault(p => p.Name == propertyName);
			if (param != null)
			{
				if (param.Value != null && !propertyType.IsInstanceOfType(param.Value))
					throw new ContainerException("Parameter value can't be assigned");
				return param.Value;
			}
			
			foreach (var parameter in parameters)
			{
				if (parameter.Name == null && propertyType.IsInstanceOfType(parameter.Value))
				{
					if (param != null)
						throw new ContainerException("Many assignable parameters were found");
					param = parameter;
				}
			}
			
			if (param != null)
				return param.Value;

			throw new ContainerException("Can't find assignable parameter");
		}

		private void ProcessDependency(Type type, object obj, PropertyInfo propertyInfo, string attribName)
		{
			object valueObj;

			_buildUpStack.SetPropertyName(propertyInfo.Name);

			try
			{
				var propertyType = propertyInfo.PropertyType;

				if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IResolver<>))
				{
					valueObj = CreateResolver(propertyType, attribName);
				}
				else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IWeakResolver<>))
				{
					valueObj = CreateWeakResolver(propertyType, attribName);
				}
				else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IBuilder<>))
				{
					valueObj = CreateBuilder(propertyType);
				}
				else if (propertyType == typeof(IPrefabInstantiator))
				{
					valueObj = CreatePrefabInstantiator();
				}
				else
					valueObj = ResolveByNameFor(obj, propertyType, attribName);
			}
			catch (CircularDependencyException)
			{
				throw;
			}
			catch (WrongLifetimeException)
			{
				throw;
			}
			catch (ContainerException ex)
			{
				throw new ContainerException(ex.Message + " for " + type.FullName + ":" + propertyInfo.Name);
			}
			catch (Exception ex)
			{
				throw new ContainerException(ex.Message + " while resolving " + type.FullName + ":" + propertyInfo.Name, ex);
			}

			var setter = propertyInfo.GetSetMethod(true);
			if (setter != null)
				setter.Invoke(obj, new object[] {valueObj});
		}

		/// <summary>
		/// Этот метод вызывается после выставления зависимостей у обьекта
		/// </summary>
		protected virtual void PostBuildUp(object obj)
		{
			
		}

		private object CreateResolver(Type propertyType, string name)
		{
			var args = propertyType.GetGenericArguments();
			var resolverType = typeof(Resolver<>).MakeGenericType(args);
			return Activator.CreateInstance(resolverType, name, this);
		}

		private object CreateWeakResolver(Type propertyType, string name)
		{
			var args = propertyType.GetGenericArguments();
			var resolverType = typeof(WeakResolver<>).MakeGenericType(args);
			return Activator.CreateInstance(resolverType, name, this);
		}
		
		private object CreateBuilder(Type propertyType)
		{
			var args = propertyType.GetGenericArguments();
			var resolverType = typeof(Builder<>).MakeGenericType(args);
			return Activator.CreateInstance(resolverType, this);
		}
		
		private object CreatePrefabInstantiator()
		{
			return new PrefabInstantiator(this);
		}

		private object ResolveByNameFor(object owner, Type type, string name, params object[] parameters)
		{
			ContainerKey key;
			IObjectProvider provider = ResolveProvider(type, name, out key);
			if (provider == null)
				throw new ContainerException("Can't resolve type " + type.FullName +
				                             (name == null ? "" : " registered with name \"" + name + "\""));

			_lifetimeStack.Push(key, provider.LifeTime);
			var obj = provider.GetObject(owner, this, parameters);
			_buildUpStack.CheckCircularDependency(obj);
			_lifetimeStack.Pop();
			return obj;
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

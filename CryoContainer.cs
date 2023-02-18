using System;
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
		private readonly Dictionary<ContainerKey, IObjectProvider> _providers =
			new Dictionary<ContainerKey, IObjectProvider>();

		private readonly List<IFactory> _factories = new List<IFactory>();

		private readonly CryoContainer _parentContainer;

		private readonly BuildUpStack _buildUpStack = new BuildUpStack();
		private readonly LifeTimeStack _lifetimeStack = new LifeTimeStack();
		private readonly LifeTimeManager _lifeTimeManager = new LifeTimeManager();

		public Reaction LifetimeErrorReaction
		{
			get { return _lifetimeStack.LifetimeErrorReaction; }
			set { _lifetimeStack.LifetimeErrorReaction = value; }
		}

		public Reaction CircularDependencyReaction
		{
			get { return _buildUpStack.CircularDependencyReaction; }
			set { _buildUpStack.CircularDependencyReaction = value; }
		}

		public ILifeTimeManager LifeTimeManager
		{
			get { return _lifeTimeManager; }
		}

		public CryoContainer()
		{
			RegisterFactory(new BuilderFactory());
			RegisterFactory(new ResolverFactory());
			RegisterFactory(new WeakResolverFactory());
			
#if UNITY_5_3_OR_NEWER
			RegisterFactory(new PrefabInstantiatorFactory());
#endif

		}

		public CryoContainer(CryoContainer parentContainer) : this()
		{
			_parentContainer = parentContainer;
		}

		public void Dispose()
		{
			_providers.Clear();
			_lifeTimeManager.DisposeAll();
		}

		public CryoContainer RegisterProvider<T>(IObjectProvider provider, string name = null)
		{
			var key = new ContainerKey(typeof(T), name);
			_providers[key] = provider;
			return this;
		}

		public CryoContainer RegisterFactory(IFactory factory)
		{
			_factories.Add(factory);
			return this;
		}

		/// <summary>
		/// Проверить, зарегистрирован ли обьект в контейнере
		/// </summary>
		public virtual bool IsRegistered<T>(string name = null)
		{
			if (ResolveProvider(typeof(T), name, out _) != null)
				return true;
			return _factories.Any(f => f.CanCreate(typeof(T), name));
		}

		/// <summary>
		/// Получить объект нужного типа
		/// </summary>
		public T Resolve<T>(params object[] parameters)
		{
			return (T) ResolveByName(typeof(T), null, parameters);
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
			return (T) ResolveByName(typeof(T), name, parameters);
		}

		/// <summary>
		/// Получить объект нужного типа
		/// </summary>
		public virtual object ResolveByName(Type type, string name, params object[] parameters)
		{
			return ResolveByNameFor(null, type, name, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public virtual T TryResolve<T>(params object[] parameters)
		{
			return (T) TryResolveByName(typeof(T), null, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public virtual object TryResolve(Type type, params object[] parameters)
		{
			return TryResolveByName(type, null, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public virtual T TryResolveByName<T>(string name, params object[] parameters)
		{
			return (T) TryResolveByName(typeof(T), name, parameters);
		}

		/// <summary>
		/// Попытаться получить объект нужного типа. Если объекта нет, то возвращет null не кидая исключения.
		/// </summary>
		public virtual object TryResolveByName(Type type, string name, params object[] parameters)
		{
			IObjectProvider provider = ResolveProvider(type, name, out var key);
			if (provider != null)
			{
				_lifetimeStack.Push(key, provider.LifeTime);
				var obj = provider.WeakGetObject(this, parameters);
				if (obj != null)
					_buildUpStack.CheckCircularDependency(obj);
				_lifetimeStack.Pop();
				return obj;
			}

			var factory = _factories.FirstOrDefault(f => f.CanCreate(type, name));
			if (factory != null)
			{
				_lifetimeStack.Push(key, factory.LifeTime);
				var obj = factory.Create(type, name, this);
				if (obj != null)
					_buildUpStack.CheckCircularDependency(obj);
				_lifetimeStack.Pop();
				return obj;
			}

			if (_parentContainer != null)
			{
				var ret = _parentContainer.TryResolveByName(type, name, parameters);
				if (ret != null) return ret;
			}

			return null;
		}

		/// <summary>
		/// Заинжектить зависимости в уже существующий объект
		/// </summary>
		public void BuildUp(object obj, params object[] parameters)
		{
			_buildUpStack.PushObject(obj);

			try
			{
				BuildUp(obj.GetType(), obj, parameters);
				PostBuildUp(obj);
			}
			finally
			{
				_buildUpStack.Pop();
			}
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
			for (int i = 0; i < parameters.Length; ++i)
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
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
			                                    BindingFlags.DeclaredOnly);

#endif

			foreach (PropertyInfo property in properties)
			{
				var dependencyAttr =
					property.GetCustomAttributes(typeof(DependencyAttribute), true).FirstOrDefault() as
						DependencyAttribute;
				if (dependencyAttr != null)
				{
					ProcessDependency(type, obj, property, dependencyAttr.Name);
					continue;
				}

				var paramAttribute =
					property.GetCustomAttributes(typeof(ParamAttribute), true).FirstOrDefault() as ParamAttribute;
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
				throw new ContainerException(ex.Message + " while resolving " + type.FullName + ":" + propertyInfo.Name,
					ex);
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
				valueObj = ResolveByNameFor(obj, propertyInfo.PropertyType, attribName);
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
				throw new ContainerException(ex.Message + " while resolving " + type.FullName + ":" + propertyInfo.Name,
					ex);
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
		
		private object ResolveByNameFor(object owner, Type type, string name, params object[] parameters)
		{
			IObjectProvider provider = ResolveProvider(type, name, out var key);
			if (provider != null)
			{
				_lifetimeStack.Push(key, provider.LifeTime);
				var obj = provider.GetObject(owner, this, parameters);
				_buildUpStack.CheckCircularDependency(obj);
				_lifetimeStack.Pop();
				return obj;
			}
			
			var factory = _factories.FirstOrDefault(f => f.CanCreate(type, name));
			if (factory != null)
			{
				_lifetimeStack.Push(key, factory.LifeTime);
				var obj = factory.Create(type, name, this);
				if (obj != null)
					_buildUpStack.CheckCircularDependency(obj);
				_lifetimeStack.Pop();
				return obj;
			}

			if (_parentContainer != null)
				return _parentContainer.ResolveByNameFor(owner, type, name, parameters);

			throw new ContainerException("Can't resolve type " + type.FullName +
			                             (name == null ? "" : " registered with name \"" + name + "\""));
		}

		private IObjectProvider ResolveProvider(Type type, string name, out ContainerKey key)
		{
			key = new ContainerKey(type, name);
			IObjectProvider provider = null;
			_providers.TryGetValue(key, out provider);
			return provider;
		}
	}
}
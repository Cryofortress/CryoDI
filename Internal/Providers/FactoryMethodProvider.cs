using System;

namespace CryoDI.Providers
{
	internal class FactoryMethodProvider<T> : IObjectProvider
	{
		private readonly Func<T> _factoryMethod;
		private readonly ILifeTimeManager _lifeTimeManager;

		public FactoryMethodProvider(ILifeTimeManager lifeTimeManager, LifeTime lifeTime)
			: this(Activator.CreateInstance<T>, lifeTimeManager, lifeTime)
		{
		}

		public FactoryMethodProvider(Func<T> factoryMethod, ILifeTimeManager lifeTimeManager, LifeTime lifeTime)
		{
			LifeTime = lifeTime;
			_lifeTimeManager = lifeTimeManager;
			_factoryMethod = factoryMethod;
		}

		public LifeTime LifeTime { get; }

		public object GetObject(object owner, CryoContainer container, params object[] parameters)
		{
			var obj = _factoryMethod();
			container.BuildUp(obj, parameters);

			if (obj is IDisposable disposable)
				_lifeTimeManager.Add(disposable, LifeTime);
			return obj;
		}

		public object WeakGetObject(CryoContainer container, params object[] parameters)
		{
			return null;
		}

		public void Dispose()
		{
			// do nothing
		}
	}
}
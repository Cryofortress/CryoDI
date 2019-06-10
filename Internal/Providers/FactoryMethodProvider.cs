using System;

namespace CryoDI.Providers
{
	internal class FactoryMethodProvider<T> : IObjectProvider
	{
		private readonly Func<T> _factoryMethod;
		
	    public FactoryMethodProvider(LifeTime lifeTime)
	    {
	        LifeTime = lifeTime;
		    _factoryMethod = Activator.CreateInstance<T>;
	    }
		
		public FactoryMethodProvider(Func<T> factoryMethod, LifeTime lifeTime)
		{
			LifeTime = lifeTime;
			_factoryMethod = factoryMethod;
		}

		public LifeTime LifeTime { get; private set; }

		public object GetObject(CryoContainer container, params object[] parameters)
		{
			var obj = _factoryMethod();
		    container.BuildUp(obj, parameters);

			LifeTimeManager.TryToAdd(obj, LifeTime);
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

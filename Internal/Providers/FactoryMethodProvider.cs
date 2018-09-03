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

		public object GetObject(Container container)
		{
			var obj = _factoryMethod();
		    container.BuildUp(obj);

			LifeTimeManager.TryToAdd(obj, LifeTime);
			return obj;
		}

		public void Dispose()
		{
			// do nothing
		}
	}
}

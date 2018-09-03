using System;

namespace CryoDI.Providers
{
	internal class SingletonProvider<T> : IObjectProvider
	{
		private T _instance;
	    private bool _created;
		private readonly Func<T> _factoryMethod;

	    public SingletonProvider(LifeTime lifeTime) 
		    : this(Activator.CreateInstance<T>, lifeTime)
	    {
	    }
		
		public SingletonProvider(Func<T> factoryMethod, LifeTime lifeTime)
		{
			LifeTime = lifeTime;
			_factoryMethod = factoryMethod;
		}

		public LifeTime LifeTime { get; private set; }

		public object GetObject(Container container)
		{
			if (!_created)
			{
				_instance = _factoryMethod();
				_created = true;

			    container.BuildUp(_instance);
			    LifeTimeManager.TryToAdd(this, LifeTime);
			}
			return _instance;
		}

	    public void Dispose()
	    {
		    if (!_created)
			    return;

		    if (LifeTime != LifeTime.External)
		    {
			    var disposable = _instance as IDisposable;
			    if (disposable != null)
				    disposable.Dispose();
		    }
		    _instance = default(T);
		    _created = false;
	    }
	}
}

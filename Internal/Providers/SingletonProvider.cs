using System;

namespace CryoDI.Providers
{
	internal class SingletonProvider<T> : IObjectProvider where T : new()
	{
		private T _instance;
	    private bool _created;

	    public SingletonProvider(LifeTime lifeTime)
	    {
	        LifeTime = lifeTime;
	    }

		public LifeTime LifeTime { get; private set; }

		public object GetObject(Container container)
		{
			if (!_created)
			{
				_instance = Activator.CreateInstance<T>();
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

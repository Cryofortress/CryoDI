using System;

namespace CryoDI.Providers
{
	internal class InstanceProvider<T> : IObjectProvider
	{
		private T _instance;
	    private bool _disposed = false;

		public InstanceProvider(T instance, LifeTime lifeTime)
		{
			_instance = instance;
			LifeTime = lifeTime;

			LifeTimeManager.TryToAdd(this, LifeTime);
		}

		public LifeTime LifeTime { get; private set; }

		public object GetObject(CryoContainer container, params object[] unused)
		{
		    if (_disposed)
		        throw new ContainerException("Instance of type " + typeof(T) + " already disposed");
			return _instance;
		}

		public object WeakGetObject(CryoContainer container, params object[] unused)
		{
			if (_disposed) return null;
			return _instance;
		}

	    public void Dispose()
	    {
		    if (_disposed)
			    return;
	        var disposable = _instance as IDisposable;
	        if (disposable != null)
	            disposable.Dispose();
	        _instance = default (T);
	        _disposed = true;
	    }
	}
}

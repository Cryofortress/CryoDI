using System;

namespace CryoDI.Providers
{
	internal class ActivatorObjectProvider<T> : IObjectProvider where T : new()
	{
	    public ActivatorObjectProvider(LifeTime lifeTime)
	    {
	        LifeTime = lifeTime;
	    }

		public LifeTime LifeTime { get; private set; }

		public object GetObject(Container container)
		{
			var obj = Activator.CreateInstance<T>();
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

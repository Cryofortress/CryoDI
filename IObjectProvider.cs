using System;

namespace CryoDI.Providers
{
	public interface IObjectProvider : IDisposable
	{
		LifeTime LifeTime { get; }
		object GetObject(Container container);
	}
}

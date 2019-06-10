using System;

namespace CryoDI.Providers
{
	public interface IObjectProvider : IDisposable
	{
		LifeTime LifeTime { get; }
		object GetObject(CryoContainer container, params object[] parameters);
		object WeakGetObject(CryoContainer container, params object[] parameters);
	}
}

using System;

namespace CryoDI.Providers
{
	internal interface IObjectProvider : IDisposable
	{
		LifeTime LifeTime { get; }
		object GetObject(Container container);
	}
}

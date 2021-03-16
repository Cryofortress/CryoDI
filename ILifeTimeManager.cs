using System;

namespace CryoDI
{
	public interface ILifeTimeManager
	{
		void Add(IDisposable disposable, LifeTime lifeTime);
	}
}
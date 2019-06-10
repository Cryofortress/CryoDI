#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI.Providers
{
	public class FindObjectProvider<T> : IObjectProvider where T : Object
	{
		private T _cached;

		public FindObjectProvider(LifeTime lifeTime)
		{
			LifeTime = lifeTime;
		}

		public LifeTime LifeTime { get; private set; }

		public object GetObject(CryoContainer container, params object[] parameters)
		{
			if (IsDestroyed())
			{
				_cached = FindObject();

				var cryoBehaviour = _cached as CryoBehaviour;
				if (cryoBehaviour != null && !cryoBehaviour.BuiltUp)
				{
					cryoBehaviour.BuildUp();
				}

				LifeTimeManager.TryToAdd(this, LifeTime);
			}
			return _cached;
		}

		public object WeakGetObject(CryoContainer container, params object[] parameters)
		{
			if (IsDestroyed())
			{
				_cached = Object.FindObjectOfType<T>();
				if (_cached == null)
					return null;

				var cryoBehaviour = _cached as CryoBehaviour;
				if (cryoBehaviour != null && !cryoBehaviour.BuiltUp)
				{
					cryoBehaviour.BuildUp();
				}

				LifeTimeManager.TryToAdd(this, LifeTime);
			}
			return _cached;
		}

		public void Dispose()
		{
			if (LifeTime != LifeTime.External)
			{
				System.IDisposable disposable;
				if (_cached != null && (disposable = _cached as System.IDisposable) != null)
					disposable.Dispose();
			}
			_cached = default (T);
		}

		private bool IsDestroyed()
		{
			if (_cached == null)
				return true;

			if (typeof(T) == typeof(GameObject))
			{
				GameObject gameObj = (GameObject) (object)_cached;
				return !gameObj;
			}

			var component = _cached as Component;
			if (component)
			{
				return !component.gameObject;
			}

			return true;
		}

		private T FindObject()
		{
			var obj = Object.FindObjectOfType<T>();
			if (obj == null)
				throw new ContainerException("Can't find object of type \"" +typeof(T) + "\"");

			return obj;
		}
	}
}
#endif
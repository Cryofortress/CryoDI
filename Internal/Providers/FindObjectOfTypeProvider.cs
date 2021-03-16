#if UNITY_5_3_OR_NEWER
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CryoDI.Providers
{
	/// <summary>
	/// Провайдер для поиска объектов в сцене, который использует UnityEngine.Object.FindObjectOfType()
	/// </summary>
	public class FindObjectOfTypeProvider<T> : IObjectProvider where T : Object
	{
		private T _cached;
		private readonly ILifeTimeManager _lifeTimeManager;

		public FindObjectOfTypeProvider(ILifeTimeManager lifeTimeManager, LifeTime lifeTime)
		{
			_lifeTimeManager = lifeTimeManager;
			LifeTime = lifeTime;
		}

		public LifeTime LifeTime { get; }

		public object GetObject(object owner, CryoContainer container, params object[] parameters)
		{
			if (IsDestroyed())
			{
				_cached = FindObject();

				if (_cached is CryoBehaviour cryoBehaviour)
				{
					if (!cryoBehaviour.BuiltUp)
						cryoBehaviour.BuildUp();
				}
				else if (_cached is Component component)
				{
					var cryoBuilder = component.GetComponent<CryoBuilder>();
					if (cryoBuilder != null && !cryoBuilder.BuiltUp)
						cryoBuilder.BuildUp();
				}

				_lifeTimeManager.Add(this, LifeTime);
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
				if (cryoBehaviour != null && !cryoBehaviour.BuiltUp) cryoBehaviour.BuildUp();

				_lifeTimeManager.Add(this, LifeTime);
			}

			return _cached;
		}

		public void Dispose()
		{
			if (LifeTime != LifeTime.External)
			{
				IDisposable disposable;
				if (_cached != null && (disposable = _cached as IDisposable) != null)
					disposable.Dispose();
			}

			_cached = default;
		}

		private bool IsDestroyed()
		{
			if (_cached == null)
				return true;

			if (typeof(T) == typeof(GameObject))
			{
				var gameObj = (GameObject) (object) _cached;
				return !gameObj;
			}

			var component = _cached as Component;
			if (component) return !component.gameObject;

			return true;
		}

		private T FindObject()
		{
			var obj = Object.FindObjectOfType<T>();
			if (obj == null)
				throw new ContainerException("Can't find object of type \"" + typeof(T) + "\"");

			return obj;
		}
	}
}
#endif
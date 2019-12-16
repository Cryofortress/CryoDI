using UnityEngine;
#if UNITY_5_3_OR_NEWER
using System;
using System.Linq;

namespace CryoDI.Providers
{
	internal class ScenePathProvider<T> : IObjectProvider
	{
		private readonly string _path;
		private T _cached;

		public ScenePathProvider(string path, LifeTime lifeTime)
		{
			_path = path;
			LifeTime = lifeTime;
		}

		public LifeTime LifeTime { get; }

		public object GetObject(object owner, CryoContainer container, params object[] parameters)
		{
			if (IsDestroyed())
			{
				_cached = FindObject();

				var cryoBehaviour = _cached as CryoBehaviour;
				if (cryoBehaviour != null && !cryoBehaviour.BuiltUp) cryoBehaviour.BuildUp();

				LifeTimeManager.TryToAdd(this, LifeTime);
			}

			return _cached;
		}

		public object WeakGetObject(CryoContainer container, params object[] parameters)
		{
			if (IsDestroyed())
			{
				if (!TryToFindObject(out _cached))
					return null;

				var cryoBehaviour = _cached as CryoBehaviour;
				if (cryoBehaviour != null && !cryoBehaviour.BuiltUp) cryoBehaviour.BuildUp();

				LifeTimeManager.TryToAdd(this, LifeTime);
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
			var gameObject = new MaskFinder().Find(_path);
			if (gameObject == null)
				throw new ContainerException("Can't find game object \"" + _path + "\"");

			if (typeof(T) == typeof(GameObject))
				return (T) (object) gameObject;

			if (typeof(T) == typeof(Transform))
				return (T) (object) gameObject.transform;

			var components = gameObject.GetComponents<Component>();
			var component = components.OfType<T>().FirstOrDefault();
			if (component != null)
				return component;

			throw new ContainerException("Can't find component \"" + typeof(T).FullName + "\" of game object \"" +
			                             _path + "\"");
		}

		private bool TryToFindObject(out T value)
		{
			var gameObject = new MaskFinder().Find(_path);
			if (gameObject == null)
			{
				value = default;
				return false;
			}

			if (typeof(T) == typeof(GameObject))
			{
				value = (T) (object) gameObject;
				return true;
			}

			if (typeof(T) == typeof(Transform))
			{
				value = (T) (object) gameObject.transform;
				return true;
			}

			var components = gameObject.GetComponents<Component>();
			var component = components.OfType<T>().FirstOrDefault();
			if (component != null)
			{
				value = component;
				return true;
			}

			value = default;
			return false;
		}
	}
}

#endif
#if UNITY_5_3_OR_NEWER

using System;
using System.Linq;
using UnityEngine;

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

		public LifeTime LifeTime { get; private set; }

		public object GetObject(Container container)
		{
			if (IsDestroyed())
			{
				_cached = FindObject();

			    var sceneDependent = _cached as SceneDependent;
				if (sceneDependent != null && !sceneDependent.BuiltUp)
				{
					sceneDependent.BuildUp();
				}

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
			var gameObject = new MaskFinder().Find(_path);
			if (gameObject == null)
				throw new ContainerException("Can't find game object \"" + _path + "\"");

			if (typeof (T) == typeof (GameObject))
				return (T) (object) gameObject;

			if (typeof (T) == typeof (Transform))
				return (T) (object) gameObject.transform;

			var components = gameObject.GetComponents<Component>();
			T component = components.OfType<T>().FirstOrDefault();
			if (component != null)
				return component;

			throw new ContainerException("Can't find component \"" + typeof (T).FullName + "\" of game object \"" + _path + "\"");
		}
	}
}

#endif

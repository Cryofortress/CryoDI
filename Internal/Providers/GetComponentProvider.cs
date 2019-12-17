#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace CryoDI.Providers
{
	public class GetComponentProvider<T> : IObjectProvider where T : Component
	{
		private readonly FindComponentHint _hint;

		public GetComponentProvider(FindComponentHint hint, LifeTime lifeTime)
		{
			LifeTime = lifeTime;
			_hint = hint;
		}

		public LifeTime LifeTime { get; }

		public object GetObject(object owner, CryoContainer container, params object[] parameters)
		{
			var component = FindComponentAt(owner);

			if (component is CryoBehaviour cryoBehaviour)
			{
				if (!cryoBehaviour.BuiltUp)
					cryoBehaviour.BuildUp();
			}
			else
			{
				var cryoBuilder = component.GetComponent<CryoBuilder>();
				if (cryoBuilder != null && !cryoBuilder.BuiltUp)
					cryoBuilder.BuildUp();
			}

			LifeTimeManager.TryToAdd(component, LifeTime);
			return component;
		}

		public object WeakGetObject(CryoContainer container, params object[] parameters)
		{
			return null;
		}

		public void Dispose()
		{
			// do nothing
		}

		private T FindComponentAt(object owner)
		{
			T component;
			if (owner is GameObject gameObject)
			{
				if (_hint == FindComponentHint.ThisGameObject)
					component = gameObject.GetComponent<T>();
				else if (_hint == FindComponentHint.InChildren)
					component = gameObject.GetComponentInChildren<T>();
				else //if (_hint == FindComponentHint.InParent)
					component = gameObject.GetComponentInParent<T>();
			}
			else if (owner is Component otherComponent)
			{
				if (_hint == FindComponentHint.ThisGameObject)
					component = otherComponent.GetComponent<T>();
				else if (_hint == FindComponentHint.InChildren)
					component = otherComponent.GetComponentInChildren<T>();
				else //if (_hint == FindComponentHint.InParent)
					component = otherComponent.GetComponentInParent<T>();
			}
			else
			{
				throw new ContainerException("Can't find object of type \"" + typeof(T) +
				                             "\" because owner is not GameObject or Component");
			}

			return component;
		}
	}
}

#endif
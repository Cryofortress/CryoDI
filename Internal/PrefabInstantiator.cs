#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace CryoDI
{
	internal class PrefabInstantiator : IPrefabInstantiator
	{
		private readonly CryoContainer _container;

		public PrefabInstantiator(CryoContainer container)
		{
			_container = container;
		}
		
#if UNITY_5_3_OR_NEWER
		public Object Instantiate(Object prefab, params object[] parameters)
		{
			var instance = Object.Instantiate(prefab);
			TryToBuildUp(instance, parameters);
			return instance;
		}

		public Object Instantiate(Object prefab, Transform parent, params object[] parameters)
		{
			var instance = Object.Instantiate(prefab, parent);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public Object Instantiate(Object prefab, Transform parent, bool instantiateInWorldSpace, params object[] parameters)
		{
			var instance = Object.Instantiate(prefab, parent, instantiateInWorldSpace);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public Object Instantiate(Object prefab, Vector3 pos, Quaternion rot, params object[] parameters)
		{
			var instance = Object.Instantiate(prefab, pos, rot);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public Object Instantiate(Object prefab, Vector3 pos, Quaternion rot, Transform parent, params object[] parameters)
		{
			var instance = Object.Instantiate(prefab, pos, rot, parent);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public T Instantiate<T>(T prefab, params object[] parameters) where T : Object
		{
			var instance = Object.Instantiate<T>(prefab);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public T Instantiate<T>(T prefab, Transform parent, params object[] parameters) where T : Object
		{
			var instance = Object.Instantiate<T>(prefab, parent);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public T Instantiate<T>(T prefab, Transform parent, bool instantiateInWorldSpace, params object[] parameters) where T : Object
		{
			var instance = Object.Instantiate<T>(prefab, parent, instantiateInWorldSpace);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public T Instantiate<T>(T  prefab, Vector3 pos, Quaternion rot, params object[] parameters) where T : Object
		{
			var instance = Object.Instantiate<T>(prefab, pos, rot);
			TryToBuildUp(instance, parameters);
			return instance;
		}
		
		public T Instantiate<T>(T  prefab, Vector3 pos, Quaternion rot, Transform parent, params object[] parameters) where T : Object
		{
			var instance = Object.Instantiate<T>(prefab, pos, rot, parent);
			TryToBuildUp(instance, parameters);
			return instance;
		}

		private void TryToBuildUp<T>(T instance, object[] parameters) where T : Object
		{
			GameObject gameObj = instance as GameObject;
			if (gameObj != null)
			{
				BuildUpTree(gameObj, parameters);
			}
			else
			{
				Component component = instance as Component;
				if (component != null)
					BuildUpTree(component.gameObject, parameters);
			}
		}
		
		private void BuildUpTree(GameObject gameObj, object[] parameters)
		{
			var monoBehaviours = gameObj.GetComponentsInChildren<MonoBehaviour>();
			foreach (var monoBehaviour in monoBehaviours)
			{
				if (monoBehaviour is CryoBehaviour)
					continue;
				
				_container.BuildUp(monoBehaviour, parameters);
			}
		}
#endif
	}
}
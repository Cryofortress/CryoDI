#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace CryoDI
{
	/// <summary>
	/// Инстанциатор префабов
	/// TODO: доработать, чтобы корректно обрабатывались параметры в CryoBehaviour
	/// </summary>
	public interface IPrefabInstantiator
	{
#if UNITY_5_3_OR_NEWER
		Object Instantiate(Object prefab, params object[] parameters);

		Object Instantiate(Object prefab, Transform parent, params object[] parameters);

		Object Instantiate(Object prefab, Transform parent, bool instantiateInWorldSpace, params object[] parameters);

		Object Instantiate(Object prefab, Vector3 pos, Quaternion rot, params object[] parameters);

		Object Instantiate(Object prefab, Vector3 pos, Quaternion rot, Transform parent, params object[] parameters);

		T Instantiate<T>(T prefab, params object[] parameters) where T : Object;

		T Instantiate<T>(T prefab, Transform parent, params object[] parameters) where T : Object;
		
		T Instantiate<T>(T prefab, Transform parent, bool instantiateInWorldSpace, params object[] parameters) where T : Object;

		T Instantiate<T>(T prefab, Vector3 pos, Quaternion rot, params object[] parameters) where T : Object;

		T Instantiate<T>(T prefab, Vector3 pos, Quaternion rot, Transform parent, params object[] parameters) where T : Object;
#endif
	}
}

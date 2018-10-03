using UnityEngine;

namespace CryoDI
{
#if UNITY_5_3_OR_NEWER
	
	/// <summary>
	/// Класс, предназначенный для создания и настройки контейнера в Unity
	/// </summary>
	public abstract class UnityContainerBuilder : MonoBehaviour
	{
		private static UnityContainerBuilder _instance;
		private static CryoContainer _container;
		
		public static CryoContainer Container
		{
			get
			{
				if (_container == null)
					_instance.CreateContainer();

				return _container;
			}
		}

		protected UnityContainerBuilder()
		{
			if (_instance != null)
			{
				Debug.LogError("ContainerBuilder already exist: " + _instance.gameObject.name , _instance);
				throw new ContainerException("Instance already created");
			}

			_instance = this;
		}

		protected void Awake()
		{
			if (_container != null)
				CreateContainer();
		}
		
		protected abstract void SetupContainer(CryoContainer container);

		private void CreateContainer()
		{
			_container = new CryoContainer();
			SetupContainer(_container);
		}
	}
#endif
}
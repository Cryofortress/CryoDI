using UnityEngine;

namespace CryoDI
{
#if UNITY_5_3_OR_NEWER
	
	/// <summary>
	/// Класс, предназначенный для создания и настройки контейнера в Unity
	/// </summary>
	public class UnityStarter : MonoBehaviour, IInternalContainerBuidler
	{
		private static UnityStarter _instance;
		private CryoContainer _container;
		
		protected UnityStarter()
		{
			if (_instance != null)
			{
				throw new ContainerException("UnityStarter already created");
			}

			_instance = this;
		}

		public static UnityStarter Instance
		{
			get
			{
				if (_instance == null)
					throw new ContainerException("UnityStarter not found");
				return _instance;
			}
		}
		
		/// <summary>
		/// Override this method to customize containers content
		/// </summary>
		protected virtual void SetupContainer(CryoContainer container)
		{
		}
		
		void IInternalContainerBuidler.CreateRootContainer()
		{
			_container = new CryoContainer();
			SetupContainer(_container);
			CryoBehaviour.SetRootContainer(_container);
		}

		private void Awake()
		{
			if (_container != null)
				((IInternalContainerBuidler)this).CreateRootContainer();
		}

		private void OnDestroy()
		{
			_container.Dispose();
		}
	}
#endif
}
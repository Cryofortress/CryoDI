#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI
{	
	/// <summary>
	/// Класс, предназначенный для создания и настройки контейнера в Unity
	/// </summary>
	public class UnityStarter : MonoBehaviour, IInternalContainerBuidler
	{
		private static UnityStarter _instance;
		private CryoContainer _container;
		
		public static UnityStarter Instance
		{
			get
			{
				if (_instance == null)
					_instance = GameObject.FindObjectOfType<UnityStarter>();
				
				if (_instance == null)
					throw new ContainerException("UnityStarter not found");
				return _instance;
			}
		}

		/// <summary>
		/// Override this method to create container derived from CryoContainer  
		/// </summary>
		protected virtual CryoContainer CreateContainer()
		{
			return new CryoContainer();
		}
		
		/// <summary>
		/// Override this method to customize containers content
		/// </summary>
		protected virtual void SetupContainer(CryoContainer container)
		{
		}
		
		void IInternalContainerBuidler.CreateRootContainer()
		{
			_container = CreateContainer();
			SetupContainer(_container);
			CryoBehaviour.SetRootContainer(_container);
		}

		private void Awake()
		{
			if (_container == null)
				((IInternalContainerBuidler)this).CreateRootContainer();
		}

		private void OnDestroy()
		{
			if (_container != null)
				_container.Dispose();
		}
	}
}
#endif
#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI
{
	public sealed class CryoBuilder : MonoBehaviour
	{
		private static CryoContainer _rootContainer;

		public bool BuiltUp { get; private set; }

		private void Awake()
		{
			BuildUp();
		}

		public void BuildUp()
		{
			if (!BuiltUp)
			{
				BuiltUp = true;
				EnsureContainerExists();
				var monoBehaviours = GetComponents<MonoBehaviour>();
				foreach (var monoBehaviour in monoBehaviours)
				{
					if (monoBehaviour is CryoBehaviour)
						continue;
					_rootContainer.BuildUp(monoBehaviour);
				}

				_rootContainer.BuildUp(this);
			}
		}

		private static void EnsureContainerExists()
		{
			if (_rootContainer == null)
				((IInternalContainerBuidler) UnityStarter.Instance).CreateRootContainer();
		}

		public static void SetRootContainer(CryoContainer container)
		{
			if (_rootContainer != null)
				throw new ContainerException("Root container already set");
			_rootContainer = container;
		}
	}
}
#endif